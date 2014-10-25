using RDN.Utilities.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel.Positions.Enums;
using Scoreboard.Library.Static.Enums;

namespace Scoreboard.Library.ViewModel.Calculations
{
    public class NameFunctions
    {
        public static string skaterName(TeamNumberEnum team, GamePositionEnum position)
        {
            try
            {
                switch (team)
                {
                    case TeamNumberEnum.Team1:
                        switch (position)
                        {
                            case GamePositionEnum.J:
                                return GameViewModel.Instance.CurrentJam.JammerT1.SkaterName;
                            case GamePositionEnum.P:
                                return GameViewModel.Instance.CurrentJam.PivotT1.SkaterName;
                            case GamePositionEnum.B1:
                                return GameViewModel.Instance.CurrentJam.Blocker1T1.SkaterName;
                            case GamePositionEnum.B2:
                                return GameViewModel.Instance.CurrentJam.Blocker2T1.SkaterName;
                            case GamePositionEnum.B3:
                                return GameViewModel.Instance.CurrentJam.Blocker3T1.SkaterName;
                            case GamePositionEnum.B4:
                                return GameViewModel.Instance.CurrentJam.Blocker4T1.SkaterName;
                        }
                        break;
                    case TeamNumberEnum.Team2:
                        switch (position)
                        {
                            case GamePositionEnum.J:
                                return GameViewModel.Instance.CurrentJam.JammerT2.SkaterName;
                            case GamePositionEnum.P:
                                return GameViewModel.Instance.CurrentJam.PivotT2.SkaterName;
                            case GamePositionEnum.B1:
                                return GameViewModel.Instance.CurrentJam.Blocker1T2.SkaterName;
                            case GamePositionEnum.B2:
                                return GameViewModel.Instance.CurrentJam.Blocker2T2.SkaterName;
                            case GamePositionEnum.B3:
                                return GameViewModel.Instance.CurrentJam.Blocker3T2.SkaterName;
                            case GamePositionEnum.B4:
                                return GameViewModel.Instance.CurrentJam.Blocker4T2.SkaterName;
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return "_";
        }
        public static string skaterNumber(TeamNumberEnum team, GamePositionEnum position)
        {
            try
            {
                switch (team)
                {
                    case TeamNumberEnum.Team1:
                        switch (position)
                        {
                            case GamePositionEnum.J:
                                return GameViewModel.Instance.CurrentJam.JammerT1.SkaterNumber;
                            case GamePositionEnum.P:
                                return GameViewModel.Instance.CurrentJam.PivotT1.SkaterNumber;
                            case GamePositionEnum.B1:
                                return GameViewModel.Instance.CurrentJam.Blocker1T1.SkaterNumber;
                            case GamePositionEnum.B2:
                                return GameViewModel.Instance.CurrentJam.Blocker2T1.SkaterNumber;
                            case GamePositionEnum.B3:
                                return GameViewModel.Instance.CurrentJam.Blocker3T1.SkaterNumber;
                            case GamePositionEnum.B4:
                                return GameViewModel.Instance.CurrentJam.Blocker4T1.SkaterNumber;
                        }
                        break;
                    case TeamNumberEnum.Team2:
                        switch (position)
                        {
                            case GamePositionEnum.J:
                                return GameViewModel.Instance.CurrentJam.JammerT2.SkaterNumber;
                            case GamePositionEnum.P:
                                return GameViewModel.Instance.CurrentJam.PivotT2.SkaterNumber;
                            case GamePositionEnum.B1:
                                return GameViewModel.Instance.CurrentJam.Blocker1T2.SkaterNumber;
                            case GamePositionEnum.B2:
                                return GameViewModel.Instance.CurrentJam.Blocker2T2.SkaterNumber;
                            case GamePositionEnum.B3:
                                return GameViewModel.Instance.CurrentJam.Blocker3T2.SkaterNumber;
                            case GamePositionEnum.B4:
                                return GameViewModel.Instance.CurrentJam.Blocker4T2.SkaterNumber;
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return "_";
        }
        public static string teamName(int teamOneOrTwo)
        {
            try
            {
                if (teamOneOrTwo == 1)
                    return GameViewModel.Instance.Team1.TeamName;
                else if (teamOneOrTwo == 2)
                    return GameViewModel.Instance.Team2.TeamName;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return "";
        }
        public static string rosterName(int teamOneOrTwo, int rosterNum)
        {
            try
            {
                string[] rostah = { };
                if (teamOneOrTwo == 1)
                    rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterName).ToArray();
                else if (teamOneOrTwo == 2)
                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterName).ToArray();
                return rostah[rosterNum - 1];
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }
        public static string rosterNumber(int teamOneOrTwo, int rosterNum)
        {
            try
            {
                string[] rostah = { };
                if (teamOneOrTwo == 1)
                    rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterNumber).ToArray();
                else if (teamOneOrTwo == 2)
                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterNumber).ToArray();
                return rostah[rosterNum - 1];
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }
    }
}
