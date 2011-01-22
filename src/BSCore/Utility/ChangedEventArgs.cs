using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public class ItemChangedEventArgs<T> : EventArgs
    {
        public readonly T OldValue;
        public readonly T NewValue;
        public ItemChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
