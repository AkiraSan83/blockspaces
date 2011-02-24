using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using OpenTK;

namespace JollyBit.BS.Core.Networking.Messages
{
    [Message(Lidgren.Network.NetDeliveryMethod.ReliableSequenced, SequenceChannel = SequenceChannels.Position)]
    public class PositionMessage : ActorMessage
    {
        public PositionMessage() { }
        public PositionMessage(int actorId, Vector3 position, Quaternion rotation)
        {
            ActorId = actorId;
            Position = position;
            Rotation = rotation;
        }
        [ProtoMember(2, IsRequired = false)]
        public Vector3 Position;
        [ProtoMember(3, IsRequired = false)]
        public Quaternion Rotation;
    }
}
