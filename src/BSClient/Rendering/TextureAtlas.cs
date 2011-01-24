using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Client.Rendering
{
    public interface ITextureAtlasFactory
    {
        ITextureAtlas CreateTextureAtlas(Size atlasSize, Size subImageSize, int numMipmapLevels);
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
        public ITextureAtlas CreateTextureAtlas(Size atlasSize, Size subImageSize, int numMipmapLevels)
        {
            return new TextureAtlas(atlasSize, subImageSize, numMipmapLevels);
        }
    }

    public class TextureAtlas : ITextureAtlas
    {
        public readonly Size SubImageSize;
        public readonly int NumberOfSubImages;
        public readonly Bitmap Texture;
        public readonly int NumMipmapLevels;
        private int _currentSubImage = 0;
        public readonly int BorderSize;
        private readonly Dictionary<string, ITextureReference> _refCache = new Dictionary<string, ITextureReference>();
        public TextureAtlas(Size atlasSize, Size subImageSize, int numMipmapLevels)
        {
			System.Diagnostics.Debug.Assert(atlasSize.Width < 0, "Texture atlas WIDTH and height must be positive.");
			System.Diagnostics.Debug.Assert(atlasSize.Height < 0, "Texture atlas width and HEIGHT must be positive.");
			System.Diagnostics.Debug.Assert((atlasSize.Width & (atlasSize.Width - 1)) != 0, "Texture atlas WIDTH and height must be powers of two.");
			System.Diagnostics.Debug.Assert((atlasSize.Height & (atlasSize.Height - 1)) != 0, "Texture atlas width and HEIGHT must be powers of two.");
			
            BorderSize = numMipmapLevels;
            SubImageSize.Width = subImageSize.Width - numMipmapLevels * 2;
			SubImageSize.Height = subImageSize.Height - numMipmapLevels * 2;
            NumberOfSubImages = atlasSize.Width / subImageSize.Width;
            NumMipmapLevels = numMipmapLevels;
			
            //Create empty white bitmap to hold atlas
            Texture = new Bitmap(atlasSize.Width, atlasSize.Height);
            using (Graphics graphics = Graphics.FromImage(Texture))
            {
                graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Texture.Width, Texture.Height));
            }
        }
		
		private unsafe uint* _findPixel(BitmapData currentData, long x, long y) {
			return (uint *)(currentData.Scan0) + (uint)y * (uint)(currentData.Stride/4) + (uint)x; 	
		}
		
		private unsafe void addBorder(ref Rectangle rect) {
			Rectangle editing = new Rectangle(
				rect.X - BorderSize,
			    rect.Y - BorderSize,
			    rect.Width + 2 * BorderSize,
			    rect.Height + 2 * BorderSize
			);
			
			BitmapData currentData = Texture.LockBits(editing, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			
			if(currentData.Stride < 0)
				throw new System.NotImplementedException("Stride is negative. FIXME!");
			
			uint* scan0 = (uint *)(currentData.Scan0);
			uint strideInInts = (uint)(currentData.Stride/4); // Stride in bytes divided by 4 (8 bits -> 32 bits)
			Func<long,long,uint> findPixel = (x,y) => (uint)y * strideInInts + (uint)x;
			
			// Handle top
			for(uint y = 0; y < BorderSize; y++) { // per-line
				for(uint x = 0; x < rect.Width; x++) { // per-column
					*(scan0 + findPixel(BorderSize + x, y)) = *(scan0 + findPixel(BorderSize + x, BorderSize));
				}
			}
			
			// Handle bottom
			for(uint y = 0; y < BorderSize; y++) { // per-line
				for(uint x = 0; x < rect.Width; x++) { // per-column
					*(scan0 + findPixel(BorderSize + x, BorderSize + rect.Height + y)) = *(scan0 + findPixel(BorderSize + x, BorderSize + rect.Height - 1));
				}
			}
			
			// Handle left
			for(uint y = 0; y < rect.Height; y++) {
				for(uint x = 0; x < BorderSize; x++) {
					*(scan0 + findPixel(x, BorderSize + y)) = *(scan0 + findPixel(BorderSize, BorderSize + y));
				}
			}
			
			// Handle right
			for(uint y = 0; y < rect.Height; y++) {
				for(uint x = 0; x < BorderSize; x++) {
					*(scan0 + findPixel(rect.Width + BorderSize + x, BorderSize + y)) = *(scan0 + findPixel(rect.Width + BorderSize - 1, BorderSize + y));
				}
			}			
			
			/// Ignored corners because there were no visable rendering artifacts		
			
			Texture.UnlockBits(currentData);
			
			// To save texture atlas to file after add, uncomment the following line.
			//Texture.Save(string.Format("/tmp/texture-{0}.bmp",System.DateTime.Now.Millisecond));
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
                (_currentSubImage % NumberOfSubImages) * (SubImageSize.Width) + BorderSize, // X
                (_currentSubImage / NumberOfSubImages) * (SubImageSize.Height) + BorderSize, // Y
                SubImageSize.Width - 2 * BorderSize, // Width
                SubImageSize.Height - 2 * BorderSize); // Height

            using (Graphics g = Graphics.FromImage(Texture))
            {
                //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				// HighQualityBicubic uses a averaging step at the end that caused a gray line to be introduced in the outermost pixel of the texture
				// This was then propagated into the border, causing fuzzy white lines to appear between textures. If we need the effect of HQB, we can
				// re-paste the image over the top of this after calculating the border.
				
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				//Console.WriteLine("Writing subimage {0} (sized {1}) to area {2}.",_currentSubImage,SubImageSize,rect);
                g.DrawImage(bitmap.Bitmap, rect); 
            }
			
			// Extend the last pixel out past the border
			addBorder(ref rect);
			
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
