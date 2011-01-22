using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using Lidgren.Network;

namespace JollyBit.BS.Core.Networking
{
    public class NetworkConfig : IConfigSection
    {
        public int Port = 16061;

        public NetPeerConfiguration CreateNetPeerConfiguration()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("BlockSpaces");
            
            return config;
        }
    }
}
