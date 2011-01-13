using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.World
{
    [Flags]
    public enum BlockSides
    {
        Front = 0x0,
        Back = 0x1,
        Left = 0x2,
        Right = 0x4,
        Top = 0x8,
        Bottom = 0x10
    }
    public interface IBlock
    {

    }
    public class Block : IBlock { }
}
