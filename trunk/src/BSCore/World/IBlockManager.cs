using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JollyBit.BS.Core.Networking.Messages;

namespace JollyBit.BS.Core.World
{
    public interface IBlockManager : BS.Core.Utility.IService
    {
        ushort getShortFromBlock(IBlock block);
        IBlock getBlockFromShort(ushort id);
    }
}

