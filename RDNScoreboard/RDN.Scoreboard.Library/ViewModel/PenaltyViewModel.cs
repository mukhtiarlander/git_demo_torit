using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel.Members;

namespace Scoreboard.Library.ViewModel
{
    public class PenaltyViewModel
    {
        /// <summary>
        /// dummy constructor for serialization, do not touch.
        /// </summary>
        public PenaltyViewModel()
        { }

        public PenaltyViewModel(PenaltiesEnum penaltyType)
        {
            this.PenaltyType = penaltyType;
        }
        /// <summary>
        /// adding a penalty from the scoreboard.
        /// </summary>
        /// <param name="penaltyType"></param>
        /// <param name="penaltyAgainst"></param>
        /// <param name="periodTimeRemaining"></param>
        /// <param name="jamTimeInMilliseconds"></param>
        /// <param name="currentJam"></param>
        /// <param name="period"></param>
        public PenaltyViewModel(PenaltiesEnum penaltyType, TeamMembersViewModel penaltyAgainst, long periodTimeRemaining, long jamTimeInMilliseconds, int currentJam, int period)
        {
            PenaltyAgainstMember = penaltyAgainst;
            PenaltyType = penaltyType;
            CurrentDateTimePenalty = DateTime.UtcNow;
            GameTimeInMilliseconds = periodTimeRemaining;
            JamTimeInMilliseconds = jamTimeInMilliseconds;
            Period = period;
            JamNumber = currentJam;
            PenaltyId = Guid.NewGuid();
        }
        public PenaltyViewModel(PenaltiesEnum penaltyType, TeamMembersViewModel penaltyAgainst, int currentJam, Guid jamId, PenaltyScaleEnum scale)
        {
            PenaltyAgainstMember = penaltyAgainst;
            PenaltyType = penaltyType;
            CurrentDateTimePenalty = DateTime.UtcNow;
            GameTimeInMilliseconds = GameViewModel.Instance.PeriodClock.TimeRemaining;
            JamTimeInMilliseconds = GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining;
            Period = GameViewModel.Instance.CurrentPeriod;
            JamNumber = currentJam;
            JamId = jamId;
            PenaltyScale = scale;
            PenaltyId = Guid.NewGuid();
        }
        public PenaltyViewModel(PenaltiesEnum penaltyType, long periodTimeRemaining, int currentJam, int period, DateTime timePened, Guid penaltyId)
        {
            PenaltyTypeAbbre = ToAbbreviation(penaltyType);
            PenaltyType = penaltyType;
            CurrentDateTimePenalty = timePened;
            GameTimeInMilliseconds = periodTimeRemaining;
            Period = period;
            JamNumber = currentJam;
            PenaltyId = penaltyId;
        }

