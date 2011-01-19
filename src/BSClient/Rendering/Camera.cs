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

		private Quaternion _rot = Quaternion.Identity;
		
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
			if(angle != 0f) {
				_rot = _rot * Quaternion.FromAxisAngle(Vector3.UnitX,angle);
				_rot.Normalize();
				//Console.WriteLine(rot);
				
	            _z = Vector3.Transform(Vector3.UnitZ,_rot);
	            _y = Vector3.Transform(Vector3.UnitY,_rot);
				//GrahmSchmidt();
			}
        }

        public void RotateY(float angle)
        {
			if(angle != 0f) {
	            _rot = Quaternion.FromAxisAngle(Vector3.UnitY,angle) * _rot;
				_rot.Normalize();
				
	            _z = Vector3.Transform(Vector3.UnitZ,_rot);
	            _x = Vector3.Transform(Vector3.UnitX,_rot);
				GrahmSchmidt();
			}
        }

        public void RotateZ(float angle)
        {
			if(angle != 0f) {
	            Quaternion rot = Quaternion.FromAxisAngle(_z,angle);
				
	            _y = Vector3.Transform(_y, rot);
	            _x = Vector3.Transform(_x, rot);
				GrahmSchmidt();
			}
        }
		
		private void GrahmSchmidt() {
			_y = _y - projection(_y,_x);
			_z = _z - projection(_z,_x);
			_z = _z - projection(_z,_y);
			_x.Normalize();
			_y.Normalize();
			_z.Normalize();
		}
		
		private Vector3 projection(Vector3 x, Vector3 y) {
			return (Vector3.Dot(x,y) / y.LengthSquared) * y;
		}

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        

    }
}
