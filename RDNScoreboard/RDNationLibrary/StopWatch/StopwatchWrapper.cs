using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;

namespace RDNationLibrary.StopWatch
{
    public enum StopWatchWrapperEnum
    {
        TimeRemaining,
        TimeElapsed,
        IsClockAtZero
    }

    public class StopwatchWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Stopwatch watch;
        Timer timer;

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }
        private long _timerLength;
        public long TimerLength
        {
            get { return _timerLength; }
            set
            {
                _timerLength = value;
                OnPropertyChanged("TimerLength");
            }
        }
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        private long _currentTime;
        public long CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged("CurrentTime");
            }
        }
        private long _timeElapsed;
        public long TimeElapsed
        {
            get { return _timeElapsed; }
            set
            {
                _timeElapsed = value;
                OnPropertyChanged("TimeElapsed");
            }
        }

        private bool _isClockAtZero;
        public bool IsClockAtZero
        {
            get { return _isClockAtZero; }
            set
            {
                _isClockAtZero = value;
                OnPropertyChanged("IsClockAtZero");
            }
        }

        private long _timeRemaining;
        /// <summary>
        /// time remaing in milliseconds from the timer Length
        /// </summary>
        public long TimeRemaining
        {
            get { return _timeRemaining; }
            set
            {
                _timeRemaining = value;
                OnPropertyChanged("TimeRemaining");
            }
        }
        private long _prevTimeRemaining;
        public long PrevTimeRemaining
        {
            get { return _prevTimeRemaining; }
            set
            {
                _prevTimeRemaining = value;
                OnPropertyChanged("PrevTimeRemaining");
            }
        }

        public StopwatchWrapper(long timeForClockInMilliseconds)
        {
            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            TimerLength = timeForClockInMilliseconds;
        }
        /// <summary>
        /// dummy constructor for the xport of xml games
        /// </summary>
        public StopwatchWrapper()
        {
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeSpan ts = DateTime.UtcNow - StartTime;
            TimeElapsed = (long)ts.TotalMilliseconds;
            TimeRemaining = _timerLength - (long)ts.TotalMilliseconds;
            IsRunning = true;
            if (TimeRemaining <= 0)
                IsClockAtZero = true;
            else
                IsClockAtZero = false;  
        }

        public void Stop()
        {
            if (timer != null)
                timer.Stop();
            IsRunning = false;
        }
        public void Start()
        {
            StartTime = DateTime.UtcNow;
            timer.Start();
            IsRunning = true;
            IsClockAtZero = false;
        }

        public void Resume()
        {
            StartTime = DateTime.UtcNow - TimeSpan.FromMilliseconds(TimeElapsed);
            timer.Start();
            IsRunning = true;
        }

        public void Reset()
        {
                        IsRunning = false;
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
