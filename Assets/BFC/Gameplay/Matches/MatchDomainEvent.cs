using System;
using BFC.Core.Matches;

namespace BFC.Gameplay.Matches
{
    public enum MatchDomainEventType
    {
        MatchStarted = 0,
        PlayerActionAccepted = 1,
        PhysicalActionResolved = 2,
        PossessionChanged = 3,
        GoalScored = 4,
        MatchClockExpired = 5,
        MatchFinished = 6
    }

    public enum PossessionChangeReason
    {
        None = 0,
        ResolutionTransfer = 1,
        ActionLimitReached = 2,
        ExplicitRestart = 3
    }

    /// <summary>
    /// Immutable domain-event envelope for Phase 3. Neutral values are used for fields
    /// that do not apply to a specific event type.
    /// </summary>
    public readonly struct MatchDomainEvent
    {
        private MatchDomainEvent(
            MatchDomainEventType type,
            TeamId team,
            TeamId secondaryTeam,
            string pieceId,
            int actionNumber,
            PhysicalActionOutcome physicalOutcome,
            PossessionChangeReason possessionChangeReason,
            MatchScore score,
            MatchFinishReason finishReason,
            TimeSpan clockDuration)
        {
            Type = type;
            Team = team;
            SecondaryTeam = secondaryTeam;
            PieceId = pieceId;
            ActionNumber = actionNumber;
            PhysicalOutcome = physicalOutcome;
            PossessionChangeReason = possessionChangeReason;
            Score = score;
            FinishReason = finishReason;
            ClockDuration = clockDuration;
        }

        public MatchDomainEventType Type { get; }
        public TeamId Team { get; }
        public TeamId SecondaryTeam { get; }
        public string PieceId { get; }
        public int ActionNumber { get; }
        public PhysicalActionOutcome PhysicalOutcome { get; }
        public PossessionChangeReason PossessionChangeReason { get; }
        public MatchScore Score { get; }
        public MatchFinishReason FinishReason { get; }
        public TimeSpan ClockDuration { get; }

        public static MatchDomainEvent MatchStarted(TeamId initialPossession)
        {
            return Create(MatchDomainEventType.MatchStarted, team: initialPossession);
        }

        public static MatchDomainEvent PlayerActionAccepted(TeamId team, string pieceId, int actionNumber)
        {
            return Create(
                MatchDomainEventType.PlayerActionAccepted,
                team: team,
                pieceId: pieceId,
                actionNumber: actionNumber);
        }

        public static MatchDomainEvent PhysicalActionResolved(TeamId actingTeam, PhysicalActionOutcome outcome)
        {
            return Create(
                MatchDomainEventType.PhysicalActionResolved,
                team: actingTeam,
                physicalOutcome: outcome);
        }

        public static MatchDomainEvent PossessionChanged(
            TeamId previousPossession,
            TeamId newPossession,
            PossessionChangeReason reason)
        {
            return Create(
                MatchDomainEventType.PossessionChanged,
                team: previousPossession,
                secondaryTeam: newPossession,
                possessionChangeReason: reason);
        }

        public static MatchDomainEvent GoalScored(TeamId scoringTeam, MatchScore score)
        {
            return Create(MatchDomainEventType.GoalScored, team: scoringTeam, score: score);
        }

        public static MatchDomainEvent MatchClockExpired(TimeSpan duration)
        {
            return Create(MatchDomainEventType.MatchClockExpired, clockDuration: duration);
        }

        public static MatchDomainEvent MatchFinished(MatchFinishReason reason, MatchScore score)
        {
            return Create(MatchDomainEventType.MatchFinished, score: score, finishReason: reason);
        }

        private static MatchDomainEvent Create(
            MatchDomainEventType type,
            TeamId team = TeamId.None,
            TeamId secondaryTeam = TeamId.None,
            string pieceId = "",
            int actionNumber = 0,
            PhysicalActionOutcome physicalOutcome = PhysicalActionOutcome.ContinuePossession,
            PossessionChangeReason possessionChangeReason = PossessionChangeReason.None,
            MatchScore score = default,
            MatchFinishReason finishReason = MatchFinishReason.None,
            TimeSpan clockDuration = default)
        {
            return new MatchDomainEvent(
                type,
                team,
                secondaryTeam,
                pieceId,
                actionNumber,
                physicalOutcome,
                possessionChangeReason,
                score,
                finishReason,
                clockDuration);
        }
    }
}
