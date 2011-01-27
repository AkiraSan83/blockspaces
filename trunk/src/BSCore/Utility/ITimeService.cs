using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public interface ITimeService
    {
        event EventHandler<TimeTickEventArgs> Tick;
        double CurrentTime { get; }
        double ElapsedTime { get; }
    }
    public class TimeTickEventArgs : EventArgs
    {
        /// <summary>
        /// Time in seconds since last call
        /// </summary>
        public readonly double ElapsedTime;
        public readonly double CurrentTime;
        public TimeTickEventArgs(double elapsedTime, double currentTime)
        {
            ElapsedTime = elapsedTime;
            CurrentTime = currentTime;
        }
    }
}
