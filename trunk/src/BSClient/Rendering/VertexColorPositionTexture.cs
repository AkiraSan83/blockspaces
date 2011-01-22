using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Drawing;
namespace JollyBit.BS.Client.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTexture
    {
        public Vector3 Position;
        public uint Color;
        public Vector2 Texture;

        public VertexPositionColorTexture(float x, float y, float z, Color color, float textureX, float textureY)
        {
            Position = new Vector3(x, y, z);
            Color = ToRgba(color);
            Texture = new Vector2(textureX, textureY);
        }

        static uint ToRgba(Color color)
        {
            return (uint)color.A << 24 | (uint)color.B << 16 | (uint)color.G << 8 | (uint)color.R;
        }
    }
}
