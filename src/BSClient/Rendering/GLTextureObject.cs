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

        //public static Bitmap ColorArrayToBitmap(Color[,] colorArray)
        //{
        //    Bitmap bitm = new Bitmap(colorArray.GetLength(0), colorArray.GetLength(1), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    BitmapData bitmapdat = bitm.LockBits(new Rectangle(0, 0, bitm.Width, bitm.Height), ImageLockMode.ReadWrite, bitm.PixelFormat);
        //    int stride = bitmapdat.Stride;

        //    byte[] bytes = new byte[stride * bitm.Height];
        //    for (int x = 0; x < bitm.Height; x++)
        //    {
        //        for (int y = 0; y < bitm.Width; y++)
        //        {
        //            bytes[(x * stride) + y * 3 + 0] = colorArray[x, y].G;
        //            bytes[(x * stride) + y * 3 + 1] = colorArray[x, y].G;
        //            bytes[(x * stride) + y * 3 + 2] = colorArray[x, y].R;
        //            bytes[(x * stride) + y * 3 + 3] = colorArray[x, y].A;
        //        }
        //    }
        //    System.Runtime.InteropServices.Marshal.Copy(bytes, 0, bitmapdat.Scan0, stride * bitm.Height);
        //    bitm.UnlockBits(bitmapdat);
        //    return bitm;
        //}

    }
}
