using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using JollyBit.BS.Utility;

namespace JollyBit.BS.Rendering
{
    public interface ITextureAtlasFactory
    {
        ITextureAtlas CreateTextureAtlas(int atlasSize, int subImageSize, int numMipmapLevels);
    }

    public interface ITextureAtlas : IRenderable
    {
        /// <summary>
        /// Adds a sub image to the texture atlas. If the sub image has already been added it is
        /// not added agian.
        /// </summary>
        /// <param name="bitmap">The bitmap to add as a sub image.</param>
        /// <returns>A texture reference allowing the use of the sub image.</returns>
        ITextureReference AddSubImage(IBitmap bitmap);
    }

    public class TextureAtlasFactory : ITextureAtlasFactory
    {
        public ITextureAtlas CreateTextureAtlas(int atlasSize, int subImageSize, int numMipmapLevels)
        {
            return new TextureAtlas(atlasSize, subImageSize, numMipmapLevels);
        }
    }

    public class TextureAtlas : ITextureAtlas
    {
        public readonly int SubImageSize;
        public readonly int NumberOfSubImages;
        public readonly Bitmap Texture;
        public readonly int NumMipmapLevels;
        private int _currentSubImage = 0;
        public readonly int BorderSize;
        private readonly Dictionary<string, ITextureReference> _refCache = new Dictionary<string, ITextureReference>();
        public TextureAtlas(int atlasSize, int subImageSize, int numMipmapLevels)
        {
            BorderSize = numMipmapLevels;
            SubImageSize = subImageSize - numMipmapLevels * 2;
            NumberOfSubImages = atlasSize / subImageSize;
            NumMipmapLevels = numMipmapLevels;
            //Create empty white bitmap to hold atlas
            Texture = new Bitmap(atlasSize, atlasSize);
            using (Graphics graphics = Graphics.FromImage(Texture))
            {
                graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Texture.Width, Texture.Height));
            }
        }

        public ITextureReference AddSubImage(IBitmap bitmap)
        {
            //Check if already added
            ITextureReference textureRef;
            if (_refCache.TryGetValue(bitmap.UniqueId, out textureRef))
            {
                return textureRef;
            }

            //Clean up old texture object
            if (_textureObject != null)
            {
                _textureObject.Dispose();
                _textureObject = null;
            }

            //Add sub image
            Rectangle rect = new Rectangle(
                (_currentSubImage % NumberOfSubImages) * (SubImageSize + BorderSize * 2) + BorderSize,
                (_currentSubImage / NumberOfSubImages) * (SubImageSize + BorderSize * 2) + BorderSize,
                SubImageSize - 2 * BorderSize,
                SubImageSize - 2 * BorderSize);

            using (Graphics g = Graphics.FromImage(Texture))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(bitmap.Bitmap, rect); 
            }
            _currentSubImage++;
            //Create ITextureReference
            RectangleF location = new RectangleF(rect.X / (float)Texture.Width, rect.Y / (float)Texture.Height,
                rect.Width / (float)Texture.Width, rect.Height / (float)Texture.Height);
            textureRef = new TextureReference(location, new Action(Render));
            _refCache.Add(bitmap.UniqueId, textureRef);
            return textureRef;
        }

        private GLTextureObject _textureObject = null;
        public void Render()
        {
            if (_textureObject == null)
            {
                _textureObject = new GLTextureObject(Texture, NumMipmapLevels);
            }
            _textureObject.Render();
        }
    }
}
