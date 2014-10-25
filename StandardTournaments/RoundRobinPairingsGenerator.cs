//-----------------------------------------------------------------------
// <copyright file="RoundRobinPairingsGenerator.cs" company="(none)">
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

namespace Tournaments.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Tournaments.Standard.Records;

    public sealed class RoundRobinPairingsGenerator : IPairingsGenerator
    {
        [DebuggerDisplay("[Team A {this.A.TeamId} @ {this.B.TeamId} 4 {this.GroupId}]")]
        private class RRPairing
        {
            public int GroupId { get; set; }
            public TournamentTeam A { get; set; }
            public TournamentTeam B { get; set; }
        }

        bool IsPoolPlay { get; set; }
        public RoundRobinPairingsGenerator()
        {
        }
        public RoundRobinPairingsGenerator(bool isPoolPlay)
        {
            IsPoolPlay = isPoolPlay;
        }

        PairingsGeneratorState state = PairingsGeneratorState.NotInitialized;

        public string Name
        {
            get
            {
                return "Round-robin";
            }
        }

        public PairingsGeneratorState State
        {
            get
            {
                return this.state;
            }
        }

        public bool SupportsLateEntry
        {
            get
            {
                return true;
            }
        }
        List<int?> GroupIds;
        List<TournamentPoolGroup> PoolGroups;
        List<TournamentTeam> loadedTeams;
        List<RRPairing> loadedPairings;
        List<TournamentRound> loadedRounds;

        public void Reset()
        {
            this.loadedTeams = null;
            this.loadedPairings = null;
            this.loadedRounds = null;
            this.PoolGroups = null;
            this.state = PairingsGeneratorState.NotInitialized;
        }

        public void LoadState(IEnumerable<TournamentTeam> teams, IList<TournamentRound> rounds)
        {
            if (teams == null)
            {
                throw new ArgumentNullException("teams");
            }

            if (rounds == null)
            {
                throw new ArgumentNullException("rounds");
            }
            if (IsPoolPlay)
                LoadRoundRobinPoolPlay(teams, rounds);
            else
                LoadRoundRobinDefault(teams, rounds);
        }
        private void LoadRoundRobinPoolPlay(IEnumerable<TournamentTeam> teams, IList<TournamentRound> rounds)
        {
            this.loadedPairings = new List<RRPairing>();
            this.loadedRounds = new List<TournamentRound>();
            PoolGroups = new List<TournamentPoolGroup>();

            GroupIds = teams.Select(x => x.GroupId).Distinct().ToList();

            foreach (var id in GroupIds)
            {
                TournamentPoolGroup group = new TournamentPoolGroup();
                group.GroupId = id.Value;
                group.AddTeams(teams.Where(x => x.GroupId.HasValue && x.GroupId == id).ToList());
                PoolGroups.Add(group);
            }

            foreach (var group in PoolGroups)
            {

                // Build our total list of pairings.
                List<RRPairing> newPairings = new List<RRPairing>();
                for (int i = 0; i < group.teamsInGroup.Count; i++)
                {
                    for (int j = i + 1; j < group.teamsInGroup.Count; j++)
                    {
                        newPairings.Add(new RRPairing() { A = group.teamsInGroup[i], B = group.teamsInGroup[j], GroupId = group.GroupId });
                    }
                }

                // Remove from the pairings list each pairing that has already been executed
                foreach (TournamentRound round in rounds)
                {
                    foreach (TournamentPairing pairing in round.Pairings.Where(x => x.GroupId == group.GroupId))
                    {
                        List<TournamentTeamScore> pair = new List<TournamentTeamScore>(pairing.TeamScores);

                        if (pair.Count > 2)
                        {
                            throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  There exists a pairing containing more than two competing teams.");
                        }
                        else if (pair.Count <= 1)
                        {
                            continue;
                        }
                        else
                        {
                            Func<RRPairing, bool> filter = (RRPairing p) => (p.A.TeamId == pair[0].Team.TeamId && p.B.TeamId == pair[1].Team.TeamId) || (p.A.TeamId == pair[1].Team.TeamId && p.B.TeamId == pair[0].Team.TeamId);
                            RRPairing remove = newPairings.SingleOrDefault(filter);
                            if (remove == null)
                            {
                                if (pair[0].Team.TeamId == pair[1].Team.TeamId)
                                {
                                    throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  At lease one pairing has the same team entered twice.");
                                }
                                else if (group.teamsInGroup.Where(x => x.TeamId == pair[0].Team.TeamId).FirstOrDefault() == null || group.teamsInGroup.Where(x => x.TeamId == pair[1].Team.TeamId).FirstOrDefault() == null)
                                {
                                    throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  At lease one who does not belong to the tournament team has been involved in a pairing.");
                                }
                                else
                                {
                                    throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  At lease one pairing has been executed more than once.");
                                }
                            }

                            var isRemoved = newPairings.Remove(remove);
                            if (isRemoved == false)
                                isRemoved = false;
                        }
                    }
                }


                this.loadedPairings.AddRange(newPairings);
                this.loadedRounds.AddRange(new List<TournamentRound>(rounds));

            }
            this.loadedTeams = teams.ToList();
            this.state = PairingsGeneratorState.Initialized;
        }

        private void LoadRoundRobinDefault(IEnumerable<TournamentTeam> teams, IList<TournamentRound> rounds)
        {

            // Load our list of teams.
            List<TournamentTeam> newTeams = new List<TournamentTeam>();
            newTeams.AddRange(teams);

            // Build our total list of pairings.
            List<RRPairing> newPairings = new List<RRPairing>();
            for (int i = 0; i < newTeams.Count; i++)
            {
                for (int j = i + 1; j < newTeams.Count; j++)
                {
                    newPairings.Add(new RRPairing() { A = newTeams[i], B = newTeams[j] });
                }
            }

            // Remove from the pairings list each pairing that has already been executed
            foreach (TournamentRound round in rounds)
            {
                foreach (TournamentPairing pairing in round.Pairings)
                {
                    List<TournamentTeamScore> pair = new List<TournamentTeamScore>(pairing.TeamScores);

                    if (pair.Count > 2)
                    {
                        throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  There exists a pairing containing more than two competing teams.");
                    }
                    else if (pair.Count <= 1)
                    {
                        continue;
                    }
                    else
                    {
                        Func<RRPairing, bool> filter = (RRPairing p) => (p.A.TeamId == pair[0].Team.TeamId && p.B.TeamId == pair[1].Team.TeamId) || (p.A.TeamId == pair[1].Team.TeamId && p.B.TeamId == pair[0].Team.TeamId);
                        RRPairing remove = newPairings.SingleOrDefault(filter);
                        if (remove == null)
                        {
                            if (pair[0].Team.TeamId == pair[1].Team.TeamId)
                            {
                                throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  At lease one pairing has the same team entered twice.");
                            }
                            else if (newTeams.Where(x => x.TeamId == pair[0].Team.TeamId).FirstOrDefault() == null || newTeams.Where(x => x.TeamId == pair[1].Team.TeamId).FirstOrDefault() == null)
                            {
                                throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  At lease one who does not belong to the tournament team has been involved in a pairing.");
                            }
                            else
                            {
                                throw new InvalidTournamentStateException("The rounds alread executed in this tournament make it invalid as a round-robin tournament for the following reason:  At lease one pairing has been executed more than once.");
                            }
                        }

                        var isRemoved = newPairings.Remove(remove);
                        if (isRemoved == false)
                            isRemoved = false;
                    }
                }
            }

            this.loadedTeams = newTeams;
            this.loadedPairings = newPairings;
            this.loadedRounds = new List<TournamentRound>(rounds);
            this.state = PairingsGeneratorState.Initialized;
        }

        public TournamentRound CreateNextRound(int? places)
        {
            if (this.loadedPairings.Count == 0)
            {
                return null;
            }
            IList<TournamentPairing> pairings = null;
            if (IsPoolPlay)
                pairings = new List<TournamentPairing>(this.GetNextRoundPairingsPoolPlay(this.loadedPairings));
            else
                pairings = new List<TournamentPairing>(this.GetNextRoundPairingsDefault(this.loadedPairings));
            return new TournamentRound(pairings);

        }
        private IEnumerable<TournamentPairing> GetNextRoundPairingsPoolPlay(List<RRPairing> allPairingsLeft)
        {

            foreach (var id in GroupIds)
            {

                List<RRPairing> pairingsLeft = new List<RRPairing>(allPairingsLeft.Where(x => x.GroupId == id).ToList());
                List<TournamentTeam> teamsAdded = new List<TournamentTeam>();

                while (pairingsLeft.Count > 0)
                {
                    var nextPairings = from p in pairingsLeft
                                       orderby
                                       Math.Min(
             (from p1 in pairingsLeft
              where p1.A == p.A || p1.B != p.A
              select p1).Count(),
             (from p1 in pairingsLeft
              where p1.A == p.B || p1.B != p.B
              select p1).Count()
          ) descending
                                       select p;

                    RRPairing pairing = nextPairings.First();

                    yield return new TournamentPairing(id.GetValueOrDefault(),
                        new TournamentTeamScore[]
                    {
                        new TournamentTeamScore(pairing.A, null),
                        new TournamentTeamScore(pairing.B, null)
                    });

                    List<RRPairing> invalidated = new List<RRPairing>(pairingsLeft.Where(p => (p.A == pairing.A || p.B == pairing.A || p.A == pairing.B || p.B == pairing.B)));
                    foreach (RRPairing remove in invalidated)
                    {
                        pairingsLeft.Remove(remove);
                    }

                    teamsAdded.Add(pairing.A);
                    teamsAdded.Add(pairing.B);
                }
            }
        }

        private IEnumerable<TournamentPairing> GetNextRoundPairingsDefault(List<RRPairing> allPairingsLeft)
        {
            List<RRPairing> pairingsLeft = new List<RRPairing>(allPairingsLeft);
            List<TournamentTeam> teamsAdded = new List<TournamentTeam>();

            while (pairingsLeft.Count > 0)
            {
                var nextPairings = from p in pairingsLeft
                                   orderby
                                   Math.Min(
         (from p1 in pairingsLeft
          where p1.A == p.A || p1.B != p.A
          select p1).Count(),
         (from p1 in pairingsLeft
          where p1.A == p.B || p1.B != p.B
          select p1).Count()
      ) descending
                                   select p;

                RRPairing pairing = nextPairings.First();

                yield return new TournamentPairing(
                    new TournamentTeamScore[]
                    {
                        new TournamentTeamScore(pairing.A, null),
                        new TournamentTeamScore(pairing.B, null)
                    });

                List<RRPairing> invalidated = new List<RRPairing>(pairingsLeft.Where(p => (p.A == pairing.A || p.B == pairing.A || p.A == pairing.B || p.B == pairing.B)));
                foreach (RRPairing remove in invalidated)
                {
                    pairingsLeft.Remove(remove);
                }

                teamsAdded.Add(pairing.A);
                teamsAdded.Add(pairing.B);
            }
        }

        public IEnumerable<TournamentRanking> GenerateRankings()
        {
            //if (this.loadedPairings.Count > 0)
            //{
            //    throw new InvalidTournamentStateException("The tournament is not in a state that allows ranking for the following reason: There is at least one pairing left to execute.");
            //}

            Dictionary<TournamentTeam, RRWinRecord> records = new Dictionary<TournamentTeam, RRWinRecord>();
            foreach (TournamentTeam team in this.loadedTeams)
            {
                records[team] = new RRWinRecord() { Wins = 0, Losses = 0, Draws = 0, OverallScore = null };
            }

            foreach (TournamentRound round in loadedRounds)
            {
                foreach (TournamentPairing pairing in round.Pairings)
                {
                    List<TournamentTeamScore> pair = new List<TournamentTeamScore>(pairing.TeamScores);

                    if (pair.Count <= 1)
                    {
                        continue;
                    }

                    TournamentTeam teamA = pair[0].Team;
                    var scoreA = pair[0].Score;
                    var recordA = records.Where(x => x.Key.TeamId == teamA.TeamId).FirstOrDefault().Value;
                    TournamentTeam teamB = pair[1].Team;
                    var scoreB = pair[1].Score;
                    var recordB = records.Where(x => x.Key.TeamId == teamB.TeamId).FirstOrDefault().Value;

                    if (scoreA == null || scoreB == null)
                    {
                        throw new InvalidTournamentStateException("The tournament is not in a state that allows ranking for the following reason: At least one pairing is missing a score.");
                        //continue;
                    }

                    recordA.OverallScore += scoreA;
                    recordB.OverallScore += scoreB;

                    if (scoreA == scoreB)
                    {
                        recordA.Draws += 1;
                        recordB.Draws += 1;
                    }
                    else if (scoreA > scoreB)
                    {
                        recordA.Wins += 1;
                        recordB.Losses += 1;
                        recordB.OverallDifference += scoreB.Subtract(scoreA);
                        recordA.OverallDifference += scoreA.Subtract(scoreB);
                    }
                    else
                    {
                        recordA.Losses += 1;
                        recordB.Wins += 1;
                        recordA.OverallDifference += scoreA.Subtract(scoreB);
                        recordB.OverallDifference += scoreB.Subtract(scoreA);
                    }
                }
            }

            int r = 1, lastRank = 1;
            RRWinRecord lastRecord = null;

            var ranks = from team in records.Keys
                        let teamRecord = records[team]
                        orderby teamRecord descending
                        select new RRRank() { Team = team, Rank = r++, Record = teamRecord };

            foreach (var rank in ranks)
            {
                if (rank.Record != null && lastRecord == rank.Record)
                {
                    rank.Rank = lastRank;
                }

                lastRecord = rank.Record;
                lastRank = rank.Rank;

                string scoreDescription = String.Format("{0:F} ({1}-{2}-{3} with {4} overall).", rank.Record.WinRecord, rank.Record.Wins, rank.Record.Draws, rank.Record.Losses, rank.Record.OverallScore);
                yield return new TournamentRanking(rank.Team, rank.Rank, rank.Record.Wins, rank.Record.Losses, rank.Record.OverallDifference, rank.Record.OverallScore, scoreDescription);
            }

            yield break;
        }
    }
}
