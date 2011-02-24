using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Client.Networking.Messages;
using JollyBit.BS.Core.World.Actors;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;

namespace JollyBit.BS.Client.World.Actors
{
    public interface IClientSyncComponent : IActorComponent
    {
        event EventHandler<EventArgs<Tuple<IConnection, ActorMessage>>> MessageReceived;
    }
    /// <summary>
    /// Provides IClientSyncComponents. Most of the functionality of the IClientSyncComponent
    /// is implemented in the provider allowing IClientSyncComponents to be very lightweight.
    /// </summary>
    public class ClientSyncComponentProvider : Provider<IClientSyncComponent>
    {
        private readonly IConnection _connection;
        private readonly IKernel _kernel;
        private readonly IDictionary<Type, MessageComponentAssociation> _messageTypeToAssociation = new Dictionary<Type, MessageComponentAssociation>();
        public ClientSyncComponentProvider(IKernel kernel, IConnection connection)
        {
            _connection = connection;
            _kernel = kernel;
            foreach (var association in _kernel.GetAll<MessageComponentAssociation>())
                _messageTypeToAssociation.Add(association.MessageType, association);
            _connection.MessageReceived += new EventHandler<EventArgs<object>>(_connection_MessageReceived);
        }
        void _connection_MessageReceived(object sender, EventArgs<object> e)
        {
            ActorMessage msg;
            if ((msg = e.Data as ActorMessage) != null)
            {
                //Get actor via ActorId
                IActor actor = _kernel.Get<IActor>(new Parameter("actorid", msg.ActorId, false));
                //create associated component on actor if not already present
                MessageComponentAssociation association;
                if (_messageTypeToAssociation.TryGetValue(msg.GetType(), out association))
                {
                    actor.Get(association.ComponentType);
                }
                //get sync component and raise message received
                SyncComponent syncComponent = actor.Get<IClientSyncComponent>() as SyncComponent;
                if (syncComponent != null) syncComponent.RaiseMessageReceived(_connection, msg);
            }
        }

        protected override IClientSyncComponent CreateInstance(IContext context)
        {
            return _kernel.Get<SyncComponent>();
        }

        private class SyncComponent : IClientSyncComponent
        {
            public void RaiseMessageReceived(IConnection connection, ActorMessage message)
            {
                if (MessageReceived != null) MessageReceived(this, new EventArgs<Tuple<IConnection, ActorMessage>>(Tuple.Create(connection, message)));
            }
            public event EventHandler<EventArgs<Tuple<IConnection, ActorMessage>>> MessageReceived;
            [Inject]
            public IActor Actor { get; set; }
        }
    }
}
