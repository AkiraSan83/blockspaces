using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Core.World.Actors;

namespace JollyBit.BS.Server.World.Actors
{
    public interface IServerSyncComponent : IActorComponent
    {
        event EventHandler<EventArgs<IConnection>> ConnectionEnteringRelevantSet;
        event EventHandler<EventArgs<IConnection>> ConnectionExitingRelevantSet;
        IEnumerable<IConnection> RelevantConnections { get; }
    }
}
