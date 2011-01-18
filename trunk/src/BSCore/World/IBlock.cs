using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.World;
using JollyBit.BS.Utility;

namespace JollyBit.BS.World
{
    [Flags]
    public enum BlockSides
    {
        None = 0x0,
        Front = 0x1,
        Back = 0x2,
        Left = 0x4,
        Right = 0x8,
        Top = 0x10,
        Bottom = 0x20
    }
    public interface IBlock
    {
        IFileReference GetTextureForSide(BlockSides side);
    }
    public class Block : IBlock
    {
        public IFileReference GetTextureForSide(BlockSides side)
        {
            if (side == BlockSides.Top)
            {
                return new FileReference("grass.png");
            }
            return new FileReference("dirt.png");
        }
    }
}
