using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Server
{
    public class Server : ITimeService
    {
        Thread _thread = null;
        Stopwatch _stopWatch = new Stopwatch();
        public Server()
        {

        }

        private int _tickIntervalMilliseconds = 1000 / 20;
        public double TickInterval
        {
            get { return _tickIntervalMilliseconds / 1000.0; }
            set { _tickIntervalMilliseconds = (int)(value * 1000.0); }

        }

        public void Start()
        {
            if (_thread != null)
            {
                throw new System.Exception("Server is already started.");
            }
            _thread = new Thread(new ThreadStart(_start));
            _thread.Start();
        }

        private long _lastStopWatch = 0;
        public void _start()
        {
            _lastStopWatch = -_tickIntervalMilliseconds;
            long targetElapsed = 0;
            _stopWatch.Start();
            while (true)
            {
                if (_stopWatch.ElapsedMilliseconds > targetElapsed)
                {
                    _elapsedTime = (_stopWatch.ElapsedMilliseconds - _lastStopWatch) / 1000.0;
                    _lastStopWatch = _stopWatch.ElapsedMilliseconds;
                    _currentTime = _lastStopWatch / 1000.0;
                    targetElapsed = _lastStopWatch + _tickIntervalMilliseconds;
                    if (Tick != null) Tick(this, new TimeTickEventArgs(_elapsedTime, _currentTime));
                }
                else Thread.Sleep(1);
            }
        }

        public event EventHandler<TimeTickEventArgs> Tick;

        private double _currentTime = -1;
        public double CurrentTime
        {
            get { return _currentTime; }
        }

        private double _elapsedTime = -1;
        public double ElapsedTime
        {
            get { return _elapsedTime; }
        }
    }
}
