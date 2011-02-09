using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;
using System.Reflection;
using Lidgren.Network;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Server.Networking.Messages
{
    public class MessageTypeManager : MessageTypeManagerBase
    {
        public MessageTypeManager(ILogger logger, IConnectionManager connectionManager)
            : base(true, logger)
        {
            connectionManager.ConnectionEstablished += new EventHandler<EventArgs<Core.Networking.IConnection>>(connectionManager_ConnectionEstablished);
        }

        void connectionManager_ConnectionEstablished(object sender, EventArgs<Core.Networking.IConnection> e)
        {
            foreach (var msgTypeDesc in _messageTypeIdToMessageType.Values.Where(v => v.MessageTypeId > 100)) //Dont send messages with static ids
            {
                MessageTypeMessage msg = new MessageTypeMessage(msgTypeDesc);
                e.Data.SendMessage(msg);
            }
        }

    }
}
