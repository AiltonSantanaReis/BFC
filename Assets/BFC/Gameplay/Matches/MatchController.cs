using System;
using System.Collections.Generic;
using BFC.Core.Matches;
using BFC.Core.Rules;

namespace BFC.Gameplay.Matches
{
    /// <summary>
    /// Pure match-flow state machine. It owns logical transitions but does not render,
    /// read input, or manipulate Unity physics directly.
    /// </summary>
    public sealed class MatchController
    {
        private readonly IMatchRules _rules;
        private readonly List<MatchDomainEvent> _events = new List<MatchDomainEvent>();
        private PlayerActionCommand? _pendingAction;

        public MatchController(IMatchRules rules, TimeSpan duration)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(duration));
            }

            State = new MatchState(
                MatchPhase.NotStarted,
                TeamId.None,
                actionsUsedInPossession: 0,
                new MatchScore(0, 0),
                duration,
                TimeSpan.Zero);
        }

        public MatchState State { get; private set; }

        public bool HasPendingPhysicalResolution => _pendingAction.HasValue;

        public PlayerActionCommand? PendingAction => _pendingAction;

        public void StartMatch(TeamId initialPossession)
        {
            if (initialPossession == TeamId.None)
            {
                throw new ArgumentOutOfRangeException(nameof(initialPossession));
            }

            if (State.Phase != MatchPhase.NotStarted)
            {
                throw new InvalidOperationException("Match can only be started once.");
            }

            State = NewState(MatchPhase.AwaitingAction, initialPossession, 0, State.Score, State.ElapsedTime);
            _events.Add(MatchDomainEvent.MatchStarted(initialPossession));
        }

        public PlayerActionSubmissionResult TrySubmitAction(PlayerActionCommand command)
        {
            switch (State.Phase)
            {
                case MatchPhase.NotStarted:
                    return PlayerActionSubmissionResult.Reject(PlayerActionRejectionReason.MatchNotStarted);
                case MatchPhase.Finished:
                    return PlayerActionSubmissionResult.Reject(PlayerActionRejectionReason.MatchFinished);
                case MatchPhase.ResolvingAction:
                    return PlayerActionSubmissionResult.Reject(PlayerActionRejectionReason.PhysicalResolutionPending);
                case MatchPhase.AwaitingRestart:
                    return PlayerActionSubmissionResult.Reject(PlayerActionRejectionReason.RestartPending);
            }

            if (command.Team != State.Possession)
            {
                return PlayerActionSubmissionResult.Reject(PlayerActionRejectionReason.TeamNotInPossession);
            }

            int actionNumber = State.ActionsUsedInPossession + 1;
            if (actionNumber > _rules.MaxActionsPerPossession)
            {
                throw new InvalidOperationException("Match state exceeded the ruleset action limit.");
            }

            // GAME-003: no next competitive action is accepted until physics resolves this one.
            State = NewState(
                MatchPhase.ResolvingAction,
                State.Possession,
                actionNumber,
                State.Score,
                State.ElapsedTime);
            _pendingAction = command;
            _events.Add(MatchDomainEvent.PlayerActionAccepted(command.Team, command.PieceId, actionNumber));
            return PlayerActionSubmissionResult.Accept();
        }

        public void ResolvePhysicalAction(PhysicalActionResolution resolution)
        {
            if (State.Phase != MatchPhase.ResolvingAction || !_pendingAction.HasValue)
            {
                throw new InvalidOperationException("No player action is waiting for physical resolution.");
            }

            TeamId actingTeam = State.Possession;
            _pendingAction = null;
            _events.Add(MatchDomainEvent.PhysicalActionResolved(actingTeam, resolution.Outcome));

            if (resolution.Outcome == PhysicalActionOutcome.Goal)
            {
                MatchScore score = State.Score.AddGoal(resolution.GoalScoringTeam);
                State = NewState(MatchPhase.AwaitingRestart, TeamId.None, 0, score, State.ElapsedTime);
                _events.Add(MatchDomainEvent.GoalScored(resolution.GoalScoringTeam, score));
                return;
            }

            if (resolution.Outcome == PhysicalActionOutcome.TransferPossession)
            {
                TransferPossession(PossessionChangeReason.ResolutionTransfer);
                return;
            }

            // GAME-004: the action counter is independent from ball-touch semantics.
            if (State.ActionsUsedInPossession >= _rules.MaxActionsPerPossession)
            {
                TransferPossession(PossessionChangeReason.ActionLimitReached);
                return;
            }

            State = NewState(
                MatchPhase.AwaitingAction,
                actingTeam,
                State.ActionsUsedInPossession,
                State.Score,
                State.ElapsedTime);
        }

        public void ResumeAfterRestart(TeamId nextPossession)
        {
            if (State.Phase != MatchPhase.AwaitingRestart)
            {
                throw new InvalidOperationException("Match is not waiting for a restart.");
            }

            if (nextPossession == TeamId.None)
            {
                throw new ArgumentOutOfRangeException(nameof(nextPossession));
            }

            // OPEN-002 remains unresolved: caller/ruleset must explicitly choose restart possession.
            State = NewState(MatchPhase.AwaitingAction, nextPossession, 0, State.Score, State.ElapsedTime);
            _events.Add(MatchDomainEvent.PossessionChanged(
                TeamId.None,
                nextPossession,
                PossessionChangeReason.ExplicitRestart));
        }

        public void AdvanceClock(TimeSpan delta)
        {
            if (delta < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delta));
            }

            if (State.Phase == MatchPhase.NotStarted)
            {
                throw new InvalidOperationException("Match clock cannot advance before match start.");
            }

            if (State.Phase == MatchPhase.Finished)
            {
                throw new InvalidOperationException("Match clock cannot advance after match finish.");
            }

            bool wasExpired = State.IsClockExpired;
            TimeSpan elapsed = State.ElapsedTime + delta;
            if (elapsed > State.Duration)
            {
                elapsed = State.Duration;
            }

            State = NewState(
                State.Phase,
                State.Possession,
                State.ActionsUsedInPossession,
                State.Score,
                elapsed);

            if (!wasExpired && State.IsClockExpired)
            {
                _events.Add(MatchDomainEvent.MatchClockExpired(State.Duration));
            }
        }

        public void FinishMatch(MatchFinishReason reason)
        {
            if (reason == MatchFinishReason.None)
            {
                throw new ArgumentOutOfRangeException(nameof(reason));
            }

            if (State.Phase == MatchPhase.NotStarted || State.Phase == MatchPhase.Finished)
            {
                throw new InvalidOperationException("Only an active match can be finished.");
            }

            if (reason == MatchFinishReason.TimeExpired && !State.IsClockExpired)
            {
                throw new InvalidOperationException("TimeExpired requires an expired match clock.");
            }

            _pendingAction = null;
            State = NewState(MatchPhase.Finished, TeamId.None, 0, State.Score, State.ElapsedTime);
            _events.Add(MatchDomainEvent.MatchFinished(reason, State.Score));
        }

        public IReadOnlyList<MatchDomainEvent> DrainEvents()
        {
            MatchDomainEvent[] result = _events.ToArray();
            _events.Clear();
            return result;
        }

        private void TransferPossession(PossessionChangeReason reason)
        {
            TeamId previous = State.Possession;
            TeamId next = OpponentOf(previous);
            State = NewState(MatchPhase.AwaitingAction, next, 0, State.Score, State.ElapsedTime);
            _events.Add(MatchDomainEvent.PossessionChanged(previous, next, reason));
        }

        private MatchState NewState(
            MatchPhase phase,
            TeamId possession,
            int actionsUsedInPossession,
            MatchScore score,
            TimeSpan elapsedTime)
        {
            return new MatchState(
                phase,
                possession,
                actionsUsedInPossession,
                score,
                State.Duration,
                elapsedTime);
        }

        private static TeamId OpponentOf(TeamId team)
        {
            switch (team)
            {
                case TeamId.TeamA:
                    return TeamId.TeamB;
                case TeamId.TeamB:
                    return TeamId.TeamA;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team));
            }
        }
    }
}
