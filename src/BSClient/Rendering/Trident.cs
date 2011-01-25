using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Drawing;

using JollyBit.BS.Core.World;

namespace JollyBit.BS.Client.Rendering
{
    public class Trident : IRenderable
    {
        
		
		public Trident() {
		}

        private static Vector3 position = new Vector3(.8f, .8f, -2f);
        private static Matrix4 orthographicProjMatrix = Matrix4.CreateOrthographic(2, 2, 1, 10);
        public void Render() {
			Matrix4 camMatrix;
           	GL.GetFloat(GetPName.ModelviewMatrix, out camMatrix);
			
			GL.PushMatrix();
			GL.LoadIdentity();
            GL.Translate(position);
			GL.MultMatrix(ref camMatrix);

            //switch to ortho proj
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadMatrix(ref orthographicProjMatrix);
			
			GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Red);
                GL.Vertex3(0f, 0f, 0f);
                GL.Vertex3(.1f, 0f, 0f);
                GL.Color3(Color.Green);
                GL.Vertex3(0f, 0f, 0f);
                GL.Vertex3(0f, .1f, 0f);
                GL.Color3(Color.Blue);
                GL.Vertex3(0f, 0f, 0f);
                GL.Vertex3(0f, 0f, .1f);
            GL.End();

            //switch back to prospective proj
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
			
			GL.PopMatrix();
		}
	}
}
