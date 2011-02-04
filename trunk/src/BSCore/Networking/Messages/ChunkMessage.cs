using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using System.Runtime.Serialization;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableOrdered, SequenceChannel = SequenceChannels.Initialization)]
    [DataContract]
    public class ChunkMessage : IMessage
    {
        [DataMember(Name="Chunk", Order=1, IsRequired=true)]
        public ushort[] Chunk;

        public Point3L Location = new Point3L();

        [DataMember(Name = "XLocation", Order = 2, IsRequired = true)]
        public long XLocation
        {
            get { return Location.X; }
            set { Location.X = value; }
        }
        [DataMember(Name = "YLocation", Order = 3, IsRequired = true)]
        public long YLocation
        {
            get { return Location.Y; }
            set { Location.Y = value; }
        }
        [DataMember(Name = "ZLocation", Order = 4, IsRequired = true)]
        public long ZLocation
        {
            get { return Location.Z; }
            set { Location.Z = value; }
        }
    }
}

