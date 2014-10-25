//-----------------------------------------------------------------------
// <copyright file="TournamentRanking.cs" company="(none)">
//  Copyright (c) 2009 John Gietzen
//
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
//
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
//  BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
//  ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace Tournaments
{
    /// <summary>
    /// Describes the position of a team in a tournament's rankings.
    /// </summary>
    public sealed class TournamentRanking
    {
        /// <summary>
        /// Holds the team being ranked.
        /// </summary>
        public  TournamentTeam team{get;set;}

        /// <summary>
        /// Holds the rank number of the ranking.
        /// </summary>
        public  double rank { get; set; }

        /// <summary>
        /// Holds the score description or justification of the ranking.
        /// </summary>
        public  string scoreDescription{get;set;}

        public TournamentRanking()
        { }
        /// <summary>
        /// Initializes a new instance of the TournamentRanking class.
        /// </summary>
        /// <param name="team">The team being ranked.</param>
        /// <param name="rank">The actual rank number of the ranking.</param>
        /// <param name="scoreDescription">The score description or justification of the ranking.</param>
        public TournamentRanking(TournamentTeam team, double rank, string scoreDescription)
        {
            this.team = team;
            this.rank = rank;
            this.scoreDescription = scoreDescription;
        }
        public TournamentRanking(TournamentTeam team, double rank, int wins, int loses, Score pointSpread, Score totalPoints, string scoreDescription)
        {
            this.team = team;
            this.rank = rank;
            this.Wins = wins;
            this.Loses = loses;
            this.PointSpread = pointSpread;
            this.TotalPoints = totalPoints;
            this.scoreDescription = scoreDescription;
        }

   
        public int Wins
        {

            get;
            set;
        }
        public int Loses
        {
            get;
            set;
        }
        public Score PointSpread
        {
            get;
            set;
        }
        public Score TotalPoints
        {
            get;
            set;
        }
        public string TeamName
        {
            get;
            set;
        }

       
    }
}
