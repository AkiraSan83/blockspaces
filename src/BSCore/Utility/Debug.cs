using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace JollyBit.BS.Core.Utility
{
    public static class Debug
    {
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void AssertTrue(bool value, string message, params object[] parms)
        {
            if (!value)
            {
                throw new System.Exception(string.Format(message, parms));
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void AssertFalse(bool value, string message, params object[] parms)
        {
            if (value)
            {
                throw new System.Exception(string.Format(message, parms));
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void AssertNotNull(object value, string message, params object[] parms)
        {
            if (value == null)
            {
                throw new System.Exception(string.Format(message, parms));
            }
        } 
    }
}
