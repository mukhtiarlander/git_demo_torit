using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Scoreboard.Library.Models;
using RDN.Portable.Classes.Team;

namespace Scoreboard.Library.ViewModel.Members
{
    public enum TeamViewModelEnum { TeamName, TimeOutsLeft, Logo, LeagueName }
    public class TeamViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TeamViewModel()
        { }

        private TeamLogo _logo {get;set;}
        public TeamLogo Logo
        {
            get { return _logo; }
            set
            {
                _logo = value;
                OnPropertyChanged("Logo");
            }
        }

      
        public LineUpViewModel LineUpSettings { get; set; }

        private string _leagueName;
        public string LeagueName
        {
            get { return _leagueName; }
            set
            {
                _leagueName = value;
                OnPropertyChanged("LeagueName");
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
        public int SeedRating { get; set; }
        public int PoolNumber { get; set; }
        public Guid TeamId { get; set; }
        public Guid TeamLinkId { get; set; }
        public long Score { get; set; }
        private ObservableCollection<TeamMembersViewModel> _teamMembers = new ObservableCollection<TeamMembersViewModel>();

        public ObservableCollection<TeamMembersViewModel> TeamMembers
        {
            get { return _teamMembers; }
            set { _teamMembers = value; }
        }
        /// <summary>
        /// clears all the team positions.
        /// </summary>
        public void clearTeamPositions()
        {
            for (int i = 0; i < _teamMembers.Count; i++)
            {
                _teamMembers[i].IsBenched = true;
                _teamMembers[i].IsBlocker1 = false;
                _teamMembers[i].IsBlocker2 = false;
                _teamMembers[i].IsBlocker3 = false;
                _teamMembers[i].IsBlocker4 = false;
                _teamMembers[i].IsJammer = false;
                _teamMembers[i].IsPivot = false;
                _teamMembers[i].IsLeadJammer = false;
                _teamMembers[i].LostLeadJammerEligibility = false;
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
