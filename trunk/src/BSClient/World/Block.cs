using System;

using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World;

namespace JollyBit.BS.Client.World
{
    public class Block : IBlock
    {
        private readonly string Left, Right, Front, Back, Top, Bottom;

        public Block(string left, string right, string front, string back, string top, string bottom) {
            Left = left;
            Right = right;
            Front = front;
            Back = back;
            Top = top;
            Bottom = bottom;
        }

        public FileReference GetTextureForSide(BlockSides side) {
            switch(side) {
                case BlockSides.Left: return new FileReference(Left);
                case BlockSides.Right: return new FileReference(Right);
                case BlockSides.Front: return new FileReference(Front);
                case BlockSides.Back: return new FileReference(Back);
                case BlockSides.Top: return new FileReference(Top);
                case BlockSides.Bottom: return new FileReference(Bottom);
            }
            return new FileReference("invalid.png");
        }
    }
}

