using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using Ninject;

namespace JollyBit.BS.Core
{
    public class Constants
    {
        public const byte CHUNK_SIZE_X = 50;
        public const byte CHUNK_SIZE_Y = 50;
        public const byte CHUNK_SIZE_Z = 50;

        public static IKernel Kernel = null;
    }
}
