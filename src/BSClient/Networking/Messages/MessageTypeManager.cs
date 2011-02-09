using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;
using JollyBit.BS.Core.Networking;

namespace JollyBit.BS.Client.Networking.Messages
{
    public class MessageTypeManager : MessageTypeManagerBase
    {
        public MessageTypeManager(ILogger logger, IConnection connection)
            : base(false, logger)
        {
            connection.MessageReceived += new EventHandler<Core.Utility.EventArgs<object>>(connection_MessageReceived);
        }

        void connection_MessageReceived(object sender, Core.Utility.EventArgs<object> e)
        {
            if (e.Data is MessageTypeMessage)
            {
                MessageTypeMessage messageTypeMessage = e.Data as MessageTypeMessage;
                //Find the message
                MessageTypeDescription desc = _messageTypeToMessageTypeId.Values.FirstOrDefault(v => v.MessageType.FullName == messageTypeMessage.MessageTypeName);
                if(desc.MessageTypeId <= 100 )
                {
                    _logger.Warn("MessageTypeMessage attempted to register MessageType with MessageTypeId <= 100 which are reserved for static use. MessageTypeId='{0}' MessageTypeName='{1}'", messageTypeMessage.MessageTypeId, messageTypeMessage.MessageTypeName);
                }
                else if (desc != null)
                {
                    desc.MessageTypeId = messageTypeMessage.MessageTypeId;

                    if (_messageTypeIdToMessageType.ContainsKey(desc.MessageTypeId) && _messageTypeIdToMessageType[desc.MessageTypeId].MessageType != desc.MessageType)
                    {
                        _logger.Warn("MessageTypeMessage registers previously registered MessageTypeId. The new MessageType will replace the old MessageType OldMessageTypeId='{0}' OldMessageTypeName='{1}' NewMessageTypeId='{2}' NewMessageTypeName='{3}'", _messageTypeIdToMessageType[desc.MessageTypeId].MessageTypeId, _messageTypeIdToMessageType[desc.MessageTypeId].MessageType.FullName, desc.MessageTypeId, desc.MessageType.FullName);
                    }
                    _messageTypeIdToMessageType.Remove(desc.MessageTypeId);
                    _messageTypeIdToMessageType.Add(desc.MessageTypeId, desc);
                }
                else
                {
                    _logger.Warn("Unknown MessageTypeMessage received. MessageTypeId='{0}' MessageTypeName='{1}'", messageTypeMessage.MessageTypeId, messageTypeMessage.MessageTypeName);
                }
            }
        } 
    }
}
