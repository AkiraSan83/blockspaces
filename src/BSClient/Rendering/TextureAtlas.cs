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
        public readonly int SubImageSize;
        public readonly int NumberOfSubImages;
        public readonly Bitmap Texture;
        private int _currentSubImage = 0;
        public readonly int BorderSize;
        public TextureAtlas(int atlasSize, int subImageSize, int borderSize)
        {
            BorderSize = borderSize;
            SubImageSize = subImageSize;
            NumberOfSubImages = atlasSize / subImageSize;
            Texture = new Bitmap(atlasSize, atlasSize);
            using (Graphics graphics = Graphics.FromImage(Texture))
            {
                graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Texture.Width, Texture.Height));
            }
        }

        public RectangleF AddSubImage(Bitmap subImageToAdd)
        {
            Rectangle rect = new Rectangle(
                (_currentSubImage % NumberOfSubImages) * (SubImageSize + BorderSize * 2) + BorderSize,
                (_currentSubImage / NumberOfSubImages) * (SubImageSize + BorderSize * 2) + BorderSize,
                SubImageSize - 2 * BorderSize,
                SubImageSize - 2 * BorderSize);

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
