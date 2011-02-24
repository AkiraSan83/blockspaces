using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.World.Actors
{
    /// <summary>
    /// Gives parent actor a position in the game world.
    /// </summary>
    public interface IPositionableComponent : IActorComponent
    {
        /// <summary>
        /// The position of the parent actor in the game world.
        /// </summary>
        Vector3 Position { get; set; }
        /// <summary>
        /// The rotation of the parent actor in the game world.
        /// </summary>
        Quaternion Rotation { get; set; }
        /// <summary>
        /// This event is raised whenever the position of the parent actor changes.
        /// </summary>
        event EventHandler<ChangedEventArgs<Vector3>> PositionChanged;
        /// <summary>
        /// This event is raised whenever the rotation of the parent actor changes.
        /// </summary>
        event EventHandler<ChangedEventArgs<Quaternion>> RotationChanged;
    }
}
