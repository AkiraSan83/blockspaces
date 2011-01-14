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
using System.Linq;
using Ninject;
using Ninject.Modules;

using JollyBit.BS.Rendering;
using JollyBit.BS.World;

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

            //Setup Ninject
            IKernel kenel = new StandardKernel();
            kenel.Load(new INinjectModule[] { new BSCoreNinjectModule(), new BSClientNinjectModule() });

			// Handle mouse and keyboard events
			new Input(this);
				
            // Set OpenGL options
			GL.ClearColor(System.Drawing.Color.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            // Create World Renderer
            MapRenderer mapRenderer = kenel.Get<MapRenderer>();
            _renderList.Add(mapRenderer);
            IChunk c = mapRenderer.Map[new Utility.Point3L(0, 0, 0)];
            c = mapRenderer.Map[new Utility.Point3L(-1, 0, 0)];
            c = mapRenderer.Map[new Utility.Point3L(0, -1, 0)];
            c = mapRenderer.Map[new Utility.Point3L(0, 0, -1)];

            //// Build Dynamic Cube!
            //IList<VertexPositionColor> verts = new List<VertexPositionColor>();
            //IList<short> indexs = new List<short>();
            //Vector3 pos = new Vector3(0, 0, 0);
            //ChunkRenderer.createCubeSide(ref verts, ref indexs, pos, BlockSides.Front | BlockSides.Back | BlockSides.Left | BlockSides.Right | BlockSides.Bottom | BlockSides.Top);

            //// Add the cube to the render list
            //VertexPositionColor[] vertsArr = verts.ToArray();
            //short[] indexsArr = indexs.ToArray();
            //_renderList.Add(new Vbo<VertexPositionColor>(vertsArr, indexsArr));
			
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
