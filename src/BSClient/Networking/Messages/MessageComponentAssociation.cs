using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Core.World.Actors;


namespace JollyBit.BS.Client.Networking.Messages
{
    public class MessageComponentAssociation
    {
        public Type ComponentType { get; private set; }
        public Type MessageType { get; private set; }
        private MessageComponentAssociation(Type messageType, Type componentType)
        {
            ComponentType = componentType;
            MessageType = messageType;
        }

        public static MessageComponentAssociation Create<TMessage, TComponent>() 
            where TComponent : IActorComponent
            where TMessage : ActorMessage
        {
            return new MessageComponentAssociation(typeof(TMessage), typeof(TComponent));
        }
    }
}
