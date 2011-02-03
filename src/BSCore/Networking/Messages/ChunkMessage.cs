using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using System.Runtime.Serialization;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableUnordered)]
    [DataContract]
    public class ChunkMessage : IMessage
    {
        [DataMember(Name="Chunk", Order=1, IsRequired=true)]
        public ushort[,,] Chunk;

        [DataMember(Name="Location", Order=2, IsRequired=true)]
        public Point3L Location;
    }
}
