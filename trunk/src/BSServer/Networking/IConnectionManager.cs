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
        /// A list of operations to preform before the connection is considered established. If
        /// any of the operations return false the connection will be terminated.
        /// </summary>
        IList<IConnectionEstablishingOperation> ConnectionEstablishingOperations { get; }
        /// <summary>
        /// This event fires whenever a connection is terminated.
        /// </summary>
        event EventHandler<EventArgs<IConnection>> ConnectionTerminated;
        /// <summary>
        /// This event fires whenever a connection is established. ConnectionEstablishingOperations are preformed before
        /// this event is fired
        /// </summary>
        event EventHandler<EventArgs<IConnection>> ConnectionEstablished;

        void StartListeningForConnections();
        void StopListeningForConnections();
    }

    /// <summary>
    /// An operation that is to be preformed before a connection is considered established.
    /// </summary>
    /// <typeparam name="TCLIENT"></typeparam>
    public interface IConnectionEstablishingOperation
    {
        /// <summary>
        /// Does an operation for a specific connection. This method should return quickly and not block the
        /// calling thread.
        /// </summary>
        /// <param name="connection">The connection the operation is to be preformed on/for</param>
        /// <param name="callbackFunc">This function should be called when the operation is complete. If the function's only
        /// parameter is false the connection will be terminated. The callback function should be called in the same thread
        /// as DoConnectionEstablishingOperation was called from.</param>
        void DoConnectionEstablishingOperation(IConnection connection, Action<bool> callbackFunc);
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
        }

        #region Network Events
        void _network_ConnectionTerminated(object sender, NetworkPeerConnectionEventArgs e)
        {
            Connection conn = _connectionDict[e.Connection];
            Debug.AssertNotNull(conn, "conn should not be null");
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
            int currentOperationIndex = -1;
            Action<bool> callback = null;
            callback = (bool success) =>
            {
                currentOperationIndex++;
                if (!success)
                {
                    //Operation failed disconnect
                    _network.TerminateConnection(connection.NetPeerConnection);
                    return;
                }
                if (_connectionEstablishingOperations.Count == currentOperationIndex)
                {
                    //End of operations make connection official
                    connection.RaiseConnectionEstablished();
                    if (ConnectionEstablished != null) ConnectionEstablished(this, new EventArgs<IConnection>(connection));
                    connection.SendMessage(new InitializationCompleteMessage());
                    return;
                }
                //Do next operation in list
                IConnectionEstablishingOperation currentOperation = _connectionEstablishingOperations[currentOperationIndex];
                currentOperation.DoConnectionEstablishingOperation(connection, callback);
            };
            callback(true);
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
            get { return _connectionDict.Cast<IConnection>(); }
        }
        private readonly IList<IConnectionEstablishingOperation> _connectionEstablishingOperations = new List<IConnectionEstablishingOperation>();
        public IList<IConnectionEstablishingOperation> ConnectionEstablishingOperations
        {
            get { return _connectionEstablishingOperations; }
        }
        public event EventHandler<EventArgs<KeyValuePair<IConnection, object>>> MessageReceived;
        public event EventHandler<EventArgs<IConnection>> ConnectionTerminated;
        public event EventHandler<EventArgs<IConnection>> ConnectionEstablished;
        #endregion

        /// <summary>
        /// A dumb connection wich simply relies on ConnectionManager for all functionality
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