//-----------------------------------------------------------------------
// <copyright file="TournamentTeam.cs" company="(none)">
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Describes a team that may participate in a tournament.
    /// </summary>
    //[DebuggerDisplay("[Team {this.TeamId} @ {this.Rating}]")]
    public sealed class TournamentPoolGroup
    {
        public int GroupId { get; set; }

    public     List<TournamentTeam> teamsInGroup;
        /// <summary>
        /// Initializes a new instance of the TournamentTeam class.
        /// </summary>
        /// <param name="teamId">The unique, application-specific identifier of the team.</param>
        /// <param name="rating">The team's current rating pertaining to a specific tournament.</param>
        public TournamentPoolGroup()
        {
            teamsInGroup = new List<TournamentTeam>();
        }

        public void AddTeam(TournamentTeam team)
        {
            teamsInGroup.Add(team);
        }
        public void AddTeams(List<TournamentTeam> teams)
        {
            teamsInGroup.AddRange(teams);
        }

    }
}
