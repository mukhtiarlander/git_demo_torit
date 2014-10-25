using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.ViewModel.ClockView
{
    public enum ScoreboardSettingsEnum
    {
        BackgroundPictureLocation, BackgroundPictureCompressed
    }

    public class ScoreboardSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _backgroundPictureLocation;
        public string BackgroundPictureLocation
        {
            get { return _backgroundPictureLocation; }
            set
            {
                _backgroundPictureLocation = value;
                OnPropertyChanged("BackgroundPictureLocation");
            }
        }

        private byte[] _backgroundPictureCompressed;
        /// <summary>
        /// used to transfer the actual picture, encoding it into a byte array.
        /// </summary>
        public byte[] BackgroundPictureCompressed
        {
            get { return _backgroundPictureCompressed; }
            set
            {
                _backgroundPictureCompressed = value;
                OnPropertyChanged("BackgroundPictureCompressed");
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public ScoreboardSettings()
        { }
    }
}
