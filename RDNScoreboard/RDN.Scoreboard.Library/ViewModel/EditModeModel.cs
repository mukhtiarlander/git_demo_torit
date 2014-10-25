using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.ViewModel
{
    public enum EditModeEnum
    {
        upTeam1TimeOut,
        downTeam1TimeOut,
        Team1TimeOutChange,
        Team2TimeOutChange,
        upPeriodTime,
        downPeriodTime,
        periodClockChanged,
        jamClockChanged,
        lineUpClockChanged,
        upJamTime,
        downJamTime,
        upLineUpTime,
        downLineUpTime,
        upTimeOutTime,
        downTimeOutTime,
        upIntermissionTime,
        downIntermissionTime,
        intermissionClockChanged,
        downPeriodNumber,
        upPeriodNumber,
        periodNumberChanged,
        upJamNumber,
        downJamNumber,
        jamNumberChanged,
        upLineUpNumber,
        downLineUpNumber,
        upTeam2TimeOut,
        downTeam2TimeOut,
        IntermissionName
    }
    public class EditModeModel
    {
        public EditModeModel() { }
        public EditModeEnum EditModeType { get; set; }
        public string additionalInformation { get; set; }
    }
}
