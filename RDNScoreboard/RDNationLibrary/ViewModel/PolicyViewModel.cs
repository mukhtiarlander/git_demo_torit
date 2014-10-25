using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

using RDNationLibrary.Static;
using RDNationLibrary.Static.Enums;
using System.ComponentModel;

namespace RDNationLibrary.ViewModel
{
    public class PolicyViewModel : INotifyPropertyChanged
    {
        static PolicyViewModel instance = new PolicyViewModel();

        public static PolicyViewModel Instance
        {
            get
            {
                return instance;
            }
        }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PolicyViewModel()
        { }

        PolicyViewModel()
        { }

        /// <summary>
        /// Countdown Clock Controls Start of Jam: Starts the Jam automatically when the time between jams has ended.
        /// </summary>
        public bool LineupClockControlsStartJam { get; set; }
        /// <summary>
        /// Enable Intermission Start Clock: Starts the intermission clock to the default or designated intermission time.
        /// </summary>
        public bool EnableIntermissionStartOfClock { get; set; }
        /// <summary>
        /// Set Intermission Time: Sets the time for intermission
        /// </summary>
        public long IntermissionStartOfClockInMilliseconds { get; set; }
        /// <summary>
        /// Enable Intermission Stop Clock: When the Intermission clock is over, this resets the Period and Jam clock time.
        /// </summary>
        public bool IntermissionStopClockEnable { get; set; }
        /// <summary>
        /// Increment Jam Number At Stop of Intermission: Increments the jam number automatically at stop of intermission.
        /// </summary>
        public bool IntermissionStopClockIncrementJamNumber { get; set; }
        /// <summary>
        /// Reset Jam Number At Stop of Intermission: Resets the jam number to zero automatically at stop of intermission.
        /// </summary>
        public bool IntermissionStopClockResetJamNumber { get; set; }
        /// <summary>
        /// Reset Jam Time At Stop of Intermission: Resets the jam Time to zero automatically at stop of intermission.
        /// </summary>
        public bool IntermissionStopClockResetJamTime { get; set; }
        /// <summary>
        /// Increment Period Number At Stop of Intermission Clock.
        /// </summary>
        public bool IntermissionStopClockIncrementPeriodNumber { get; set; }
        /// <summary>
        /// Reset Period Number At Stop of Intermission Clock.
        /// </summary>
        public bool IntermissionStopClockResetPeriodNumber { get; set; }
        /// <summary>
        /// Reset Period Time At Stop of Intermission Clock.
        /// </summary>
        public bool IntermissionStopClockResetPeriodTime { get; set; }
        /// <summary>
        /// Starts the line up clock when the jam clock stops.  Starts the Jam clock when the line up clock stops.
        /// </summary>
        public bool JamClockControlsLineUpClock { get; set; }
        /// <summary>
        /// This clears all Team Positions (who are not in the Penalty Box) when the Jam clock is stopped, sets all Skaters to Not Lead Jammer, and sets the Team to Not Lead Jammer.
        /// </summary>
        public bool JamClockControlsTeamPositions { get; set; }
        /// <summary>
        /// This removes Lead Jammer from any Skater sent to the Penalty Box.
        /// </summary>
        public bool PenaltyBoxControlsLeadJammer { get; set; }
        /// <summary>
        /// This controls the Lineup clock based on the Period clock. When the Period clock stops and its time is equal to its 0 (i.e. its minimum), and the Jam clock is also stopped, the Lineup clock is stopped and reset.
        /// </summary>
        public bool PeriodClockControlsLineupJamClock { get; set; }
        /// <summary>
        /// Enables the Ad change policy on the board
        /// </summary>
        public bool EnableAdChange { get; set; }
        /// <summary>
        /// Shows the adds where the line up clock sits.
        /// </summary>
        public bool AdChangeUseLineUpClock { get; set; }
        /// <summary>
        /// Ads change at the seconds specified
        /// </summary>
        public long AdChangeDisplayChangesInMilliSeconds { get; set; }
        /// <summary>
        /// Automatically change the ad image
        /// </summary>
        public bool AdChangeAutomaticallyChangeImage { get; set; }
        /// <summary>
        /// Show Ads during intermission
        /// </summary>
        public bool AdChangeShowAdsDuringIntermission { get; set; }
        /// <summary>
        /// Show Ads in Random Order
        /// </summary>
        public bool AdChangeShowAdsRandomly { get; set; }
        /// <summary>
        /// Always Show Jam Clock
        /// </summary>
        public bool AlwaysShowJamClock { get; set; }
        /// <summary>
        /// This controls the Lineup clock based on the Timeout clock. When the Timeout clock starts, the Lineup clock is stopped then reset.
        /// </summary>
        public bool TimeoutClockControlsLineupClock { get; set; }
        /// <summary>
        /// Enable the intermission naming scheme.
        /// </summary>
        public bool EnableIntermissionNaming { get; set; }
        /// <summary>
        /// Hides the clocks after Bout is over.
        /// </summary>
        public bool HideClockTimeAfterBout { get; set; }

