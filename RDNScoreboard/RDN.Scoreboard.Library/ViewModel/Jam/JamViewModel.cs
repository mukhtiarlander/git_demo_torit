using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.StopWatch;
using System.ComponentModel;
using RDN.Utilities.Error;

using Scoreboard.Library.ViewModel.Members;
using RDN.Utilities.Util;
using RDN.Portable.Classes.Games.Scoreboard;

namespace Scoreboard.Library.ViewModel
{
    public enum JamViewModelEnum { JamNumber, JammerT1, JammerT2, PivotT1, PivotT2, TeamLeadingJam, JamId }

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
            this.LeadJammers = new List<LeadJammerViewModel>();
            this.JamPasses = new List<Jam.JamPass>();
            this.JamId = Guid.NewGuid();
            this.JamNumber = jamNumber;
            this.GameTimeElapsedMillisecondsStart = currentGameTimeInMilliseconds;
            this.CurrentPeriod = currentPeriod;
            JamClock = new StopwatchWrapper(PolicyViewModel.Instance.JamClockTimePerJam);
        }

        private Guid _jamId;
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

        public Guid GameId { get; set; }
        public StopwatchWrapper JamClock { get; set; }
        public StopwatchWrapper LineUpClockAfterJam { get; set; }
        private TeamNumberEnum _teamLeadingJam;
        public TeamNumberEnum TeamLeadingJam
        {
            get
            {
                return _teamLeadingJam;
            }
            set
            {
                _teamLeadingJam = value;
                OnPropertyChanged("TeamLeadingJam");
            }
        }

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
        public TeamMembersViewModel Blocker4T1 { get; set; }
        /// <summary>
        /// used for displaying the points out to the user that watches the game from the website
        /// </summary>
        public int TotalPointsForJamT1 { get; set; }
        /// <summary>
        /// used for displaying the points out to the user that watches the game from the website
        /// </summary>
        public int TotalPointsForJamT2 { get; set; }

        public bool DidJamEndWithInjury { get; set; }
        /// <summary>
        /// jammer successfully called off jam.
        /// </summary>
        public bool DidJamGetCalledByJammerT1 { get; set; }
        public bool DidJamGetCalledByJammerT2 { get; set; }

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
        public TeamMembersViewModel Blocker4T2 { get; set; }

        public List<LeadJammerViewModel> LeadJammers { get; set; }
        public List<Jam.JamPass> JamPasses { get; set; }
        /// <summary>
        /// sets the lead jammer for the Jam.
        /// Also takes care if the Lead Jammer has changed as in MADE rules.
        /// </summary>
        /// <param name="jammerInLead"></param>
        /// <param name="gameTimeinMilliseconds"></param>
        /// <param name="jamTimeinMilliseconds"></param>
        public bool setLeadJammer(TeamMembersViewModel jammerInLead, long gameTimeinMilliseconds, TeamNumberEnum team)
        {
            try
            {
                //can only be lead if they are jammer or pivot
                if ((jammerInLead.IsPivot || jammerInLead.IsJammer) && !jammerInLead.IsInBox)
                {
                    if (LeadJammers == null)
                        LeadJammers = new List<LeadJammerViewModel>();

                    LeadJammerViewModel jamLead = new LeadJammerViewModel();
                    jamLead.Jammer = jammerInLead;
                    jamLead.JamTimeInMilliseconds = (long)TimeSpan.FromTicks(this.JamClock.TimeElapsed).TotalMilliseconds;
                    jamLead.GameTimeInMilliseconds = gameTimeinMilliseconds;
                    jamLead.Team = team;
                    jamLead.GameLeadJamId = LeadJammers.Count;
                    LeadJammers.Add(jamLead);

                    if (team == TeamNumberEnum.Team1)
                    {
                        //removes lead from this team 
                        //Object reference not set to an instance of an object. at 185
                        removeLeadJammer(team);
                        var skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == jammerInLead.SkaterId).FirstOrDefault();
                        if (skater != null)
                            skater.IsLeadJammer = true;
                        removeLeadJammer(TeamNumberEnum.Team2);
                        TeamLeadingJam = TeamNumberEnum.Team1;
                        if (JammerT1 != null && jammerInLead.SkaterId == JammerT1.SkaterId)
                            JammerT1.IsLeadJammer = true;
                        else if (PivotT1 != null && jammerInLead.SkaterId == PivotT1.SkaterId)
                            PivotT1.IsLeadJammer = true;

                    }
                    else if (team == TeamNumberEnum.Team2)
                    {
                        removeLeadJammer(team);
                        GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == jammerInLead.SkaterId).FirstOrDefault().IsLeadJammer = true;
                        removeLeadJammer(TeamNumberEnum.Team1);
                        TeamLeadingJam = TeamNumberEnum.Team2;

                        if (JammerT2 != null && jammerInLead.SkaterId == JammerT2.SkaterId)
                            JammerT2.IsLeadJammer = true;
                        else if (PivotT2 != null && jammerInLead.SkaterId == PivotT2.SkaterId)
                            PivotT2.IsLeadJammer = true;

                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return false;
        }
        public bool setLostLeadJammerEligibility(TeamMembersViewModel jammerInLead, long gameTimeinMilliseconds, TeamNumberEnum team)
        {
            try
            {
                if (team == TeamNumberEnum.Team1)
                {
                    if (JammerT1 != null && jammerInLead.SkaterId == JammerT1.SkaterId)
                        JammerT1.LostLeadJammerEligibility = true;
                    else if (PivotT1 != null && jammerInLead.SkaterId == PivotT1.SkaterId)
                        PivotT1.LostLeadJammerEligibility = true;

                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == jammerInLead.SkaterId).FirstOrDefault().LostLeadJammerEligibility = true;
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    if (JammerT2 != null && jammerInLead.SkaterId == JammerT2.SkaterId)
                        JammerT2.LostLeadJammerEligibility = true;
                    else if (PivotT2 != null && jammerInLead.SkaterId == PivotT2.SkaterId)
                        PivotT2.LostLeadJammerEligibility = true;

                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == jammerInLead.SkaterId).FirstOrDefault().LostLeadJammerEligibility = true;
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return false;
        }

        /// <summary>
        /// removes the lead jammer.
        /// </summary>
        /// <param name="team"></param>
        public void removeLeadJammer(TeamNumberEnum team)
        {
            try
            {
                if (team == TeamNumberEnum.Team1)
                {

                    for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Where(x => x.IsLeadJammer).Count(); i++)
                    {
                        GameViewModel.Instance.Team1.TeamMembers.Where(x => x.IsLeadJammer).FirstOrDefault().IsLeadJammer = false;
                    }
                    if (JammerT1 != null)
                        JammerT1.IsLeadJammer = false;
                    if (PivotT1 != null)
                        PivotT1.IsLeadJammer = false;
                }
                if (team == TeamNumberEnum.Team2)
                {
                    for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Where(x => x.IsLeadJammer).Count(); i++)
                    {
                        GameViewModel.Instance.Team2.TeamMembers.Where(x => x.IsLeadJammer).FirstOrDefault().IsLeadJammer = false;
                    }
                    if (JammerT2 != null)
                        JammerT2.IsLeadJammer = false;
                    if (PivotT2 != null)
                        PivotT2.IsLeadJammer = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// sets number 1 blocker for the team.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setBlocker1(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
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
                    Blocker1T1 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker1 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
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
                    Blocker1T2 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker1 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// sets the 2nd blocker for the jam
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setBlocker2(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
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
                    Blocker2T1 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker2 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
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
                    Blocker2T2 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker2 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the rd blocker for the jam
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setBlocker3(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
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
                    Blocker3T1 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker3 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
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
                    Blocker3T2 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker3 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public void setBlocker4(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
            {
                clearSkaterPositions(skater, team);
                if (team == TeamNumberEnum.Team1)
                {
                    for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                    {
                        if (GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker4)
                        {
                            GameViewModel.Instance.Team1.TeamMembers[i].IsBenched = true;
                            GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker4 = false;
                        }
                    }
                    Blocker4T1 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker4 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker4 = true;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                    {
                        if (GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker4)
                        {
                            GameViewModel.Instance.Team2.TeamMembers[i].IsBenched = true;
                            GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker4 = false;
                        }
                    }
                    Blocker4T2 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsBlocker4 = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker4 = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the jammer for the jam.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setJammer(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
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
                    JammerT1 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsJammer = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
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
                    JammerT2 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsJammer = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// sets the pivot for the jam.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="team"></param>
        public void setPivot(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
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
                    PivotT1 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsPivot = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
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
                    PivotT2 = new TeamMembersViewModel() { SkaterName = skater.SkaterName, IsPivot = true, SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, SkaterNumber = skater.SkaterNumber, SkaterPictureLocation = skater.SkaterPictureLocation };
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// benches all the skaters in the team.
        /// </summary>
        /// <param name="team"></param>
        public void benchAllSkaters(TeamNumberEnum team)
        {
            try
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
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public void clearSkaterPositions(TeamMembersViewModel skater, TeamNumberEnum team)
        {
            try
            {
                //if in the box, it removes the player from the box.
                GameViewModel.Instance.removeSkaterFromPenaltyBox(skater, team);
                if (team == TeamNumberEnum.Team1)
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
                    if (Blocker4T1 == skater)
                        Blocker4T1 = null;

                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker4 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    if (PivotT2 == skater)
                        PivotT2 = null;
                    if (JammerT2 == skater)
                        JammerT2 = null;
                    if (Blocker1T2 == skater)
                        Blocker1T2 = null;
                    if (Blocker2T2 == skater)
                        Blocker2T2 = null;
                    if (Blocker3T2 == skater)
                        Blocker3T2 = null;
                    if (Blocker4T2 == skater)
                        Blocker4T2 = null;

                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker4 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;

                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        /// <summary>
        /// handles all logic to bench the skater for the game.
        /// </summary>
        /// <param name="skater"></param>
        /// <param name="teamNumber"></param>
        public void benchSkater(TeamMembersViewModel skater, TeamNumberEnum teamNumber)
        {
            try
            {
                if (teamNumber == TeamNumberEnum.Team1)
                {
                    if (Blocker1T1 != null && Blocker1T1.SkaterId == skater.SkaterId)
                        Blocker1T1 = null;
                    if (Blocker2T1 != null && Blocker2T1.SkaterId == skater.SkaterId)
                        Blocker2T1 = null;
                    if (Blocker3T1 != null && Blocker3T1.SkaterId == skater.SkaterId)
                        Blocker3T1 = null;
                    if (Blocker4T1 != null && Blocker4T1.SkaterId == skater.SkaterId)
                        Blocker4T1 = null;
                    if (PivotT1 != null && PivotT1.SkaterId == skater.SkaterId)
                        PivotT1 = null;
                    if (JammerT1 != null && JammerT1.SkaterId == skater.SkaterId)
                        JammerT1 = null;

                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = true;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                    GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker4 = false;
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
                    if (Blocker4T2 != null && Blocker4T2.SkaterId == skater.SkaterId)
                        Blocker4T2 = null;
                    if (PivotT2 != null && PivotT2.SkaterId == skater.SkaterId)
                        PivotT2 = null;
                    if (JammerT2 != null && JammerT2.SkaterId == skater.SkaterId)
                        JammerT2 = null;

                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBenched = true;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker1 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker2 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker3 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsBlocker4 = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsInBox = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsJammer = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsLeadJammer = false;
                    GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skater.SkaterId).FirstOrDefault().IsPivot = false;
                }
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// updates the latest pass with another point.
        /// </summary>
        /// <param name="skaterId"></param>
        /// <param name="score"></param>
        public void UpdateLatestJamPassScore(Guid skaterId, int score)
        {
            try
            {
                var lastPass = this.JamPasses.Where(x => x.SkaterWhoPassed.SkaterId == skaterId).OrderByDescending(x => x.PassNumber).FirstOrDefault();
                if (lastPass != null)
                    lastPass.AddPointsForPass(score);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public void AddJamPass(Jam.JamPass pass)
        {
            try
            {
                int passNumber = 1;
                var lastPass = this.JamPasses.Where(x => x.Team == pass.Team).OrderByDescending(x => x.PassNumber).FirstOrDefault();
                if (lastPass != null)
                {
                    passNumber = lastPass.PassNumber;
                    passNumber += 1;
                }
                pass.SetPassNumber(passNumber);
                this.JamPasses.Add(pass);
            }
            catch (Exception e)
            {
                ErrorViewModel.Save(e, this.GetType(), ErrorGroupEnum.UI, additionalInformation: Logger.Instance.getLoggedMessages());
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
