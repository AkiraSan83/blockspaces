using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking.Messages;
using Ninject;
using JollyBit.BS.Core.Networking;
using Lidgren.Network;
using System.IO;
using JollyBit.BS.Core;

namespace JollyBit.BS.Server.Networking
{
    public interface IConnectionManager
    {
        /// <summary>
        /// This function sends a via the specified connection.
        /// </summary>
        /// <typeparam name="T">The type of message to send</typeparam>
        /// <param name="connection">The connection to use in order to send the event</param>
        /// <param name="message">The message to send</param>
        void SendMessage<T>(IConnection connection, T message);
        /// <summary>
        /// This event is fired whenever any connection receives a message
        /// </summary>
        event EventHandler<EventArgs<KeyValuePair<IConnection, object>>> MessageReceived;
        /// <summary>
        /// This event returns a list of current connections.
        /// </summary>
        IEnumerable<IConnection> Connections { get; }
        /// <summary>
        /// This event fires whenever a connection is terminated.
        /// </summary>
        event EventHandler<EventArgs<IConnection>> ConnectionTerminated;
        /// <summary>
        /// This event fires whenever a connection is established. This event is fired before the connection
        /// is considered initialized. This event should be used to send client initialization data.
        /// </summary>
        event EventHandler<EventArgs<IConnection>> ConnectionEstablished;
        /// <summary>
        /// This event fires when the connection is initialized. This occurs after the  
        /// ConnectionEstablished event fires.
        /// </summary>
        event EventHandler<EventArgs<IConnection>> ConnectionInitialized;
        void StartListeningForConnections();
        void StopListeningForConnections();
    }

    internal class ConnectionManager : IConnectionManager
    {
        private readonly IDictionary<object, Connection> _connectionDict = new Dictionary<object, Connection>();
        private readonly INetworkPeer _network;

        public ConnectionManager(INetworkPeer network)
        {
            _network = network;
            _network.ConnectionEstablished += new EventHandler<NetworkPeerConnectionEventArgs>(_network_ConnectionEstablished);
            _network.MessageReceived += new EventHandler<NetworkPeerConnectionEventArgs>(_network_DataReceived);
            _network.ConnectionTerminated += new EventHandler<NetworkPeerConnectionEventArgs>(_network_ConnectionTerminated);
            this.MessageReceived += new EventHandler<EventArgs<KeyValuePair<IConnection, object>>>(ConnectionManager_MessageReceived);
        }

        void ConnectionManager_MessageReceived(object sender, EventArgs<KeyValuePair<IConnection, object>> e)
        {
            //Raise Initialization Complete Message
            if (e.Data.Value is InitializationCompleteMessage)
            {
                Connection conn;
                if ((conn = e.Data.Key as Connection) != null) conn.RaiseConnectionInitialized();
                if (ConnectionInitialized != null) ConnectionInitialized(this, new EventArgs<IConnection>(e.Data.Key));
            }
        }

        #region Network Events
        void _network_ConnectionTerminated(object sender, NetworkPeerConnectionEventArgs e)
        {
            Connection conn = _connectionDict[e.Connection];
            Debug.AssertNotNull(conn, "conn should not be null");
            _connectionDict.Remove(conn);
            conn.RaiseConnectionTerminated();
            if (ConnectionTerminated != null) ConnectionTerminated(this, new EventArgs<IConnection>(conn));
        }

        void _network_DataReceived(object sender, NetworkPeerConnectionEventArgs e)
        {
            Connection conn = _connectionDict[e.Connection];
            Debug.AssertNotNull(conn, "conn should not be null");
            Debug.AssertNotNull(e.Data, "e.Data should not be null");
            conn.RaiseMessageReceived(new EventArgs<object>(e.Data));
            if (MessageReceived != null) MessageReceived(this, new EventArgs<KeyValuePair<IConnection, object>>(new KeyValuePair<IConnection, object>(conn, e.Data)));
        }

        void _network_ConnectionEstablished(object sender, NetworkPeerConnectionEventArgs e)
        {
            Connection connection = new Connection(this, e.Connection);
            _connectionDict.Add(e.Connection, connection);
            connection.RaiseConnectionEstablished();
            if (ConnectionEstablished != null) ConnectionEstablished(this, new EventArgs<IConnection>(connection));
            connection.SendMessage(new InitializationCompleteMessage());
        }
        
        #endregion

        #region IConnectionManager
        public void SendMessage<T>(IConnection connection, T message)
        {
            Connection conn = connection as Connection;
            if (conn != null)
            {
                _network.SendMessage(message, conn.NetPeerConnection);
            }
            else conn.SendMessage(message);
        }
        public IEnumerable<IConnection> Connections
        {
            get { return _connectionDict.Values.Cast<IConnection>(); }
        }
        public event EventHandler<EventArgs<KeyValuePair<IConnection, object>>> MessageReceived;
        public event EventHandler<EventArgs<IConnection>> ConnectionTerminated;
        public event EventHandler<EventArgs<IConnection>> ConnectionEstablished;
        public event EventHandler<EventArgs<IConnection>> ConnectionInitialized;
        #endregion

        /// <summary>
        /// A dumb connection which simply relies on ConnectionManager for all functionality
        /// </summary>
        private class Connection : IConnection
        {
            private readonly IConnectionManager _connectionManager;
            public readonly object NetPeerConnection;
            public Connection(ConnectionManager connectionManager, object netPeerConnection)
            {
                _connectionManager = connectionManager;
                NetPeerConnection = netPeerConnection;
            }
            public void SendMessage<T>(T message)
            {
                _connectionManager.SendMessage(this, message);
            }
            public void RaiseMessageReceived(EventArgs<object> args)
            {
                if (MessageReceived != null) MessageReceived(this, args);
            }
            public event EventHandler<EventArgs<object>> MessageReceived;
            public void RaiseConnectionEstablished()
            {
                if (ConnectionEstablished != null) ConnectionEstablished(this, new EventArgs());
            }
            public event EventHandler ConnectionEstablished;
            public void RaiseConnectionTerminated()
            {
                if (ConnectionEstablished != null) ConnectionTerminated(this, new EventArgs());
            }
            public event EventHandler ConnectionTerminated;
            public void RaiseConnectionInitialized()
            {
                if (ConnectionInitialized != null) ConnectionInitialized(this, new EventArgs());
            }
            public event EventHandler ConnectionInitialized;

            private IClient _client = null;
            public IClient Client
            {
                get { return _client; }
                set { _client = value; }
            }
        }


        public void StartListeningForConnections()
        {
            _network.Start();
        }

        public void StopListeningForConnections()
        {
            _network.Stop();
        }
    }
}