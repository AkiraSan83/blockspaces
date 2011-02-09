using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using ProtoBuf;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableOrdered, SequenceChannel=SequenceChannels.Initialization, MessageTypeId=1)]
    [ProtoContract]
    public class MessageTypeMessage : IMessage
    {
        public MessageTypeMessage() { }
        public MessageTypeMessage(IMessageTypeDescription desc)
        {
            this.MessageTypeName = desc.MessageType.FullName;
            this.MessageTypeId = desc.MessageTypeId;
        }
        [ProtoMember(1, IsRequired = true)]
        public string MessageTypeName;
        [ProtoMember(2, IsRequired = true)]
        public ushort MessageTypeId;
    }
}

