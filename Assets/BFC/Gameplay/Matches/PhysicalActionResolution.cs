using System;
using BFC.Core.Matches;

namespace BFC.Gameplay.Matches
{
    public enum PhysicalActionOutcome
    {
        ContinuePossession = 0,
        TransferPossession = 1,
        Goal = 2
    }

    /// <summary>
    /// Result reported after physics reaches a safe continuation point (GAME-003).
    /// It intentionally does not equate an action with a ball touch (GAME-004).
    /// </summary>
    public readonly struct PhysicalActionResolution
    {
        private PhysicalActionResolution(PhysicalActionOutcome outcome, TeamId goalScoringTeam)
        {
            Outcome = outcome;
            GoalScoringTeam = goalScoringTeam;
        }

        public PhysicalActionOutcome Outcome { get; }

        public TeamId GoalScoringTeam { get; }

        public static PhysicalActionResolution ContinuePossession()
        {
            return new PhysicalActionResolution(PhysicalActionOutcome.ContinuePossession, TeamId.None);
        }

        public static PhysicalActionResolution TransferPossession()
        {
            return new PhysicalActionResolution(PhysicalActionOutcome.TransferPossession, TeamId.None);
        }

        public static PhysicalActionResolution Goal(TeamId scoringTeam)
        {
            if (scoringTeam == TeamId.None)
            {
                throw new ArgumentOutOfRangeException(nameof(scoringTeam));
            }

            return new PhysicalActionResolution(PhysicalActionOutcome.Goal, scoringTeam);
        }
    }
}
