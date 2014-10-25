using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RDNationLibrary.Static.Enums;
using RDNationLibrary.StopWatch;
using System.ComponentModel;

namespace RDNationLibrary.ViewModel
{
    public enum JamViewModelEnum { JamNumber, JammerT1, JammerT2, PivotT1, PivotT2 }

    /// <summary>
    /// Jam View controls the contents for each and every jam.
    /// </summary>
    public class JamViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// dummy contructor used to export game to xml
        /// </summary>
        public JamViewModel() { }
        public JamViewModel(int jamNumber, long currentGameTimeInMilliseconds, int currentPeriod)
        {
            this.JamNumber = jamNumber;
            this.GameTimeElapsedMillisecondsStart = currentGameTimeInMilliseconds;
            this.CurrentPeriod = currentPeriod;
            JamClock= new StopwatchWrapper(PolicyViewModel.Instance.JamClockTimePerJam);
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


        public StopwatchWrapper JamClock { get; set; }
        public StopwatchWrapper LineUpClockAfterJam { get; set; }


        public long GameTimeElapsedMillisecondsStart { get; set; }
        public long GameTimeElapsedMillisecondsEnd { get; set; }
        public int CurrentPeriod { get; set; }

        private TeamMembersViewModel _jammerT1;
        public TeamMembersViewModel JammerT1
        {
            get
            {
                return _jammerT1;
            }
            set
            {
                _jammerT1 = value;
                OnPropertyChanged("JammerT1");
            }
        }

        private TeamMembersViewModel _pivotT1;
        public TeamMembersViewModel PivotT1
        {
            get
            {
                return _pivotT1;
            }
            set
            {
                _pivotT1 = value;
                OnPropertyChanged("PivotT1");
            }
        }
        public TeamMembersViewModel Blocker1T1 { get; set; }
        public TeamMembersViewModel Blocker2T1 { get; set; }
        public TeamMembersViewModel Blocker3T1 { get; set; }

        private TeamMembersViewModel _jammerT2;
        public TeamMembersViewModel JammerT2
        {
            get
            {
                return _jammerT2;
            }
            set
            {
                _jammerT2 = value;
                OnPropertyChanged("JammerT2");
            }
        }

        private TeamMembersViewModel _pivotT2;
        public TeamMembersViewModel PivotT2
        {
            get
            {
                return _pivotT2;
            }
            set
            {
                _pivotT2 = value;
                OnPropertyChanged("PivotT2");
            }
        }
        public TeamMembersViewModel Blocker1T2 { get; set; }
        public TeamMembersViewModel Blocker2T2 { get; set; }
        public TeamMembersViewModel Blocker3T2 { get; set; }

        public List<LeadJammerViewModel> LeadJammers { get; set; }
        /// <summary>
        /// sets the lead jammer for the Jam.
        /// Also takes care if the Lead Jammer has changed as in MADE rules.
        /// </summary>
        /// <param name="jammerInLead"></param>
        /// <param name="gameTimeinMilliseconds"></param>
        /// <param name="jamTimeinMilliseconds"></param>
        public bool setLeadJammer(TeamMembersViewModel jammerInLead, long gameTimeinMilliseconds, TeamNumberEnum team)
        {
            //can only be lead if they are jammer or pivot
            if ((jammerInLead.IsPivot | jammerInLead.IsJammer) && !jammerInLead.IsInBox)
            {
                if (LeadJammers == null)
                    LeadJammers = new List<LeadJammerViewModel>();

                LeadJammerViewModel jamLead = new LeadJammerViewModel();
                jamLead.Jammer = jammerInLead;
                jamLead.JamTimeInMilliseconds = (long)TimeSpan.FromTicks(this.JamClock.TimeElapsed).TotalMilliseconds;
                jamLead.GameTimeInMilliseconds = gameTimeinMilliseconds;

                LeadJammers.Add(jamLead);

                if (team == TeamNumberEnum.Team1)
                {
                    //removes lead from this team 
                    removeLeadJammer(team);
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == jammerInLead.SkaterId).FirstOrDefault().IsLeadJammer = true;
                    removeLeadJammer(TeamNumberEnum.Team2);
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    removeLeadJammer(team);
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == jammerInLead.SkaterId).FirstOrDefault().IsLeadJammer = true;
                    removeLeadJammer(TeamNumberEnum.Team1);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// removes the lead jammer.
        /// </summary>
        /// <param name="team"></param>
        public void removeLeadJammer(TeamNumberEnum team)
        {
            if (team == TeamNumberEnum.Team1)
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Where(x => x.IsLeadJammer).Count(); i++)
                {
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.IsLeadJammer).FirstOrDefault().IsLeadJammer = false;
                }
            if (team == TeamNumberEnum.Team2)
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Where(x => x.IsLeadJammer).Count(); i++)
                {
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.IsLeadJammer).FirstOrDefault().IsLeadJammer = false;
                }

        }

        /// <summary>
        /// sets number 1 blocker for the team.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setBlocker1(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            clearSkaterPositions(skater, team);
            if (team == TeamNumberEnum.Team1)
            {
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker1)
                    {
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker1 = false;
                    }
                }
                Blocker1T1 = skater;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = true;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker1)
                    {
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker1 = false;
                    }
                }
                Blocker1T2 = skater;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = true;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
        }
        /// <summary>
        /// sets the 2nd blocker for the jam
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setBlocker2(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            clearSkaterPositions(skater, team);
            if (team == TeamNumberEnum.Team1)
            {
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker2)
                    {
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker2 = false;
                    }
                }
                Blocker2T1 = skater;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = true;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker2)
                    {
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker2 = false;
                    }
                }
                Blocker2T2 = skater;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = true;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
        }
        /// <summary>
        /// sets the rd blocker for the jam
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setBlocker3(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            clearSkaterPositions(skater, team);
            if (team == TeamNumberEnum.Team1)
            {
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker3)
                    {
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker3 = false;
                    }
                }
                Blocker3T1 = skater;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = true;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker3)
                    {
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker3 = false;
                    }
                }
                Blocker3T2 = skater;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = true;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
        }
        /// <summary>
        /// sets the jammer for the jam.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setJammer(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            clearSkaterPositions(skater, team);
            if (team == TeamNumberEnum.Team1)
            {
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team1.TeamMembers[i].IsJammer)
                    {
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team1.TeamMembers[i].IsJammer = false;
                    }
                }
                JammerT1 = skater;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = true;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team2.TeamMembers[i].IsJammer)
                    {
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team2.TeamMembers[i].IsJammer = false;
                    }
                }
                JammerT2 = skater;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = true;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
        }
        /// <summary>
        /// sets the pivot for the jam.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setPivot(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            clearSkaterPositions(skater, team);
            if (team == TeamNumberEnum.Team1)
            {
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team1.TeamMembers[i].IsPivot)
                    {
                        GameViewModel.Instance.Team1.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team1.TeamMembers[i].IsPivot = false;
                    }
                }
                PivotT1 = skater;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = true;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    if (GameViewModel.Instance.Team2.TeamMembers[i].IsPivot)
                    {
                        GameViewModel.Instance.Team2.TeamMembers[i].IsBenched = true;
                        GameViewModel.Instance.Team2.TeamMembers[i].IsPivot = false;
                    }
                }
                PivotT2 = skater;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = true;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
            }
        }
        /// <summary>
        /// benches all the skaters in the team.
        /// </summary>
        /// <param name="team"></param>
        public void benchAllSkaters(TeamNumberEnum team)
        {
            if (team == TeamNumberEnum.Team1)
            {
                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    benchSkater(GameViewModel.Instance.Team1.TeamMembers[i], TeamNumberEnum.Team1);
                }
            }
            else if (team == TeamNumberEnum.Team2)
            {
                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    benchSkater(GameViewModel.Instance.Team2.TeamMembers[i], TeamNumberEnum.Team2);
                }
            }
        }

        public void clearSkaterPositions(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            if (team == TeamNumberEnum.Team1)
            {
                if (PivotT1 == skater)
                    PivotT1 = null;
                if (JammerT1 == skater)
                    JammerT1 = null;
                if (Blocker1T1 == skater)
                    Blocker1T1= null;
                if (Blocker2T1 == skater)
                    Blocker2T1= null;
                if (Blocker3T1== skater)
                    Blocker3T1= null;
                               
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;
            }
            else if (team == TeamNumberEnum.Team2)
            {
                if (PivotT1 == skater)
                    PivotT1 = null;
                if (JammerT1 == skater)
                    JammerT1 = null;
                if (Blocker1T1 == skater)
                    Blocker1T1 = null;
                if (Blocker2T1 == skater)
                    Blocker2T1 = null;
                if (Blocker3T1 == skater)
                    Blocker3T1 = null;
                
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;

            }
        }

        /// <summary>
        /// handles all logic to bench the skater for the game.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="teamNumber"></param>
        public void benchSkater(TeamMembersViewModel skater, TeamNumberEnum teamNumber)
        {
            if (teamNumber == TeamNumberEnum.Team1)
            {
                if (Blocker1T1 != null && Blocker1T1.SkaterId == skater.SkaterId)
                    Blocker1T1 = null;
                if (Blocker2T1 != null && Blocker2T1.SkaterId == skater.SkaterId)
                    Blocker2T1 = null;
                if (Blocker3T1 != null && Blocker3T1.SkaterId == skater.SkaterId)
                    Blocker3T1 = null;
                if (PivotT1 != null && PivotT1.SkaterId == skater.SkaterId)
                    PivotT1 = null;
                if (JammerT1 != null && JammerT1.SkaterId == skater.SkaterId)
                    JammerT1 = null;

                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = true;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;
            }
            else if (teamNumber == TeamNumberEnum.Team2)
            {
                if (Blocker1T2 != null && Blocker1T2.SkaterId == skater.SkaterId)
                    Blocker1T2 = null;
                if (Blocker2T2 != null && Blocker2T2.SkaterId == skater.SkaterId)
                    Blocker2T2 = null;
                if (Blocker3T2 != null && Blocker3T2.SkaterId == skater.SkaterId)
                    Blocker3T2 = null;
                if (PivotT2 != null && PivotT2.SkaterId == skater.SkaterId)
                    PivotT2 = null;
                if (JammerT2 != null && JammerT2.SkaterId == skater.SkaterId)
                    JammerT2 = null;

                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = true;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;
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
