using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Ninject.Extensions.Logging;
using Ninject;
using System.IO;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking.Messages;

namespace JollyBit.BS.Core.Networking
{
    /// <summary>
    /// Interface that provides low level access to the network
    /// </summary>
    public interface INetworkPeer
    {
        event EventHandler<NetworkPeerConnectionEventArgs> ConnectionEstablished;
        event EventHandler<NetworkPeerConnectionEventArgs> ConnectionTerminated;
        event EventHandler<NetworkPeerConnectionEventArgs> ConnectionTerminating;
        event EventHandler<NetworkPeerConnectionEventArgs> MessageReceived;
        void SendMessage<T>(T message, object connection);
        void TerminateConnection(object connection);
        void Start();
        void Stop();
        void Connect(string host, int port);
        void Disconnect();
    }

    public class NetworkPeerConnectionEventArgs : EventArgs
    {
        public readonly object Connection;
        public readonly object Data;
        public NetworkPeerConnectionEventArgs(object connection, object data)
        {
            Connection = connection;
            Data = data;
        }
    }

    public class NetworkPeer : NetPeer, INetworkPeer
    {
        private readonly ILogger _logger;
        private IMessageTypeManager _messageTypeManager = null;
        private readonly ITimeService _timeService;
        private readonly IKernel _kernel;
        public NetworkPeer(ILoggerFactory loggerFactory, NetPeerConfiguration config, ITimeService timeService, IKernel kernel)
            : base(config)
        {
            _kernel = kernel;
            _logger = loggerFactory.GetLogger(typeof(NetworkPeer));
            _timeService = timeService;
            _timeService.Tick += new EventHandler<TimeTickEventArgs>(_timeService_Tick);
        }

        void _timeService_Tick(object sender, TimeTickEventArgs e)
        {
            CheckMessages();
        }

        public event EventHandler<NetworkPeerConnectionEventArgs> ConnectionEstablished;
        public event EventHandler<NetworkPeerConnectionEventArgs> ConnectionTerminated;
        public event EventHandler<NetworkPeerConnectionEventArgs> ConnectionTerminating;
        public event EventHandler<NetworkPeerConnectionEventArgs> MessageReceived;

        private void dataReceived(NetIncomingMessage message)
        {
            if(MessageReceived == null) return;
            ushort messageTypeId = message.ReadUInt16();
            IMessageTypeDescription mtd = _messageTypeManager.GetMessageTypeDescription(messageTypeId);
            if (mtd == null) //unknown message... log and drop
            {
                _logger.Warn("Received unknown message. The message will be ignored. TypeId='{0}' Length='{1}' Ip='{2}' Port='{3}'", messageTypeId, message.LengthBytes, message.SenderConnection.RemoteEndpoint.Address.ToString(), message.SenderConnection.RemoteEndpoint.Port);
                return;
            }
#if DEBUG
            _logger.Debug("Received message. Type='{0}' TypeId='{1}' Length='{2}' Ip='{3}' Port='{4}'", mtd.MessageType.Name, mtd.MessageTypeId, message.LengthBytes, message.SenderConnection.RemoteEndpoint.Address.ToString(), message.SenderConnection.RemoteEndpoint.Port);
#endif 
            object ret;
            using (MemoryStream stream = new MemoryStream(message.ReadBytes(message.LengthBytes - 2)))
            {
                ret = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, mtd.MessageType);
            }
            MessageReceived(this, new NetworkPeerConnectionEventArgs(message.SenderConnection, ret));
        }

        public void CheckMessages()
        {
            NetIncomingMessage message;
            while ((message = this.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        dataReceived(message);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                        switch (status)
	                    {
                            case NetConnectionStatus.Connected:
                                _logger.Info("Client connected. Ip='{0}' Port='{1}'", message.SenderConnection.RemoteEndpoint.Address.ToString(), message.SenderConnection.RemoteEndpoint.Port);
                                if (ConnectionEstablished != null) ConnectionEstablished(this, new NetworkPeerConnectionEventArgs(message.SenderConnection, null));
                                break;
                            case NetConnectionStatus.Disconnected:
                                _logger.Info("Client disconnected. Ip='{0}' Port='{1}'", message.SenderConnection.RemoteEndpoint.Address.ToString(), message.SenderConnection.RemoteEndpoint.Port);
                                if (ConnectionTerminated != null) ConnectionTerminated(this, new NetworkPeerConnectionEventArgs(message.SenderConnection, null));
                                break;
                            case NetConnectionStatus.Disconnecting:
                                _logger.Info("Client disconnecting. Ip='{0}' Port='{1}'", message.SenderConnection.RemoteEndpoint.Address.ToString(), message.SenderConnection.RemoteEndpoint.Port);
                                if (ConnectionTerminating != null) ConnectionTerminating(this, new NetworkPeerConnectionEventArgs(message.SenderConnection, null));
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                            case NetConnectionStatus.RespondedConnect:
                            case NetConnectionStatus.None:
                            default:
                                break;
	                    }
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
#if DEBUG
                        _logger.Debug("Lidgren debug message. Message='{0}'", message.ReadString());
#endif 
                        break;
                    case NetIncomingMessageType.WarningMessage:
                        _logger.Warn("Lidgren warning message. Message='{0}'", message.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.Error:
                        _logger.Error("Lidgren error message. Message='{0}'", message.ReadString());
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                    case NetIncomingMessageType.NatIntroductionSuccess:
                    case NetIncomingMessageType.DiscoveryResponse:
                    case NetIncomingMessageType.DiscoveryRequest:
                    case NetIncomingMessageType.Receipt:
                    case NetIncomingMessageType.UnconnectedData:
                    default:
                        throw new System.NotImplementedException();
                        break;
                }
                this.Recycle(message);
            }
        }

        public void SendMessage<T>(T message, object connection)
        {
            IMessageTypeDescription mtd = _messageTypeManager.GetMessageTypeDescription(typeof(T));
            NetOutgoingMessage outMessage = this.CreateMessage();
            outMessage.Write(mtd.MessageTypeId);
            using (MemoryStream stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, message);
                stream.Seek(0, SeekOrigin.Begin);
                outMessage.Write(stream.ToArray());
            }
#if DEBUG
            _logger.Debug("Sending message. Type='{0}' TypeId='{1}' Length='{2}' Ip='{3}' Port='{4}'", mtd.MessageType.Name, mtd.MessageTypeId, outMessage.LengthBytes, (connection as NetConnection).RemoteEndpoint.Address.ToString(), (connection as NetConnection).RemoteEndpoint.Port);
#endif 
            this.SendMessage(outMessage, connection as NetConnection, mtd.DeliveryMethod, mtd.SequenceChannel);
        }


        public void TerminateConnection(object connection)
        {
            (connection as NetConnection).Disconnect(null);
        }


        public void Stop()
        {
            base.Shutdown(null);
        }

        public void Start()
        {
            _messageTypeManager = _kernel.Get<IMessageTypeManager>();
            this.Configuration.AcceptIncomingConnections = true;
            base.Start();
            //while(true)
            //    CheckMessages();
        }

        public new void Connect(string host, int port)
        {
            this.Configuration.AcceptIncomingConnections = false;
            this.Configuration.Port = 0;
            this.Start();
            if (Connections.Count > 0)
            {
                throw new System.Exception("Already connected to a server!");
            }
            base.Connect(host, port, null);
        }

        public void Disconnect()
        {
            if (Connections.Count == 0)
            {
                throw new System.Exception("Can not disconnect! Client is not connected.");
            }
            Connections[0].Disconnect(null);
        }
    }
}
