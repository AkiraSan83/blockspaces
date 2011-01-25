using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.Networking.Messages
{
    /// <summary>
    /// This attribute is used to indicate that a class will be serialized and sent as a message
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    sealed class MessageAttribute : Attribute
    {
        public readonly NetDeliveryMethod DeliveryMethod;
        private ushort? _messageTypeId = null;

        /// <summary>
        /// The id used to identify the message type. If this parameter is null
        /// the MessageType will automatically be assigned a MessageTypeId wich is usally what is desired.
        /// MessageTypeId values between 1 and 100 are valid. An error will be thrown if two message types have the same
        /// MessageTypeId.
        /// </summary
        public ushort? MessageTypeId
        {
            get { return _messageTypeId; }
            set 
            {
                Debug.AssertTrue(value != 1 && value < 101, "MessageTypeId must be between 1 and 100");
                _messageTypeId = value; 
            }
        } 

        /// <summary>
        /// This attribute is used to indicate that a class will be serialized and sent as a message 
        /// </summary>
        /// <param name="deliveryMethod">The DeliveryMethod used to send the message.</param>
        public MessageAttribute(NetDeliveryMethod deliveryMethod)
        {
            DeliveryMethod = deliveryMethod;
        }
    }
}
