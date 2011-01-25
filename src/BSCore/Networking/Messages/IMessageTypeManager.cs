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
    public interface IMessageTypeManager
    {
        IMessageTypeDescription GetMessageTypeDescription(uint messageTypeId);
        IMessageTypeDescription GetMessageTypeDescription(Type messageType);
    }
    internal class MessageTypeDescription : IMessageTypeDescription
    {
        private readonly ushort _messageTypeId;
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
    internal class MessageTypeManager : IMessageTypeManager
    {
        private readonly ILogger _logger;
        public MessageTypeManager(ILogger logger)
        {
            _logger = logger;
            reflectAssemblyForMessageTypes();
        }
        private void reflectAssemblyForMessageTypes()
        {
            ushort currentMessageTypeId = 101;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                {
                    MessageAttribute messageAttr = type.GetCustomAttributes(typeof(MessageAttribute), false).FirstOrDefault() as MessageAttribute;
                    if (messageAttr != null)
                    {
                        MessageTypeDescription messageTypeDesc;
                        if (messageAttr.MessageTypeId != null)
                        {
                            Debug.AssertFalse(_messageTypeIdToMessageType.ContainsKey((ushort)messageAttr.MessageTypeId), "Two message types cannot have the same MessageTypeId!");
                            messageTypeDesc = new MessageTypeDescription((ushort)messageAttr.MessageTypeId, type, 0, messageAttr.DeliveryMethod);
                        }
                        else
                        {
                            messageTypeDesc = new MessageTypeDescription(currentMessageTypeId, type, 0, messageAttr.DeliveryMethod);
                            currentMessageTypeId++;
                        }
                        _messageTypeIdToMessageType.Add(messageTypeDesc.MessageTypeId, messageTypeDesc);
                        _messageTypeToMessageTypeId.Add(messageTypeDesc.MessageType, messageTypeDesc);
                    }
                }
        }
        private IDictionary<uint, IMessageTypeDescription> _messageTypeIdToMessageType = new Dictionary<uint, IMessageTypeDescription>();
        private IDictionary<Type, IMessageTypeDescription> _messageTypeToMessageTypeId = new Dictionary<Type, IMessageTypeDescription>();
        public IMessageTypeDescription GetMessageTypeDescription(uint messageTypeId)
        {
            IMessageTypeDescription messageTypeDesc;
            if (_messageTypeIdToMessageType.TryGetValue(messageTypeId, out messageTypeDesc))
            {
                return messageTypeDesc;
            }
            return null;
        }
        public IMessageTypeDescription GetMessageTypeDescription(Type messageType)
        {
            IMessageTypeDescription messageTypeDesc;
            if (_messageTypeToMessageTypeId.TryGetValue(messageType, out messageTypeDesc))
            {
                return messageTypeDesc;
            }
            return null;
        }
    }
}
