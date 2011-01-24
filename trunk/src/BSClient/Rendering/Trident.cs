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
    public class Trident : IRenderable, IPositionable
    {
		public Vector3 Position { get; set; }
		
		public Trident(Vector3 position) {
			Position = position;
		}
		
		public Trident() {
			Position = new Vector3(0f,0f,0f);
		}
		
        public void Render() {
			Matrix4 camMatrix;
           	GL.GetFloat(GetPName.ModelviewMatrix, out camMatrix);
			
			GL.PushMatrix();
			GL.LoadIdentity();
			GL.Translate(Position);
			GL.MultMatrix(ref camMatrix);
			
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
			
			GL.PopMatrix();
		}
	}
}
