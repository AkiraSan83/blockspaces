using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Utility;
using Ninject;

namespace JollyBit.BS
{
    public class BSCoreConstants
    {
        public const byte CHUNK_SIZE_X = 50;
        public const byte CHUNK_SIZE_Y = 50;
        public const byte CHUNK_SIZE_Z = 50;

        public static IKernel Kernel = null;
    }
}
