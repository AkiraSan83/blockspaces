using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.World.Actors;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking.Messages;

namespace JollyBit.BS.Server.World.Actors
{
    public class SimpleSyncComponent : IServerSyncComponent
    {
        IConnectionManager _connectionManager;
        IActor _actor;
        public SimpleSyncComponent(IActor actor, IConnectionManager connectionManager)
        {
            _actor = actor;
            _connectionManager = connectionManager;
            _connectionManager.ConnectionInitialized += new EventHandler<EventArgs<IConnection>>(_connectionManager_ConnectionInitialized);
            _connectionManager.ConnectionTerminated += new EventHandler<EventArgs<IConnection>>(_connectionManager_ConnectionTerminated);
        }

        void _connectionManager_ConnectionTerminated(object sender, EventArgs<IConnection> e)
        {
            if (ConnectionExitingRelevantSet != null)
                ConnectionExitingRelevantSet(this, new EventArgs<IConnection>(e.Data));
        }

        void _connectionManager_ConnectionInitialized(object sender, EventArgs<IConnection> e)
        {
            if (ConnectionEnteringRelevantSet != null)
                ConnectionEnteringRelevantSet(this, new EventArgs<IConnection>(e.Data));
        }
        
        public event EventHandler<EventArgs<IConnection>> ConnectionEnteringRelevantSet;

        public event EventHandler<EventArgs<IConnection>> ConnectionExitingRelevantSet;

        public IEnumerable<IConnection> RelevantConnections
        {
            get { return _connectionManager.Connections; }
        }
        
        public IActor Actor { get { return _actor; } }
    }
}