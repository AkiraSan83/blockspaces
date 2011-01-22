using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.World
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
	   
	public class AnnoyingStoneBlock : IBlock
    {
        public IFileReference GetTextureForSide(BlockSides side)
        {
//            if(side == BlockSides.Top)
//                return new FileReference("stone_top.png");
//			if(side == BlockSides.Bottom)
//                return new FileReference("stone_bottom.png");
//			if(side == BlockSides.Left)
//                return new FileReference("stone_left.png");
//			if(side == BlockSides.Right)
//                return new FileReference("stone_right.png");
//			if(side == BlockSides.Front)
//                return new FileReference("stone_front.png");
//			if(side == BlockSides.Back)
//                return new FileReference("stone_back.png");
            return new FileReference("stone.png");
        }
    }
}
