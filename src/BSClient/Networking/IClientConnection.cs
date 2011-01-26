using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Networking;

namespace JollyBit.BS.Client.Networking
{
    public interface IClientConnection<TCLIENT> : IConnection<TCLIENT>
    {
        void Connect(string host, int port);
        void Disconnect();
    }
}
