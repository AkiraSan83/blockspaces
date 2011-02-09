using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using ProtoBuf;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableOrdered, SequenceChannel=SequenceChannels.Initialization, MessageTypeId=2)]
    [ProtoContract]
    public class UnknownMessageTypeMessage : IMessage
    {
        public UnknownMessageTypeMessage() { }
        public UnknownMessageTypeMessage(ushort messageTypeId)
        {
            this.MessageTypeId = messageTypeId;
        }
        [ProtoMember(2, IsRequired = true)]
        public ushort MessageTypeId;
    }
}

