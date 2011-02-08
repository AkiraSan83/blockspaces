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
        public const byte CHUNK_SIZE_X = 20;
        public const byte CHUNK_SIZE_Y = 20;
        public const byte CHUNK_SIZE_Z = 20;

        public static IKernel Kernel = null;
    }
}
