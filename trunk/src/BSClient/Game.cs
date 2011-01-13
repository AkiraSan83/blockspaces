#if !_WIN32
	#define MONO
#endif

#region --- Using directives ---

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Input;
using System.Drawing;

using JollyBit.BS.Rendering;

#endregion

namespace JollyBit.BS
{
	public class BSClient : GameWindow
    {
		private IList<IRenderable> _renderList = new List<IRenderable>();
		private Camera _camera = new Camera();
		
		public IList<IRenderable> RenderList {
			get { return _renderList; }
		}
		public Camera Camera {
			get { return _camera; }
		}

        public BSClient() : base(800, 600) { }

		protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			// Handle mouse and keyboard events
			new Input(this);
				
            // Set OpenGL options
			GL.ClearColor(System.Drawing.Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

			// Build Dynamic Cube!
            VertexPositionColor[] verts = new VertexPositionColor[4 * 6];
            short[] indexs = new short[6 * 6];
            Vector3 pos = new Vector3(0, 0, 0);
            ChunkRenderer._createCubeSide(ref verts, 4 * 0, ref indexs, 6 * 0, ref pos, ChunkRenderer.CubeSideTypes.Front);
            ChunkRenderer._createCubeSide(ref verts, 4 * 1, ref indexs, 6 * 1, ref pos, ChunkRenderer.CubeSideTypes.Back);
            ChunkRenderer._createCubeSide(ref verts, 4 * 2, ref indexs, 6 * 2, ref pos, ChunkRenderer.CubeSideTypes.Left);
            ChunkRenderer._createCubeSide(ref verts, 4 * 3, ref indexs, 6 * 3, ref pos, ChunkRenderer.CubeSideTypes.Right);
            ChunkRenderer._createCubeSide(ref verts, 4 * 4, ref indexs, 6 * 4, ref pos, ChunkRenderer.CubeSideTypes.Bottom);
            ChunkRenderer._createCubeSide(ref verts, 4 * 5, ref indexs, 6 * 5, ref pos, ChunkRenderer.CubeSideTypes.Top);
            
			// Add the cube to the render list
			_renderList.Add( new Vbo<VertexPositionColor>(verts, indexs) );
			
			// Build trident and add to the render list
			_renderList.Add( new Trident() );
        }    

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
			
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Render the camera
            _camera.Render();

			// Render each item in the render list
			foreach(var renderable in _renderList) {
				renderable.Render();
			}

            SwapBuffers();
        }

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (BSClient example = new BSClient())
            {
                example.Run(30.0, 0.0);
            }
        }
    }
}
