using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;
using System.Reflection;
using Lidgren.Network;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.Networking.Messages
{
    public abstract class MessageTypeManagerBase : IMessageTypeManager
    {
        protected readonly ILogger _logger;
        public MessageTypeManagerBase(bool assignMessageTypesIds, ILogger logger)
        {
            _logger = logger;
            _logger.Info("MessageTypeManager started.");
            reflectAssemblyForMessageTypes(assignMessageTypesIds);
        }

        private void reflectAssemblyForMessageTypes(bool assignMessageTypesIds)
        {
            ushort currentMessageTypeId = 101;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                {
                    MessageAttribute messageAttr = type.GetCustomAttributes(typeof(MessageAttribute), false).FirstOrDefault() as MessageAttribute;
                    if (messageAttr != null)
                    {
                        MessageTypeDescription messageTypeDesc;
                        if (messageAttr.MessageTypeId != 0)
                        {
                            Debug.AssertFalse(_messageTypeIdToMessageType.ContainsKey((ushort)messageAttr.MessageTypeId), "Two message types cannot have the same MessageTypeId!");
                            _logger.Debug("Message type discovered. Type='{0}' TypeId='{1}'", type.FullName, (ushort)messageAttr.MessageTypeId);
                            messageTypeDesc = new MessageTypeDescription((ushort)messageAttr.MessageTypeId, type, (int)messageAttr.SequenceChannel, messageAttr.DeliveryMethod);
                        }
                        else
                        {
                            _logger.Debug("Message type discovered. Type='{0}' TypeId='{1}'", type.FullName, currentMessageTypeId);
                            messageTypeDesc = new MessageTypeDescription(currentMessageTypeId, type, (int)messageAttr.SequenceChannel, messageAttr.DeliveryMethod);
                            currentMessageTypeId++;
                        }
                        if (assignMessageTypesIds || messageAttr.MessageTypeId != 0)
                            _messageTypeIdToMessageType.Add(messageTypeDesc.MessageTypeId, messageTypeDesc);
                        _messageTypeToMessageTypeId.Add(messageTypeDesc.MessageType, messageTypeDesc);
                    }
                }
        }

        protected IDictionary<uint, MessageTypeDescription> _messageTypeIdToMessageType = new Dictionary<uint, MessageTypeDescription>();
        protected IDictionary<Type, MessageTypeDescription> _messageTypeToMessageTypeId = new Dictionary<Type, MessageTypeDescription>();
        public IMessageTypeDescription GetMessageTypeDescription(uint messageTypeId)
        {
            MessageTypeDescription messageTypeDesc;
            if (_messageTypeIdToMessageType.TryGetValue(messageTypeId, out messageTypeDesc))
            {
                return messageTypeDesc;
            }
            return null;
        }
        public IMessageTypeDescription GetMessageTypeDescription(Type messageType)
        {
            MessageTypeDescription messageTypeDesc;
            if (_messageTypeToMessageTypeId.TryGetValue(messageType, out messageTypeDesc))
            {
                return messageTypeDesc;
            }
            return null;
        }
    }
}