        public int Period { get; set; }
        public Guid PenaltyId { get; set; }
        public DateTime CurrentDateTimePenalty { get; set; }
        public PenaltiesEnum PenaltyType { get; set; }
        public string PenaltyTypeAbbre { get; set; }
        public long GameTimeInMilliseconds { get; set; }
        public long JamTimeInMilliseconds { get; set; }
        public int JamNumber { get; set; }
        public Guid JamId { get; set; }
        public TeamMembersViewModel PenaltyAgainstMember { get; set; }
        public PenaltyScaleEnum PenaltyScale { get; set; }
        /// <summary>
        /// pushes the abbrieviation to what derby people understand.
        /// </summary>
        /// <param name="penalty"></param>
        /// <returns></returns>
        public static string ToAbbreviation(PenaltiesEnum penalty)
        {
            switch (penalty)
            {
                case PenaltiesEnum.Abusive_Language:
                    return "AL";
                case PenaltiesEnum.Back_Block:
                    return "BB";
                case PenaltiesEnum.Blocking_Out_Of_Bounds:
                    return "BoB";
                case PenaltiesEnum.Blocking_To_The_Head:
                    return "BH";
                case PenaltiesEnum.Cut_Track:
                    return "X";
                case PenaltiesEnum.Elbows:
                    return "E";
                case PenaltiesEnum.Excessive_Force:
                    return "EF";
                case PenaltiesEnum.Expulsion:
                    return "E";
                case PenaltiesEnum.False_Start:
                    return "FS";
                case PenaltiesEnum.Forearms:
                    return "F";
                case PenaltiesEnum.Four_Minors:
                    return "FM";
                case PenaltiesEnum.Hitting:
                    return "H";
                case PenaltiesEnum.Illegal_Procedure:
                    return "IP";
                case PenaltiesEnum.Low_Block:
                    return "LB";
                case PenaltiesEnum.Multi_Player_Block:
                    return "MPB";
                case PenaltiesEnum.Team_Penalty:
                    return "TP";
                case PenaltiesEnum.Arms:
                    return "A";
                case PenaltiesEnum.Tripping:
                    return "T";
                case PenaltiesEnum.Clockwise_Skating:
                    return "C";
                case PenaltiesEnum.Stopping:
                    return "S";
                case PenaltiesEnum.ClockWise_Blocking:
                    return "BC";
                case PenaltiesEnum.Stop_Blocks:
                    return "BS";
                case PenaltiesEnum.Assisting_Out_Of_Bounds:
                    return "AB";
                case PenaltiesEnum.Destruction_Of_Pack:
                    return "DP";
                case PenaltiesEnum.Failure_To_Reform:
                    return "FR";
                case PenaltiesEnum.Skating_Out_Of_Play:
                    return "OP";
                case PenaltiesEnum.Blocking_Out_Of_Play:
                    return "BP";
                case PenaltiesEnum.Assisting_Out_Of_Play:
                    return "AP";
                case PenaltiesEnum.Unknown:
                    return "U";
                case PenaltiesEnum.Illegal_Contact:
                    return "IC";
                default:
                    return "";
            }
        }

        public static string ToAbbreviation(PenaltiesRDCLEnum penalty)
        {
            switch (penalty)
            {
                case PenaltiesRDCLEnum.Abusive_Language:
                    return "AL";
                case PenaltiesRDCLEnum.Back_Block:
                    return "BB";
                case PenaltiesRDCLEnum.Blocking_Out_Of_Bounds:
                    return "BoB";
                case PenaltiesRDCLEnum.Blocking_To_The_Head:
                    return "BH";
                case PenaltiesRDCLEnum.Cutting_Track:
                    return "CT";
                case PenaltiesRDCLEnum.Expulsion:
                    return "E";
                case PenaltiesRDCLEnum.Hitting:
                    return "H";
                case PenaltiesRDCLEnum.Illegal_Procedure:
                    return "IP";
                case PenaltiesRDCLEnum.Multi_Player_Block:
                    return "MB";
                case PenaltiesRDCLEnum.Arms:
                    return "A";
                case PenaltiesRDCLEnum.Tripping:
                    return "T";
                case PenaltiesRDCLEnum.Clockwise_Skating:
                    return "C";
                case PenaltiesRDCLEnum.Stopping:
                    return "S";
                case PenaltiesRDCLEnum.ClockWise_Blocking:
                    return "BC";
                case PenaltiesRDCLEnum.Stop_Blocks:
                    return "BS";
                case PenaltiesRDCLEnum.Assisting_Out_Of_Bounds:
                    return "AB";
                case PenaltiesRDCLEnum.Destruction_Of_Pack:
                    return "DP";
                case PenaltiesRDCLEnum.Failure_To_Reform:
                    return "FR";
                case PenaltiesRDCLEnum.Skating_Out_Of_Play:
                    return "OP";
                case PenaltiesRDCLEnum.Blocking_Out_Of_Play:
                    return "BP";
                case PenaltiesRDCLEnum.Assisting_Out_Of_Play:
                    return "AP";
                case PenaltiesRDCLEnum.Unknown:
                    return "U";
                default:
                    return "";
            }
        }
        public static string ToAbbreviation(PenaltiesWFTDA2010Enum penalty)
        {
            switch (penalty)
            {
                case PenaltiesWFTDA2010Enum.Abusive_Language:
                    return "AL";
                case PenaltiesWFTDA2010Enum.Back_Block:
                    return "BB";
                case PenaltiesWFTDA2010Enum.Blocking_Out_Of_Bounds:
                    return "BoB";
                case PenaltiesWFTDA2010Enum.Blocking_To_The_Head:
                    return "BH";
                case PenaltiesWFTDA2010Enum.Cutting_Track:
                    return "CT";
                case PenaltiesWFTDA2010Enum.Elbows:
                    return "E";
                case PenaltiesWFTDA2010Enum.Excessive_Force:
                    return "EF";
                case PenaltiesWFTDA2010Enum.Expulsion:
                    return "E";
                case PenaltiesWFTDA2010Enum.False_Start:
                    return "FS";
                case PenaltiesWFTDA2010Enum.Forearms:
                    return "F";
                case PenaltiesWFTDA2010Enum.Four_Minors:
                    return "FM";
                case PenaltiesWFTDA2010Enum.Hitting:
                    return "H";
                case PenaltiesWFTDA2010Enum.Illegal_Procedure:
                    return "IP";
                case PenaltiesWFTDA2010Enum.Low_Blocking:
                    return "LB";
                case PenaltiesWFTDA2010Enum.Multi_Player_Block:
                    return "MPB";
                case PenaltiesWFTDA2010Enum.Team_Penalty:
                    return "TP";
                case PenaltiesWFTDA2010Enum.Unknown:
                    return "U";
                default:
                    return "";
            }
        }

