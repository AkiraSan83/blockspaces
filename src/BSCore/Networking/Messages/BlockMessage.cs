using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using ProtoBuf;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableOrdered, SequenceChannel=SequenceChannels.Initialization)]
    [ProtoContract]
    public class BlockMessage : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public ushort ID;

        [ProtoMember(2, IsRequired = false)]
        public string Left;

        [ProtoMember(3, IsRequired = false)]
        public string Right;

        [ProtoMember(4, IsRequired = false)]
        public string Top;

        [ProtoMember(5, IsRequired = false)]
        public string Bottom;

        [ProtoMember(6, IsRequired = false)]
        public string Front;

        [ProtoMember(7, IsRequired = false)]
        public string Back;
    }
}