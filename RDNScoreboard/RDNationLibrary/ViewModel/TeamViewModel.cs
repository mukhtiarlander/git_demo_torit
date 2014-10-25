using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace RDNationLibrary.ViewModel
{
    public enum TeamViewModelEnum { TeamName, TimeOutsLeft, LogoLocation }
    public class TeamViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TeamViewModel()
        { }

        private string  _logoLocation;
        public string LogoLocation
        {
            get { return _logoLocation; }
            set
            {
                _logoLocation = value;
                OnPropertyChanged("LogoLocation");
            }
        }
        private string _teamName;
        public string TeamName
        {
            get { return _teamName; }
            set
            {
                _teamName = value;
                OnPropertyChanged("TeamName");
            }
        }
        public Guid TeamId { get; set; }
        private ObservableCollection<TeamMembersViewModel> _teamMembers = new ObservableCollection<TeamMembersViewModel>();

        public ObservableCollection<TeamMembersViewModel> TeamMembers
        {
            get { return _teamMembers; }
            set { _teamMembers = value; }
        }

        public void clearTeamPositions()
        {
            for (int i = 0; i < _teamMembers.Count; i++)
            {
                _teamMembers[i].IsBenched = true;
                _teamMembers[i].IsBlocker1 = false;
                _teamMembers[i].IsBlocker2 = false;
                _teamMembers[i].IsBlocker3 = false;
                _teamMembers[i].IsJammer = false;
                _teamMembers[i].IsPivot = false;
                _teamMembers[i].IsLeadJammer = false;
            }
        }
        private int _timeOutsLeft;
        public int TimeOutsLeft
        {
            get { return _timeOutsLeft; }
            set
            {
                _timeOutsLeft = value;
                OnPropertyChanged("TimeOutsLeft");
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