        public static PenaltiesWFTDAEnum ConvertToWFTDA(PenaltiesEnum penalty)
        {
            switch (penalty)
            {

                case PenaltiesEnum.High_Block:
                    return PenaltiesWFTDAEnum.High_Block;
                case PenaltiesEnum.Blocking_To_The_Head:
                    return PenaltiesWFTDAEnum.Blocking_With_Head;
                case PenaltiesEnum.ClockWise_Blocking:
                    return PenaltiesWFTDAEnum.Clockwise_To_Block;
                case PenaltiesEnum.Direction_Of_Gameplay:
                    return PenaltiesWFTDAEnum.Direction_Of_Gameplay;
                case PenaltiesEnum.Failure_To_Reform:
                    return PenaltiesWFTDAEnum.Failure_To_Reform;
                case PenaltiesEnum.Destruction_Of_Pack:
                    return PenaltiesWFTDAEnum.Destruction_Of_Pack;
                case PenaltiesEnum.Out_Of_Play:
                    return PenaltiesWFTDAEnum.Out_Of_Play;
                case PenaltiesEnum.Skating_Out_Of_Bounds:
                    return PenaltiesWFTDAEnum.Skating_Out_Of_Bounds;
                case PenaltiesEnum.Insubordination:
                    return PenaltiesWFTDAEnum.Misconduct_Gross;
                case PenaltiesEnum.Back_Block:
                    return PenaltiesWFTDAEnum.Back_Block;
                case PenaltiesEnum.Blocking_Out_Of_Bounds:
                    return PenaltiesWFTDAEnum.Blocking_Out_Of_Bounds;
                case PenaltiesEnum.Assisting_Out_Of_Bounds:
                    return PenaltiesWFTDAEnum.Assisting_Out_Of_Bounds;
                case PenaltiesEnum.Cut_Track:
                    return PenaltiesWFTDAEnum.Cut_Track;
                case PenaltiesEnum.Elbows:
                    return PenaltiesWFTDAEnum.Elbows;
                case PenaltiesEnum.Forearms:
                    return PenaltiesWFTDAEnum.Forearms;
                case PenaltiesEnum.Violation:
                    return PenaltiesWFTDAEnum.Violation;
                case PenaltiesEnum.False_Start:
                    return PenaltiesWFTDAEnum.False_Start;
                case PenaltiesEnum.Illegal_Procedure:
                    return PenaltiesWFTDAEnum.Illegal_Procedure;
                case PenaltiesEnum.Low_Block:
                    return PenaltiesWFTDAEnum.Low_Block;
                case PenaltiesEnum.Multi_Player_Block:
                    return PenaltiesWFTDAEnum.Multi_Player_Block;
                default:
                    return PenaltiesWFTDAEnum.Unknown;
            }
        }

