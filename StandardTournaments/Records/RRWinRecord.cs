using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tournaments.Standard.Records
{
    public class RRWinRecord : IComparable
    {
        public int Wins
        {
            get;
            set;
        }
        public int Losses
        {
            get;
            set;
        }
        public int Draws
        {
            get;
            set;
        }
        public Score OverallScore
        {
            get;
            set;
        }
        public Score OverallDifference
        {
            get;
            set;
        }
        public double WinRecord
        {
            get
            {
                return (this.Wins * 1.0) + (this.Draws * 0.5) + (this.Losses * 0.0);
            }
        }

        public override bool Equals(object obj)
        {
            RRWinRecord record = obj as RRWinRecord;
            if (record != null)
            {
                return this == record;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Wins + 10 * this.Draws + 100 * this.Losses;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            RRWinRecord record = obj as RRWinRecord;

            if (record != null)
            {
                int comp = this.WinRecord.CompareTo(record.WinRecord);
                if (comp != 0)
                {
                    return comp;
                }

                comp = this.Wins.CompareTo(record.Wins);
                if (comp != 0)
                {
                    return comp;
                }

                comp = this.Losses.CompareTo(record.Losses);
                if (comp != 0)
                {
                    return -1 * comp;
                }

                comp = this.OverallScore.CompareTo(record.OverallScore);
                if (comp != 0)
                {
                    return comp;
                }

                return 0;
            }
            else
            {
                throw new ArgumentException("Object is not an RRWinRecord", "obj");
            }
        }

        public static bool operator ==(RRWinRecord score1, RRWinRecord score2)
        {
            if (object.ReferenceEquals(score1, score2))
            {
                return true;
            }
            else if ((object)score1 == null || (object)score2 == null)
            {
                return false;
            }
            else
            {
                return score1.Wins == score2.Wins &&
                    score1.Losses == score2.Losses &&
                    score1.Draws == score2.Draws &&
                    score1.OverallScore == score2.OverallScore;
            }
        }

        public static bool operator !=(RRWinRecord score1, RRWinRecord score2)
        {
            return !(score1 == score2);
        }

        public static bool operator >(RRWinRecord score1, RRWinRecord score2)
        {
            return score1.CompareTo(score2) > 0;
        }

        public static bool operator <(RRWinRecord score1, RRWinRecord score2)
        {
            return score1.CompareTo(score2) < 0;
        }

        public static bool operator >=(RRWinRecord score1, RRWinRecord score2)
        {
            return score1.CompareTo(score2) >= 0;
        }

        public static bool operator <=(RRWinRecord score1, RRWinRecord score2)
        {
            return score1.CompareTo(score2) <= 0;
        }
    }

  
}
