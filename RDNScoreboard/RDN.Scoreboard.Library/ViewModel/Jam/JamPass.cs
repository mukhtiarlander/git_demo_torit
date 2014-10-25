using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel.Jam
{
    public enum JamPassEnum { JamNumber, SkaterWhoPassed, JamTimeMilliseconds, Team, PointsScoredForPass, JamId, PassId, PassNumber }
    public class JamPass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// dummy contructor used to export game to xml
        /// </summary>
        public JamPass() { }
        public JamPass(int jamNumber, long jamTimeMilliseconds, TeamMembersViewModel skaterWhoPassed, Guid jamId, TeamNumberEnum team)
        {
            this.PassId = Guid.NewGuid();
            this.JamNumber = JamNumber;
            this.SkaterWhoPassed = skaterWhoPassed;
            this.JamId = jamId;
            this.JamTimeMilliseconds = jamTimeMilliseconds;
            this.Team = team;
        }

        public JamPass SetPassNumber(int pass)
        {
            this.PassNumber = pass;
            return this;
        }
        public JamPass AddPointsForPass(int points)
        {
            this.PointsScoredForPass += points;
            return this;
        }

        private int _passNumber { get; set; }
        public int PassNumber
        {
            get
            {
                return _passNumber;
            }
            set
            {
                _passNumber = value;
                OnPropertyChanged("PassNumber");
            }
        }

        private Guid _passId { get; set; }
        public Guid PassId
        {
            get
            {
                return _passId;
            }
            set
            {
                _passId = value;
                OnPropertyChanged("PassId");
            }
        }

        private Guid _jamId { get; set; }
        public Guid JamId
        {
            get
            {
                return _jamId;
            }
            set
            {
                _jamId = value;
                OnPropertyChanged("JamId");
            }
        }

        private int _pointsScoredForPass { get; set; }
        public int PointsScoredForPass
        {
            get
            {
                return _pointsScoredForPass;
            }
            set
            {
                _pointsScoredForPass = value;
                OnPropertyChanged("PointsScoredForPass");
            }
        }

        private long _passJamTimeMilliseconds { get; set; }
        public long JamTimeMilliseconds
        {
            get
            {
                return _passJamTimeMilliseconds;
            }
            set
            {
                _passJamTimeMilliseconds = value;
                OnPropertyChanged("JamTimeMilliseconds");
            }
        }

        private TeamMembersViewModel _skaterWhoPassed { get; set; }
        public TeamMembersViewModel SkaterWhoPassed
        {
            get
            {
                return _skaterWhoPassed;
            }
            set
            {
                _skaterWhoPassed = value;
                OnPropertyChanged("SkaterWhoPassed");
            }
        }

        private int _jamNumber;
        public int JamNumber
        {
            get
            {
                return _jamNumber;
            }
            set
            {
                _jamNumber = value;
                OnPropertyChanged("JamNumber");
            }
        }

        private TeamNumberEnum _team;
        public TeamNumberEnum Team
        {
            get
            {
                return _team;
            }
            set
            {
                _team = value;
                OnPropertyChanged("Team");
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
