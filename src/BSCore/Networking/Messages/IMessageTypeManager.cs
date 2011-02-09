using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;
using System.Reflection;
using Lidgren.Network;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.Networking.Messages
{
    public interface IMessageTypeDescription
    {
        Type MessageType { get; }
        ushort MessageTypeId { get; }
        int SequenceChannel { get; }
        NetDeliveryMethod DeliveryMethod { get; }
    }
    public interface IMessageTypeManager : IService
    {
        IMessageTypeDescription GetMessageTypeDescription(uint messageTypeId);
        IMessageTypeDescription GetMessageTypeDescription(Type messageType);
    }
    public class MessageTypeDescription : IMessageTypeDescription
    {
        private ushort _messageTypeId;
        private readonly Type _messageType;
        private readonly int _sequenceChannel;
        private NetDeliveryMethod _deliveryMethod;
        public MessageTypeDescription(ushort messageTypeId, Type messageType, int sequenceChannel, NetDeliveryMethod deliveryMethod)
        {
            _messageTypeId = messageTypeId;
            _messageType = messageType;
            _sequenceChannel = sequenceChannel;
            _deliveryMethod = deliveryMethod;
        }
        public Type MessageType
        {
            get { return _messageType; }
        }
        public ushort MessageTypeId
        {
            get { return _messageTypeId; }
            set { _messageTypeId = value; }
        }
        public int SequenceChannel
        {
            get { return _sequenceChannel; }
        }
        public NetDeliveryMethod DeliveryMethod
        {
            get { return _deliveryMethod; }
        }
    }
}
