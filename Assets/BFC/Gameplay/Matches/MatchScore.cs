using System;
using BFC.Core.Matches;

namespace BFC.Gameplay.Matches
{
    /// <summary>
    /// Immutable logical score. Team presentation names/colors do not belong here.
    /// </summary>
    public readonly struct MatchScore
    {
        public MatchScore(int teamA, int teamB)
        {
            if (teamA < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(teamA));
            }

            if (teamB < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(teamB));
            }

            TeamA = teamA;
            TeamB = teamB;
        }

        public int TeamA { get; }

        public int TeamB { get; }

        public int For(TeamId team)
        {
            switch (team)
            {
                case TeamId.TeamA:
                    return TeamA;
                case TeamId.TeamB:
                    return TeamB;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team));
            }
        }

        public MatchScore AddGoal(TeamId scoringTeam)
        {
            switch (scoringTeam)
            {
                case TeamId.TeamA:
                    return new MatchScore(TeamA + 1, TeamB);
                case TeamId.TeamB:
                    return new MatchScore(TeamA, TeamB + 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(scoringTeam));
            }
        }
    }
}
