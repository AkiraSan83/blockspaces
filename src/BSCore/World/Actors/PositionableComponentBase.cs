using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Core.Networking;

namespace JollyBit.BS.Core.World.Actors
{
    public abstract class PositionableComponentBase : IPositionableComponent
    {
        public IActor Actor { get; private set; }
        public PositionableComponentBase(IActor actor)
        {
            Actor = actor;
        }
        //Position
        private Vector3 _position = new Vector3();
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    Vector3 old = _position;
                    _position = value;
                    OnPositionChanged(old, _position);
                    if (PositionChanged != null) PositionChanged(this, new ChangedEventArgs<Vector3>(old, _position));
                }
            }
        }
        public event EventHandler<ChangedEventArgs<Vector3>> PositionChanged;
        protected abstract void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition);
        //Rotation
        private Quaternion _rotation = new Quaternion();
        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation != value)
                {
                    Quaternion old = _rotation;
                    _rotation = value;
                    OnRotationChanged(old, _rotation);
                    if (RotationChanged != null) RotationChanged(this, new ChangedEventArgs<Quaternion>(old, _rotation));
                }
            }
        }
        public event EventHandler<ChangedEventArgs<Quaternion>> RotationChanged;
        protected abstract void OnRotationChanged(Quaternion oldRotation, Quaternion newRotation);
    }
}
