using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public class EventArgs<T> : EventArgs
    {
        public readonly T Data;
        public EventArgs(T data)
        {
            Data = data;
        }
    }
}
