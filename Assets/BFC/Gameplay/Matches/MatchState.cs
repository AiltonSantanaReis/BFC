using System;
using BFC.Core.Matches;

namespace BFC.Gameplay.Matches
{
    /// <summary>
    /// Read-only snapshot of the authoritative logical match state.
    /// Construction is owned by BFC.Gameplay so callers cannot manufacture transitions.
    /// </summary>
    public sealed class MatchState
    {
        internal MatchState(
            MatchPhase phase,
            TeamId possession,
            int actionsUsedInPossession,
            MatchScore score,
            TimeSpan duration,
            TimeSpan elapsedTime)
        {
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(duration));
            }

            if (elapsedTime < TimeSpan.Zero || elapsedTime > duration)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedTime));
            }

            if (actionsUsedInPossession < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(actionsUsedInPossession));
            }

            Phase = phase;
            Possession = possession;
            ActionsUsedInPossession = actionsUsedInPossession;
            Score = score;
            Duration = duration;
            ElapsedTime = elapsedTime;
        }

        public MatchPhase Phase { get; }

        public TeamId Possession { get; }

        public int ActionsUsedInPossession { get; }

        public MatchScore Score { get; }

        public TimeSpan Duration { get; }

        public TimeSpan ElapsedTime { get; }

        public TimeSpan RemainingTime => Duration - ElapsedTime;

        public bool IsClockExpired => ElapsedTime >= Duration;
    }
}
