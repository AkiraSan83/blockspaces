using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using OpenTK;

namespace JollyBit.BS.Core.Networking.Messages
{
    [ProtoContract]
    public abstract class ActorMessage
    {
        /// <summary>
        /// The ActorId of the actor who is responsible for handling the message
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public int ActorId;
    }
}
