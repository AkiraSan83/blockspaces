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
        public Camera camera = new Camera();

        VertexPositionColor[] CubeVertices = new VertexPositionColor[]
        {
                new VertexPositionColor(-1.0f, -1.0f,  1.0f, Color.DarkRed),
                new VertexPositionColor( 1.0f, -1.0f,  1.0f, Color.DarkRed),
                new VertexPositionColor( 1.0f,  1.0f,  1.0f, Color.Gold),
                new VertexPositionColor(-1.0f,  1.0f,  1.0f, Color.Gold),
                new VertexPositionColor(-1.0f, -1.0f, -1.0f, Color.DarkRed),
                new VertexPositionColor( 1.0f, -1.0f, -1.0f, Color.DarkRed), 
                new VertexPositionColor( 1.0f,  1.0f, -1.0f, Color.Gold),
                new VertexPositionColor(-1.0f,  1.0f, -1.0f, Color.Gold) 
        };

        readonly short[] CubeElements = new short[]
        {
            0, 1, 2, 2, 3, 0, // front face
            3, 2, 6, 6, 7, 3, // top face
            7, 6, 5, 5, 4, 7, // back face
            4, 0, 3, 3, 7, 4, // left face
            0, 1, 5, 5, 4, 0, // bottom face
            1, 5, 6, 6, 2, 1, // right face
        };

        public BSClient() : base(800, 600) { }

        private Vbo<VertexPositionColor> v;
		
		private Point _center;
		protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			Input i = new Input(this);
			
            GL.ClearColor(System.Drawing.Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);

            v = new Vbo<VertexPositionColor>(CubeVertices, CubeElements);
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

            camera.Render();

            //Render Trident
            GL.LineWidth(2f);
            GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Red); 
                GL.Vertex3(0f, 0f, 0f);
                GL.Vertex3(1f, 0f, 0f);
                GL.Color3(Color.Green);
                GL.Vertex3(0f, 0f, 0f);
                GL.Vertex3(0f, 1f, 0f);
                GL.Color3(Color.Blue);
                GL.Vertex3(0f, 0f, 0f);
                GL.Vertex3(0f, 0f, 1f);
            GL.End();            

            //v.Render();

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
