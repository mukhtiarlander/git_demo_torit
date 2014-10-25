using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Scoreboard.Library.ViewModel
{
    public enum LineUpTypesEnum { NONE,PlainLineUp, SideBarLineUp }
    public enum LineUpViewModelEnum
    {
        LineUpTypeSelected, PlainBorderColor,
        PlainTextColor, PlainBackgroundColor, SidebarColor,
        SidebarTextColor, SidebarSkaterTextColor, SidebarBackgroundColor
    }
    /// <summary>
    /// holds the settings for the line up controls and slideshow
    /// </summary>
    public class LineUpViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LineUpViewModel()
        { }

        private LineUpTypesEnum _lineUpTypeSelected;
        /// <summary>
        /// what current line up has been selected.
        /// </summary>
        public LineUpTypesEnum LineUpTypeSelected
        {
            get { return _lineUpTypeSelected; }
            set
            {
                _lineUpTypeSelected = value;
                OnPropertyChanged("LineUpTypeSelected");
            }
        }

        private string _plainBorderColor;
        /// <summary>
        /// the border color of the plain line up view
        /// </summary>
        public string PlainBorderColor
        {
            get { return _plainBorderColor; }
            set
            {
                _plainBorderColor = value;
                OnPropertyChanged("PlainBorderColor");
            }
        }
        private string _plainTextColor;
        /// <summary>
        /// the text color of the plain line up view
        /// </summary>
        public string PlainTextColor
        {
            get { return _plainTextColor; }
            set
            {
                _plainTextColor = value;
                OnPropertyChanged("PlainTextColor");
            }
        }
        private string _plainBackgroundColor;
        /// <summary>
        /// the background color of the plain line up view
        /// </summary>
        public string PlainBackgroundColor
        {
            get { return _plainBackgroundColor; }
            set
            {
                _plainBackgroundColor = value;
                OnPropertyChanged("PlainBackgroundColor");
            }
        }

        private string _sidebarColor;
        /// <summary>
        /// the color of the side bar in the sidebar line up view
        /// </summary>
        public string SidebarColor
        {
            get { return _sidebarColor; }
            set
            {
                _sidebarColor = value;
                OnPropertyChanged("SidebarColor");
            }
        }
        private string _sidebarTextColor;
        /// <summary>
        /// the text color inside the sidebar of the sidebar lineup view
        /// </summary>
        public string SidebarTextColor
        {
            get { return _sidebarTextColor; }
            set
            {
                _sidebarTextColor = value;
                OnPropertyChanged("SidebarTextColor");
            }
        }
        private string _sidebarSkaterTextColor;
        /// <summary>
        /// the color of the skaters name in the line up view
        /// </summary>
        public string SidebarSkaterTextColor
        {
            get { return _sidebarSkaterTextColor; }
            set
            {
                _sidebarSkaterTextColor = value;
                OnPropertyChanged("SidebarSkaterTextColor");
            }
        }
        private string _sidebarBackgroundColor;
        /// <summary>
        /// the background color of the sidebar line up view
        /// </summary>
        public string SidebarBackgroundColor
        {
            get { return _sidebarBackgroundColor; }
            set
            {
                _sidebarBackgroundColor = value;
                OnPropertyChanged("SidebarBackgroundColor");
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
    }
}
