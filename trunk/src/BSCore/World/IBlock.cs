using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    }
    public class Block : IBlock { }
}
