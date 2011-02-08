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
        [DataMember(Order=1, IsRequired=true)]
        public ushort[] Chunk;

        //[DataMember(Order = 2, IsRequired = true)]
        public Point3L Location = new Point3L();

        [DataMember(Order = 2, IsRequired = true)]
        public long LocationX
        {
            get { return Location.X; }
            set { Location.X = value; }
        }

        [DataMember(Order = 3, IsRequired = true)]
        public long LocationY
        {
            get { return Location.Y; }
            set { Location.Y = value; }
        }

        [DataMember(Order = 4, IsRequired = true)]
        public long LocationZ
        {
            get { return Location.Z; }
            set { Location.Z = value; }
        }
    }
}

