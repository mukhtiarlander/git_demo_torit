using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.ViewModel.Video
{
    public enum OverlayEnum { LogoPictureLocation, LogoOnOff, TopTeam1Color, BottomTeam1Color, FontTeam1Color, TopTeam2Color, BottomTeam2Color, TimeOutColor, TextColor, ScoresOnTop, OverlayColor, IsOverlayTransparent, GreenScreen, ShowJamScore }
    public class Overlay : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Overlay()
        {
            TurnOffTimeOuts = true;
        }

        private bool _logoOnOff;
        public bool LogoOnOff
        {
            get { return _logoOnOff; }
            set
            {
                _logoOnOff = value;
                OnPropertyChanged("LogoOnOff");
            }
        }
        private bool _scoresOnTop;
        public bool ScoresOnTop
        {
            get { return _scoresOnTop; }
            set
            {
                _scoresOnTop = value;
                OnPropertyChanged("ScoresOnTop");
            }
        }
        private bool _isTransparent;
        public bool IsOverlayTransparent
        {
            get { return _isTransparent; }
            set
            {
                _isTransparent = value;
                OnPropertyChanged("IsOverlayTransparent");
            }
        }
        private int _verticalPosition;
        public int VerticalPosition
        {
            get { return _verticalPosition; }
            set
            {
                _verticalPosition = value;
                OnPropertyChanged("VerticalPosition");
            }
        }
        private double _textSizePosition;
        public double TextSizePosition
        {
            get { return _textSizePosition; }
            set
            {
                _textSizePosition = value;
                OnPropertyChanged("TextSizePosition");
            }
        }

        private bool _isBottomRowOn;
        public bool IsBottomRowOn
        {
            get { return _isBottomRowOn; }
            set
            {
                _isBottomRowOn = value;
                OnPropertyChanged("IsBottomRowOn");
            }
        }
        private bool _showJamScore;
        public bool ShowJamScore
        {
            get { return _showJamScore; }
            set
            {
                _showJamScore = value;
                OnPropertyChanged("ShowJamScore");
            }
        }
        private bool _showTimeOuts;
        public bool TurnOffTimeOuts
        {
            get { return _showTimeOuts; }
            set
            {
                _showTimeOuts = value;
                OnPropertyChanged("ShowTimeOuts");
            }
        }
        private string _topTeam1Color;
        public string TopTeam1Color
        {
            get { return _topTeam1Color; }
            set
            {
                _topTeam1Color = value;
                OnPropertyChanged("TopTeam1Color");
            }
        }
        private string _bottomTeam1Color;
        public string BottomTeam1Color
        {
            get { return _bottomTeam1Color; }
            set
            {
                _bottomTeam1Color = value;
                OnPropertyChanged("BottomTeam1Color");
            }
        }
        private string _fontTeam1Color;
        public string FontTeam1Color
        {
            get { return _fontTeam1Color; }
            set
            {
                _fontTeam1Color = value;
                OnPropertyChanged("FontTeam1Color");
            }
        }
        private string _topTeam2Color;
        public string TopTeam2Color
        {
            get { return _topTeam2Color; }
            set
            {
                _topTeam2Color = value;
                OnPropertyChanged("TopTeam2Color");
            }
        }
        private string _bottomTeam2Color;
        public string BottomTeam2Color
        {
            get { return _bottomTeam2Color; }
            set
            {
                _bottomTeam2Color = value;
                OnPropertyChanged("BottomTeam2Color");
            }
        }
        private string _fontTeam2Color;
        public string FontTeam2Color
        {
            get { return _fontTeam2Color; }
            set
            {
                _fontTeam2Color = value;
                OnPropertyChanged("FontTeam2Color");
            }
        }
        private string _timeOutColor;
        public string TimeOutColor
        {
            get { return _timeOutColor; }
            set
            {
                _timeOutColor = value;
                OnPropertyChanged("TimeOutColor");
            }
        }
        private string _textColor;
        public string TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
                OnPropertyChanged("TextColor");
            }
        }

        private string _greenScreen;
        public string GreenScreen
        {
            get { return _greenScreen; }
            set
            {
                _greenScreen = value;
                OnPropertyChanged("GreenScreen");
            }
        }


        private string _overlayColor;
        public string OverlayColor
        {
            get { return _overlayColor; }
            set
            {
                _overlayColor = value;
                OnPropertyChanged("OverlayColor");
            }
        }

        private string _logoPictureLocation;
        public string LogoPictureLocation
        {
            get { return _logoPictureLocation; }
            set
            {
                _logoPictureLocation = value;
                OnPropertyChanged("LogoPictureLocation");
            }
        }
        private byte[] _logoPictureCompressed;
        /// <summary>
        /// used to transfer the actual picture, encoding it into a byte array.
        /// </summary>
        public byte[] LogoPictureCompressed
        {
            get { return _logoPictureCompressed; }
            set
            {
                _logoPictureCompressed = value;
                OnPropertyChanged("LogoPictureCompressed");
            }
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
