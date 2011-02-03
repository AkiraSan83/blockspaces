using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Networking;
using Ninject;

namespace JollyBit.BS.Client.Networking
{
    public class Connection<TCLIENT> : IClientConnection<TCLIENT>
    {
        INetworkPeer _network;
        object _networkPeerConnection;
        [Inject]
        public Connection(INetworkPeer network)
        {
            _network = network;
            _network.ConnectionEstablished += new EventHandler<NetworkPeerConnectionEventArgs>(_network_ConnectionEstablished);
            _network.ConnectionTerminated += new EventHandler<NetworkPeerConnectionEventArgs>(_network_ConnectionTerminated);
            _network.MessageReceived += new EventHandler<NetworkPeerConnectionEventArgs>(_network_MessageReceived);
        }

        void _network_MessageReceived(object sender, NetworkPeerConnectionEventArgs e)
        {
            if (MessageReceived != null) MessageReceived(this, new Core.Utility.EventArgs<object>(e.Data));
        }

        void _network_ConnectionTerminated(object sender, NetworkPeerConnectionEventArgs e)
        {
            _networkPeerConnection = null;
            if (ConnectionTerminated != null) ConnectionTerminated(this, new EventArgs());
        }

        void _network_ConnectionEstablished(object sender, NetworkPeerConnectionEventArgs e)
        {
            _networkPeerConnection = e.Connection;
            Console.WriteLine("Connected");
            if (ConnectionEstablished != null) ConnectionEstablished(this, new EventArgs());
        }

        public void SendMessage<T>(T message)
        {
            _network.SendMessage(message, _networkPeerConnection);
        }

        public event EventHandler<Core.Utility.EventArgs<object>> MessageReceived;

        public event EventHandler ConnectionEstablished;

        public event EventHandler ConnectionTerminated;

        private TCLIENT _client = default(TCLIENT);
        public TCLIENT Client
        {
            get { return _client; }
            set { _client = value; }
        }

        public void Connect(string host, int port)
        {
            //_network.Start(0);
            _network.Connect(host, port);
        }

        public void Disconnect()
        {
            _network.Disconnect();
            //_network.Stop();
        }
    }
}
