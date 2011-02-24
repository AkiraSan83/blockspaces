using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.World.Actors;
using OpenTK;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Client.World.Actors
{
    public class PositionComponent : PositionableComponentBase
    {
        private readonly IClientSyncComponent _syncComponent;
        private readonly ILogger _logger;
        public PositionComponent(IActor actor, IClientSyncComponent syncComponent, ILogger logger)
            : base(actor)
        {
            _logger = logger;
            _syncComponent = syncComponent;
            _syncComponent.MessageReceived += new EventHandler<Core.Utility.EventArgs<Tuple<Core.Networking.IConnection, Core.Networking.Messages.ActorMessage>>>(_syncComponent_MessageReceived);
        }

        void _syncComponent_MessageReceived(object sender, Core.Utility.EventArgs<Tuple<Core.Networking.IConnection, Core.Networking.Messages.ActorMessage>> e)
        {
            PositionMessage msg;
            if ((msg = e.Data.Item2 as PositionMessage) != null)
            {
                Position = msg.Position;
                Rotation = msg.Rotation;
                _logger.Debug("actor #{0} moved. Position={1} Rotation={2}", Actor.ActorId, Position, Rotation);
            }
        }

        protected override void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition) { }

        protected override void OnRotationChanged(Quaternion oldRotation, Quaternion newRotation) { }
    }
}