        /// <summary>
        /// Name of the First Intermission
        /// </summary>
        public string FirstIntermissionNameText { get; set; }
        /// <summary>
        /// Intermission Name after Confirmed Points
        /// </summary>
        public string FirstIntermissionNameConfirmedText { get; set; }
        /// <summary>
        /// Name of Second Intermission 
        /// </summary>
        public string SecondIntermissionNameText { get; set; }
        /// <summary>
        /// Name of Second Intermission Confirmed
        /// </summary>
        public string SecondIntermissionNameConfirmedText { get; set; }
        /// <summary>
        /// some other text used for intermission
        /// </summary>
        public string IntermissionOtherText { get; set; }
        /// <summary>
        /// Jam Clock Time.
        /// </summary>
        public long JamClockTimePerJam { get; set; }
        public long LineUpClockPerJam { get; set; }
        /// <summary>
        /// period clock in milliseconds
        /// </summary>
        public long PeriodClock { get; set; }
        /// <summary>
        /// total number of periods per game
        /// </summary>
        public int NumberOfPeriods { get; set; }
        /// <summary>
        /// number of time outs per period
        /// </summary>
        public int TimeOutsPerPeriod { get; set; }
        /// <summary>
        /// time out clock in milliseconds
        /// </summary>
        public long TimeOutClock { get; set; }

        private string _gameSelectionType;
        public string GameSelectionType
        {
            get { return _gameSelectionType; }
            set
            {
                _gameSelectionType = value;
                OnPropertyChanged("GameSelectionType");
            }
        }
        /// <summary>
        /// makes the proper game policy changes to the game
        /// </summary>
        /// <param name="gameType"></param>
        public void changeGameSelectionType(GameTypeEnum gameType)
        {
            GameSelectionType = gameType.ToString();
            if (gameType == GameTypeEnum.MADE)
            {
                NumberOfPeriods = 4;
                PeriodClock = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                TimeOutsPerPeriod = 3;
            }
            else if (gameType == GameTypeEnum.MADE_COED)
            {
                NumberOfPeriods = 8;
                PeriodClock = (long)TimeSpan.FromMinutes(10).TotalMilliseconds;
                JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                TimeOutsPerPeriod = 3;

            }
            else if (gameType == GameTypeEnum.WFTDA)
            {
                NumberOfPeriods = 2;
                PeriodClock = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;
                JamClockTimePerJam = (long)TimeSpan.FromMinutes(2).TotalMilliseconds;
                LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                TimeOutsPerPeriod = 2;
            }
            else if (gameType == GameTypeEnum.RENEGADE)
            {//TODO: find renenegade rule set
            }
            else if (gameType == GameTypeEnum.OSDA)
            {
                NumberOfPeriods = 4;
                PeriodClock = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
                JamClockTimePerJam = (long)TimeSpan.FromMinutes(90).TotalMilliseconds;
                LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                TimeOutsPerPeriod = 1;
            }
            else if (gameType == GameTypeEnum.OSDA_COED)
            {
                NumberOfPeriods = 8;
                PeriodClock = (long)TimeSpan.FromMinutes(10).TotalMilliseconds;
                JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
                LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
                TimeOutsPerPeriod = 1;
            }
        }

