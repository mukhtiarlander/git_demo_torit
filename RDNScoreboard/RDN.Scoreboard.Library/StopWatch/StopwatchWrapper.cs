using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;

namespace Scoreboard.Library.StopWatch
{
    public enum StopWatchWrapperEnum
    {
        TimeRemaining,
        TimeElapsed,
        IsClockAtZero,
        IsPaused
    }
    public enum StopWatchTypeEnum
    {
        PeriodClock = 1,
        JamClock = 2,
        LineUpClock = 3,
        IntermissionClock = 4,
        TimeOutClock = 5
    }

    public class StopwatchWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Timer timer;

        private bool _isPaused;
        /// <summary>
        /// is the clock currently counting down.
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                _isPaused = value;
                OnPropertyChanged("IsPaused");
            }
        }
        /// <summary>
        /// we need to track how many milliseconds occured since the last elapsed timer interval.
        /// </summary>
        private long _millisecondsSinceLastInterval = 0;

        private bool _isRunning;
        /// <summary>
        /// is the clock currently counting down.
        /// </summary>
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

        public StopwatchWrapper(long timeForClockInMilliseconds)
        {
            timer = new Timer(500);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            TimerLength = timeForClockInMilliseconds;
            TimeRemaining = timeForClockInMilliseconds;
            TimeElapsed = 0;
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
            _millisecondsSinceLastInterval = (long)ts.TotalMilliseconds - _millisecondsSinceLastInterval;

            TimeElapsed += _millisecondsSinceLastInterval;
            TimeRemaining = _timerLength - TimeElapsed;

            _millisecondsSinceLastInterval = (long)ts.TotalMilliseconds;



            //Console.WriteLine(TimeElapsed);
            //Console.WriteLine(_timerLength);
            //Console.WriteLine(TimeRemaining);
            //Console.WriteLine(StartTime);
            IsRunning = true;
            if (TimeRemaining <= 0)
                IsClockAtZero = true;
        }

        public void Stop()
        {
            if (timer != null)
                timer.Stop();
            IsRunning = false;
        }
        public void Pause()
        {
            if (timer != null && IsRunning)
            {
                timer.Stop();
                IsPaused = true;
            }
        }
        public void Start()
        {
            StartTime = DateTime.UtcNow;
            timer.Start();
            IsRunning = true;
            IsClockAtZero = false;
            _millisecondsSinceLastInterval = 0;
        }
        /// <summary>
        /// resumes the clock.
        /// </summary>
        public void Resume()
        {
            if (IsPaused)
            {
                StartTime = DateTime.UtcNow;
                timer.Start();
                IsRunning = true;
                IsPaused = false;
                _millisecondsSinceLastInterval = 0;
                //Console.WriteLine(TimeElapsed);
                //Console.WriteLine(_timerLength);
                //Console.WriteLine(TimeRemaining);
                //Console.WriteLine(StartTime);
            }
        }
        /// <summary>
        /// adds seconds to the clock.
        /// </summary>
        /// <param name="seconds">seconds to be added.  Can be negative or positive.</param>
        public void AddSecondsToClock(int seconds)
        {
            if (StartTime == new DateTime())
            {
                StartTime = DateTime.UtcNow;
                TimeRemaining = _timerLength;
            }
            StartTime = StartTime.AddSeconds(seconds);
            TimeRemaining += seconds * 1000;
            TimerLength += seconds * 1000;
            //Console.WriteLine("change");
            //Console.WriteLine("change"+TimeRemaining);
            //Console.WriteLine("change"+TimerLength);
        }
        /// <summary>
        /// changes the amount of seconds of a clock
        /// </summary>
        /// <param name="seconds"></param>
        public void changeSecondsOfClock(int seconds)
        {
            TimeElapsed = 0;
            TimeRemaining = seconds * 1000;
            TimerLength = seconds * 1000;
            //Console.WriteLine("change");
            //Console.WriteLine("change" + seconds);
            //Console.WriteLine("change" + TimeRemaining);
            //Console.WriteLine("change" + TimerLength);
        }

        public void Reset()
        {
            IsRunning = false;
            StartTime = DateTime.UtcNow;
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
