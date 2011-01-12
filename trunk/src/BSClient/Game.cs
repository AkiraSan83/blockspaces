#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

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

#endregion

namespace JollyBit.BS.Rendering
{
    public class BSClient : GameWindow
    {
        Camera camera = new Camera();
        private float keySpeed = 0.05f;
        private float mouseSpeed = 0.001f;

        struct VBO { public int VboID, EboID, NumElements; }

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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(System.Drawing.Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);

            v = new Vbo<VertexPositionColor>(CubeVertices, CubeElements);

            //Point center = new Point();
            //center.X = (int)(this.Location.X + Width / 2.0f);
            //center.Y = (int)(this.Location.Y + Height / 2.0f);
            //System.Windows.Forms.Cursor.Position = center;
            //System.Windows.Forms.Cursor.Hide();
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


        //bool firstUpdate = true;
        //Point mouseCenter;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Application control flow
            if (Keyboard[Key.Escape])
                Exit();

            if (Keyboard[Key.W])
                camera.MoveForward(keySpeed);
            if (Keyboard[Key.S])
                camera.MoveForward(-keySpeed);
            if (Keyboard[Key.D])
                camera.StrafeRight(keySpeed);
            if (Keyboard[Key.A])
                camera.StrafeRight(-keySpeed);
            if (Keyboard[Key.Q])
                camera.RotateY(keySpeed);
            if (Keyboard[Key.E])
                camera.RotateY(-keySpeed);
			
            //if (firstUpdate)
            //{
            //    mouseCenter = new Point(Mouse.X, Mouse.Y);
            //    firstUpdate = false;
            //}
            //float mouseXDelta = mouseCenter.X - Mouse.X;
            //if (mouseXDelta != 0)
            //{
            //    camera.RotateY(-mouseXDelta * mouseSpeed);
            //}
            //float mouseYDelta = mouseCenter.Y - Mouse.Y;
            //if (mouseYDelta != 0)
            //{
            //    camera.RotateX(-mouseYDelta * mouseSpeed);
            //}
            //Point center = new Point();
            //center.X = (int)(this.Location.X + Width / 2.0f);
            //center.Y = (int)(this.Location.Y + Height / 2.0f);
            //System.Windows.Forms.Cursor.Position = center;

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