        public static string ToAbbreviation(PenaltiesWFTDAEnum penalty)
        {
            switch (penalty)
            {
                case PenaltiesWFTDAEnum.High_Block:
                    return "A";
                case PenaltiesWFTDAEnum.Blocking_With_Head:
                    return "H";
                case PenaltiesWFTDAEnum.Clockwise_To_Block:
                case PenaltiesWFTDAEnum.Direction_Of_Gameplay:
                    return "C";
                case PenaltiesWFTDAEnum.Failure_To_Reform:
                case PenaltiesWFTDAEnum.Destruction_Of_Pack:
                case PenaltiesWFTDAEnum.Out_Of_Play:
                    return "P";
                case PenaltiesWFTDAEnum.Skating_Out_Of_Bounds:
                    return "S";
                case PenaltiesWFTDAEnum.Insubordination:
                    return "N";
                case PenaltiesWFTDAEnum.Misconduct_Gross:
                    return "G";
                case PenaltiesWFTDAEnum.Back_Block:
                    return "B";
                case PenaltiesWFTDAEnum.Blocking_Out_Of_Bounds:
                case PenaltiesWFTDAEnum.Assisting_Out_Of_Bounds:
                    return "O";
                case PenaltiesWFTDAEnum.Cut_Track:
                    return "X";
                case PenaltiesWFTDAEnum.Elbows:
                    return "E";
                case PenaltiesWFTDAEnum.Forearms:
                    return "F";
                case PenaltiesWFTDAEnum.Delay_Of_Game:
                    return "Z";
                case PenaltiesWFTDAEnum.Violation:
                case PenaltiesWFTDAEnum.False_Start:
                case PenaltiesWFTDAEnum.Illegal_Procedure:
                    return "I";
                case PenaltiesWFTDAEnum.Low_Block:
                    return "L";
                case PenaltiesWFTDAEnum.Multi_Player_Block:
                    return "M";
                case PenaltiesWFTDAEnum.Unknown:
                    return "U";
                default:
                    return "";
            }
        }
        public static string ToAbbreviation(PenaltiesMADEEnum penalty)
        {
            switch (penalty)
            {
                case PenaltiesMADEEnum.Abusive_Language:
                    return ToAbbreviation(PenaltiesEnum.Abusive_Language);
                case PenaltiesMADEEnum.Back_Block:
                    return ToAbbreviation(PenaltiesEnum.Back_Block);
                case PenaltiesMADEEnum.Blocking_Out_Of_Bounds:
                    return ToAbbreviation(PenaltiesEnum.Blocking_Out_Of_Bounds);
                case PenaltiesMADEEnum.Blocking_Out_Of_Play:
                    return ToAbbreviation(PenaltiesEnum.Blocking_Out_Of_Play);
                case PenaltiesMADEEnum.Blocking_To_The_Head:
                    return ToAbbreviation(PenaltiesEnum.Blocking_To_The_Head);
                case PenaltiesMADEEnum.Cutting_Track:
                    return ToAbbreviation(PenaltiesEnum.Cut_Track);
                case PenaltiesMADEEnum.Elbows:
                    return ToAbbreviation(PenaltiesEnum.Elbows);
                case PenaltiesMADEEnum.Clockwise_Skating:
                    return ToAbbreviation(PenaltiesEnum.Clockwise_Skating);
                case PenaltiesMADEEnum.Excessive_Force:
                    return ToAbbreviation(PenaltiesEnum.Excessive_Force);
                case PenaltiesMADEEnum.Expulsion:
                    return ToAbbreviation(PenaltiesEnum.Expulsion);
                case PenaltiesMADEEnum.False_Start:
                    return ToAbbreviation(PenaltiesEnum.False_Start);
                case PenaltiesMADEEnum.Forearms:
                    return ToAbbreviation(PenaltiesEnum.Forearms);
                case PenaltiesMADEEnum.Hitting:
                    return ToAbbreviation(PenaltiesEnum.Hitting);
                case PenaltiesMADEEnum.Low_Blocking:
                    return ToAbbreviation(PenaltiesEnum.Low_Block);
                case PenaltiesMADEEnum.Multi_Player_Block:
                    return ToAbbreviation(PenaltiesEnum.Multi_Player_Block);
                case PenaltiesMADEEnum.Team_Penalty:
                    return ToAbbreviation(PenaltiesEnum.Team_Penalty);
                case PenaltiesMADEEnum.Illegal_Contact:
                    return ToAbbreviation(PenaltiesEnum.Illegal_Contact);
                default:
                    return "";
            }
        }
    }
}
