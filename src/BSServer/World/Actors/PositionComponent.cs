using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.World.Actors;
using OpenTK;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Server.Networking;

namespace JollyBit.BS.Server.World.Actors
{
    public class PositionComponent : PositionableComponentBase
    {
        private readonly IServerSyncComponent _syncComponent;
        public PositionComponent(IActor actor, IServerSyncComponent syncComponent)
            : base(actor)
        {
            _syncComponent = syncComponent;
            _syncComponent.ConnectionEnteringRelevantSet += new EventHandler<Core.Utility.EventArgs<Core.Networking.IConnection>>(_syncComponent_ConnectionEnteringRelevantSet);
        }
        void _syncComponent_ConnectionEnteringRelevantSet(object sender, Core.Utility.EventArgs<Core.Networking.IConnection> e)
        {
            e.Data.SendMessage(new PositionMessage(Actor.ActorId, Position, Rotation));
        }
        protected override void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
        {
            PositionMessage msg = new PositionMessage(Actor.ActorId, Position, Rotation);
            foreach (var conn in _syncComponent.RelevantConnections)
            {
                conn.SendMessage(msg);
            }
        }
        protected override void OnRotationChanged(Quaternion oldRotation, Quaternion newRotation)
        {
            PositionMessage msg = new PositionMessage(Actor.ActorId, Position, Rotation);
            foreach (var conn in _syncComponent.RelevantConnections)
            {
                conn.SendMessage(msg);
            }
        }
    }
}
