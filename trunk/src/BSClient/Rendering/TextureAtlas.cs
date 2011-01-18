using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace JollyBit.BS.Rendering
{
    public class TextureAtlas
    {
        public readonly Size SubImageSize;
        public readonly Size NumberOfSubImages;
        public readonly Bitmap Texture;
        private int _currentSubImage = 0;
        public readonly int BorderSize;
        public TextureAtlas(Size atlasSize, Size subImageSize, int borderSize)
        {
            BorderSize = borderSize;
            SubImageSize = subImageSize;
            NumberOfSubImages = new Size(
                atlasSize.Width / (subImageSize.Width + borderSize * 2),
                atlasSize.Height / (subImageSize.Height + borderSize * 2));
            Texture = new Bitmap(atlasSize.Width, atlasSize.Height);
        }

        public RectangleF AddSubImage(Bitmap subImageToAdd)
        {
            Rectangle rect = new Rectangle(
                (_currentSubImage % NumberOfSubImages.Width) * (SubImageSize.Width + BorderSize * 2) + BorderSize, 
                (_currentSubImage / NumberOfSubImages.Width) * (SubImageSize.Height + BorderSize * 2) + BorderSize,
                SubImageSize.Width,
                SubImageSize.Height);

            using (Graphics g = Graphics.FromImage(Texture))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(subImageToAdd, rect); 
            }

            _currentSubImage++;
            return new RectangleF(rect.X / (float)Texture.Width, rect.Y / (float)Texture.Height,
                rect.Width / (float)Texture.Width, rect.Height / (float)Texture.Height);
        }
    }
}