        public void loadFromXmlPolicy()
        {
            XmlSerializer xs = new XmlSerializer(typeof(PolicyViewModel));

            //loads policy from a saved settings file.
            if (File.Exists(Config.SAVE_POLICY_FILE))
            {
                StreamReader objReader = new StreamReader(Config.SAVE_POLICY_FILE);
                PolicyViewModel policy = (PolicyViewModel)xs.Deserialize(objReader);

                AdChangeAutomaticallyChangeImage = policy.AdChangeAutomaticallyChangeImage;
                AdChangeDisplayChangesInMilliSeconds = policy.AdChangeDisplayChangesInMilliSeconds;
                AdChangeShowAdsDuringIntermission = policy.AdChangeShowAdsDuringIntermission;
                AdChangeShowAdsRandomly = policy.AdChangeShowAdsRandomly;
                AdChangeUseLineUpClock = policy.AdChangeUseLineUpClock;
                AlwaysShowJamClock = policy.AlwaysShowJamClock;
                LineupClockControlsStartJam = policy.LineupClockControlsStartJam;
                EnableAdChange = policy.EnableAdChange;
                EnableIntermissionNaming = policy.EnableIntermissionNaming;
                EnableIntermissionStartOfClock = policy.EnableIntermissionStartOfClock;
                FirstIntermissionNameConfirmedText = policy.FirstIntermissionNameConfirmedText;
                FirstIntermissionNameText = policy.FirstIntermissionNameText;
                HideClockTimeAfterBout = policy.HideClockTimeAfterBout;
                IntermissionOtherText = policy.IntermissionOtherText;
                IntermissionStartOfClockInMilliseconds = policy.IntermissionStartOfClockInMilliseconds;
                IntermissionStopClockEnable = policy.IntermissionStopClockEnable;
                IntermissionStopClockIncrementJamNumber = policy.IntermissionStopClockIncrementJamNumber;
                IntermissionStopClockIncrementPeriodNumber = policy.IntermissionStopClockIncrementPeriodNumber;
                IntermissionStopClockResetJamNumber = policy.IntermissionStopClockResetJamNumber;
                IntermissionStopClockResetJamTime = policy.IntermissionStopClockResetJamTime;
                IntermissionStopClockResetPeriodNumber = policy.IntermissionStopClockResetPeriodNumber;
                IntermissionStopClockResetPeriodTime = policy.IntermissionStopClockResetPeriodTime;
                JamClockControlsLineUpClock = policy.JamClockControlsLineUpClock;
                JamClockControlsTeamPositions = policy.JamClockControlsTeamPositions;
                PenaltyBoxControlsLeadJammer = policy.PenaltyBoxControlsLeadJammer;
                PeriodClockControlsLineupJamClock = policy.PeriodClockControlsLineupJamClock;
                SecondIntermissionNameConfirmedText = policy.SecondIntermissionNameConfirmedText;
                SecondIntermissionNameText = policy.SecondIntermissionNameText;
                TimeoutClockControlsLineupClock = policy.TimeoutClockControlsLineupClock;
                JamClockTimePerJam = policy.JamClockTimePerJam;
                LineUpClockPerJam = policy.LineUpClockPerJam;
                PeriodClock = policy.PeriodClock;
                NumberOfPeriods = policy.NumberOfPeriods;
                GameSelectionType = policy.GameSelectionType;
                TimeOutsPerPeriod = policy.TimeOutsPerPeriod;
                TimeOutClock = policy.TimeOutClock;
                objReader.Close();
            }
            else
            {
                SetDefaultPolicySettings();

            }
        }
        /// <summary>
        /// sets the default settings of the scoreboard if there are any current settings saved.
        /// </summary>
        private void SetDefaultPolicySettings()
        {
            //loads defaults for loading the policy for the first time.
            AdChangeAutomaticallyChangeImage = true;
            //TODO: place 5 seconds somewhere else in default
            AdChangeDisplayChangesInMilliSeconds = (long)TimeSpan.FromSeconds(5).TotalMilliseconds;
            AdChangeShowAdsDuringIntermission = false;
            AdChangeShowAdsRandomly = true;
            AdChangeUseLineUpClock = true;
            AlwaysShowJamClock = true;
            LineupClockControlsStartJam = true;
            EnableAdChange = true;
            EnableIntermissionNaming = true;
            EnableIntermissionStartOfClock = true;
            //TODO: place this text in the correct place when we work on languages.
            FirstIntermissionNameConfirmedText = "Halftime";
            FirstIntermissionNameText = "Halftime";
            HideClockTimeAfterBout = true;
            IntermissionOtherText = "Time To Derby";
            IntermissionStartOfClockInMilliseconds = (long)TimeSpan.FromMinutes(15).TotalMilliseconds;
            IntermissionStopClockEnable = true;
            IntermissionStopClockIncrementJamNumber = true;
            IntermissionStopClockIncrementPeriodNumber = true;
            IntermissionStopClockResetJamNumber = false;
            IntermissionStopClockResetJamTime = true;
            IntermissionStopClockResetPeriodNumber = false;
            IntermissionStopClockResetPeriodTime = true;
            JamClockControlsLineUpClock = true;
            JamClockControlsTeamPositions = true;
            PenaltyBoxControlsLeadJammer = true;
            PeriodClockControlsLineupJamClock = true;
            SecondIntermissionNameConfirmedText = "Final";
            SecondIntermissionNameText = "Unofficial Final";
            TimeoutClockControlsLineupClock = true;
            JamClockTimePerJam = (long)TimeSpan.FromSeconds(90).TotalMilliseconds;
            LineUpClockPerJam = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;
            PeriodClock = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;
            NumberOfPeriods = 4;
            GameSelectionType = GameTypeEnum.MADE.ToString();
            TimeOutClock = (long)TimeSpan.FromSeconds(60).TotalMilliseconds;

            changeGameSelectionType(GameTypeEnum.MADE);
        }


        /// <summary>
        /// 
        /// </summary>
        public void savePolicyToXml()
        {
            var saveGameTask = Task<bool>.Factory.StartNew(
                () =>
                {
                    System.Xml.Serialization.XmlSerializer writer =
                       new System.Xml.Serialization.XmlSerializer(Instance.GetType());
                    System.IO.StreamWriter file =
                       new System.IO.StreamWriter(Config.SAVE_POLICY_FILE);

                    writer.Serialize(file, Instance);
                    file.Close();
                    return true;
                });

            if (saveGameTask.Result)
            {

            }
        }

        public string GetStringValue(string key)
        {
            var doc = XDocument.Load("dic");
            return doc.Element("settings") == null || doc.Element("settings").Element(key) == null ? string.Empty : doc.Element("settings").Element(key).Value;
        }

        public bool GetBoolValue(string key)
        {
            var doc = XDocument.Load("dic");
            return Convert.ToBoolean(doc.Element("settings") == null || doc.Element("settings").Element(key) == null ? "False" : doc.Element("settings").Element(key).Value);
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
