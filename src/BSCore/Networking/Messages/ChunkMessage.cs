using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using ProtoBuf;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableUnordered)]
    [ProtoContract]
    public class ChunkMessage : IMessage
    {
        [ProtoMember(1, IsRequired = true, IsPacked = true)]
        public ushort[] Chunk;

        [ProtoMember(2, IsRequired = true)]
        public Point3L Location = new Point3L();
    }
}

