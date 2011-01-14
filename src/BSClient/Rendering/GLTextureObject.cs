using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace JollyBit.BS.Rendering
{
    public class GLTextureObject : IDisposable, IRenderable
    {
        private int _id;
        public GLTextureObject(Bitmap bitmap)
        {
            _id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _id);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (GraphicsContext.CurrentContext != null)
            {
                GL.DeleteTexture(_id);
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~GLTextureObject()
        {
            Dispose(false);
        }

        public void Render()
        {
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }
    }
}
