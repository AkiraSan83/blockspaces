using System;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using ProtoBuf;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableOrdered, SequenceChannel = SequenceChannels.Initialization)]
    [ProtoContract]
    public class InitializationCompleteMessage
    {
    }
}
