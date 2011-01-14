using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using JollyBit.BS.World;

namespace JollyBit.BS.Rendering
{
    public class Camera : IPositionable
    {

        private Vector3 _position = new Vector3(0,0,0);

        private Vector3 _x = Vector3.UnitX;
        private Vector3 _y = Vector3.UnitY;
        private Vector3 _z = Vector3.UnitZ;

        public void Render()
        {
            GL.MatrixMode(MatrixMode.Modelview);
			
            Matrix4 rot = new Matrix4(new Vector4(_x.X, _y.X, _z.X, 0.0f),
                                        new Vector4(_x.Y, _y.Y, _z.Y, 0.0f),
                                        new Vector4(_x.Z, _y.Z, _z.Z, 0.0f),
                                        Vector4.UnitW);

            Matrix4 trans = Matrix4.CreateTranslation(-_position);
			
			//System.Console.WriteLine("x,y,z = {0} {1} {2}\nrot = {3}\ntrans = {4}\n",_x,_y,_z,rot,trans);
			
            Matrix4 m = trans * rot;
            GL.LoadMatrix(ref m);
        }

        //public void LookAt(ref Vector3 target)
        //{
        //    _locX = target;
        //    Vector3.Normalize(ref _locX, out _locX);
        //}

        public void MoveForward(float distance)
        {
            _position += _z * -distance;
        }

        public void StrafeRight(float distance)
        {
            _position += _x * distance;
        }

        public void MoveUpward(float distance)
        {
            _position += _y * distance;
        }

        public void RotateX(float angle)
        {
            Matrix4 rot = Matrix4.CreateRotationX(angle);
            _z = Vector3.Transform(_z, rot);
            _z.Normalize();
            Vector3.Cross(ref _x, ref _z, out _y);
            _y.Normalize();
        }

        public void RotateY(float angle)
        {
            Matrix4 rot = Matrix4.CreateRotationY(angle);
            _z = Vector3.Transform(_z, rot);
            _z.Normalize();
            Vector3.Cross(ref _z, ref _y, out _x);
            _x.Normalize();
        }

        public void RotateZ(float angle)
        {
            Matrix4 rot = Matrix4.CreateRotationZ(angle);
            _y = Vector3.Transform(_y, rot);
            _y.Normalize();
            Vector3.Cross(ref _z, ref _y, out _x);
            _x.Normalize();
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        

    }
}
