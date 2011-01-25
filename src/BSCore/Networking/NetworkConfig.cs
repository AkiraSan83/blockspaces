using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using Lidgren.Network;
using JsonExSerializer;

namespace JollyBit.BS.Core.Networking
{
    public class NetworkConfig : IConfigSection
    {
        private NetPeerConfiguration _config;
        public NetworkConfig()
        {
            _config = new NetPeerConfiguration("BlockSpaces");
            _config.Port = 12421;
        }
        public int Port
        {
            get { return _config.Port; }
            set { _config.Port = value; }
        }
        public int MaximumConnections
        {
            get { return _config.MaximumConnections; }
            set { _config.MaximumConnections = value; }
        }
        public NetPeerConfiguration CreateNetPeerConfig()
        {
            return _config;
        }
    }
}
