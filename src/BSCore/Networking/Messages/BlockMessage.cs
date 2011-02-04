using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using System.Runtime.Serialization;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableOrdered, SequenceChannel=SequenceChannels.Initialization)]
    [DataContract]
    public class BlockMessage : IMessage
    {
        [DataMember(Name="ID", Order=1, IsRequired=true)]
        public ushort ID;

        [DataMember(Name="Left", Order=2, IsRequired=false)]
        public string Left;

        [DataMember(Name="Right", Order=3, IsRequired=false)]
        public string Right;

        [DataMember(Name="Top", Order=4, IsRequired=false)]
        public string Top;

        [DataMember(Name="Bottom", Order=5, IsRequired=false)]
        public string Bottom;

        [DataMember(Name="Front", Order=6, IsRequired=false)]
        public string Front;

        [DataMember(Name="Back", Order=7, IsRequired=false)]
        public string Back;
    }
}