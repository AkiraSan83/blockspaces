using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using JollyBit.BS.Core.World;

using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core;
using Ninject;

namespace JollyBit.BS.Client.Rendering
{
    public class Camera : IPositionable
    {
        private Vector3 _position = new Vector3(0,0,0);

		private Quaternion _rot = Quaternion.Identity;
		
        private Vector3 _x = Vector3.UnitX;
        private Vector3 _y = Vector3.UnitY;
        private Vector3 _z = Vector3.UnitZ;

        private RenderConfig _config;
        public Camera() {
            _config = Constants.Kernel.Get<IConfigManager>().GetConfig<RenderConfig>();
        }

        private Matrix4 _modelView;
        private Matrix4 _projection;
        public Matrix4 ModelView {
            get { return _modelView; }
        }
        public Matrix4 Projection {
            get { return _projection; }
        }

		public void Render()
		{
			RenderRotation();
			RenderTranslation();
		}

        public void RenderRotation()
        {
            //GL.MatrixMode(MatrixMode.Modelview);
			
            _modelView = new Matrix4(new Vector4(_x.X, _y.X, _z.X, 0.0f),
                                     new Vector4(_x.Y, _y.Y, _z.Y, 0.0f),
                                     new Vector4(_x.Z, _y.Z, _z.Z, 0.0f),
                                     Vector4.UnitW);

            GL.LoadMatrix(ref _modelView);
        }

		public void RecalculateProjection(int width, int height) {
			GL.Viewport(0, 0, width, height);

			float farClippingPlane = _config.FarClippingPlane;

            float fieldOfView = MathHelper.DegreesToRadians(_config.FieldOfView);

            float aspect_ratio = width / (float)height;

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), aspect_ratio, 1, farClippingPlane);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _projection);
            GL.MatrixMode(MatrixMode.Modelview);
		}
		
        public void RenderTranslation()
        {
            Matrix4 trans = Matrix4.CreateTranslation(-_position);
            GL.MultMatrix(ref trans);
        }

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
				recomputeBasis();
			}
        }

        public void RotateY(float angle)
        {
			if(angle != 0f) {
	            _rot = Quaternion.FromAxisAngle(Vector3.UnitY,angle) * _rot;
				_rot.Normalize();
				recomputeBasis();
			}
        }

        public void RotateZ(float angle)
        {
			if(angle != 0f) {
				_rot = _rot * Quaternion.FromAxisAngle(Vector3.UnitZ,angle);
				_rot.Normalize();
				recomputeBasis();
			}
        }
		
		// Recompute _x, _y, and _z using _rot.
		private void recomputeBasis() {
			_x = Vector3.Transform(Vector3.UnitX,_rot);
			_y = Vector3.Transform(Vector3.UnitY,_rot);
			_z = Vector3.Transform(Vector3.UnitZ,_rot);
		}
		
//		private void GrahmSchmidt() {
//			_y = _y - projection(_y,_x);
//			_z = _z - projection(_z,_x);
//			_z = _z - projection(_z,_y);
//			_x.Normalize();
//			_y.Normalize();
//			_z.Normalize();
//		}
//		
//		private Vector3 projection(Vector3 x, Vector3 y) {
//			return (Vector3.Dot(x,y) / y.LengthSquared) * y;
//		}
		
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        

    }
}
