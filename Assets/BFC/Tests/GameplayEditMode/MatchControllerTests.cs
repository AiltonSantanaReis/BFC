using System;
using BFC.Core.Matches;
using BFC.Core.Rules;
using BFC.Gameplay.Matches;
using NUnit.Framework;

namespace BFC.Tests.GameplayEditMode
{
    public sealed class MatchControllerTests
    {
        [Test]
        public void StartMatch_UsesExplicitInitialPossession()
        {
            MatchController controller = CreateController();

            controller.StartMatch(TeamId.TeamA);

            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.AwaitingAction));
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.TeamA));
            Assert.That(controller.State.ActionsUsedInPossession, Is.Zero);
        }

        [Test]
        public void SubmitAction_RejectsTeamWithoutPossession()
        {
            MatchController controller = CreateController();
            controller.StartMatch(TeamId.TeamA);

            PlayerActionSubmissionResult result = controller.TrySubmitAction(
                new PlayerActionCommand(TeamId.TeamB, "B-01"));

            Assert.That(result.Accepted, Is.False);
            Assert.That(result.RejectionReason, Is.EqualTo(PlayerActionRejectionReason.TeamNotInPossession));
            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.AwaitingAction));
        }

        [Test]
        public void SubmitAction_BlocksNextActionUntilPhysicsResolves()
        {
            MatchController controller = CreateController();
            controller.StartMatch(TeamId.TeamA);

            PlayerActionSubmissionResult first = controller.TrySubmitAction(
                new PlayerActionCommand(TeamId.TeamA, "A-01"));
            PlayerActionSubmissionResult second = controller.TrySubmitAction(
                new PlayerActionCommand(TeamId.TeamA, "A-02"));

            Assert.That(first.Accepted, Is.True);
            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.ResolvingAction));
            Assert.That(controller.HasPendingPhysicalResolution, Is.True);
            Assert.That(second.Accepted, Is.False);
            Assert.That(second.RejectionReason, Is.EqualTo(PlayerActionRejectionReason.PhysicalResolutionPending));
        }

        [Test]
        public void ContinuePossession_TransfersOnlyAfterRulesetActionLimit()
        {
            MatchController controller = CreateController(maxActionsPerPossession: 3);
            controller.StartMatch(TeamId.TeamA);

            SubmitAndResolveContinue(controller, "A-01");
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.TeamA));
            Assert.That(controller.State.ActionsUsedInPossession, Is.EqualTo(1));

            SubmitAndResolveContinue(controller, "A-02");
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.TeamA));
            Assert.That(controller.State.ActionsUsedInPossession, Is.EqualTo(2));

            SubmitAndResolveContinue(controller, "A-03");
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.TeamB));
            Assert.That(controller.State.ActionsUsedInPossession, Is.Zero);
            Assert.That(ContainsPossessionChange(controller, PossessionChangeReason.ActionLimitReached), Is.True);
        }

        [Test]
        public void PhysicalResolution_CanTransferPossessionBeforeActionLimit()
        {
            MatchController controller = CreateController();
            controller.StartMatch(TeamId.TeamA);
            controller.TrySubmitAction(new PlayerActionCommand(TeamId.TeamA, "A-01"));

            controller.ResolvePhysicalAction(PhysicalActionResolution.TransferPossession());

            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.AwaitingAction));
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.TeamB));
            Assert.That(controller.State.ActionsUsedInPossession, Is.Zero);
            Assert.That(ContainsPossessionChange(controller, PossessionChangeReason.ResolutionTransfer), Is.True);
        }

        [Test]
        public void Goal_UpdatesScoreAndRequiresExplicitRestartPossession()
        {
            MatchController controller = CreateController();
            controller.StartMatch(TeamId.TeamA);
            controller.TrySubmitAction(new PlayerActionCommand(TeamId.TeamA, "A-01"));

            controller.ResolvePhysicalAction(PhysicalActionResolution.Goal(TeamId.TeamA));

            Assert.That(controller.State.Score.TeamA, Is.EqualTo(1));
            Assert.That(controller.State.Score.TeamB, Is.Zero);
            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.AwaitingRestart));
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.None));

            PlayerActionSubmissionResult blocked = controller.TrySubmitAction(
                new PlayerActionCommand(TeamId.TeamA, "A-02"));
            Assert.That(blocked.RejectionReason, Is.EqualTo(PlayerActionRejectionReason.RestartPending));

            controller.ResumeAfterRestart(TeamId.TeamB);
            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.AwaitingAction));
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.TeamB));
        }

        [Test]
        public void ClockExpiry_IsReportedWithoutInventingFinishTimingRule()
        {
            MatchController controller = CreateController(durationSeconds: 60);
            controller.StartMatch(TeamId.TeamA);
            controller.DrainEvents();

            controller.AdvanceClock(TimeSpan.FromSeconds(60));

            Assert.That(controller.State.IsClockExpired, Is.True);
            Assert.That(controller.State.RemainingTime, Is.EqualTo(TimeSpan.Zero));
            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.AwaitingAction));
            Assert.That(ContainsEvent(controller, MatchDomainEventType.MatchClockExpired), Is.True);

            controller.FinishMatch(MatchFinishReason.TimeExpired);
            Assert.That(controller.State.Phase, Is.EqualTo(MatchPhase.Finished));
        }

        [Test]
        public void FinishedMatch_RejectsFurtherPlayerActions()
        {
            MatchController controller = CreateController();
            controller.StartMatch(TeamId.TeamA);
            controller.FinishMatch(MatchFinishReason.Authority);

            PlayerActionSubmissionResult result = controller.TrySubmitAction(
                new PlayerActionCommand(TeamId.TeamA, "A-01"));

            Assert.That(result.Accepted, Is.False);
            Assert.That(result.RejectionReason, Is.EqualTo(PlayerActionRejectionReason.MatchFinished));
            Assert.That(controller.State.Possession, Is.EqualTo(TeamId.None));
        }

        private static MatchController CreateController(
            int maxActionsPerPossession = 3,
            int durationSeconds = 300)
        {
            var rules = new MatchRules(
                maxActionsPerPossession,
                automaticGoalkeeper: true,
                normalizeCompetitiveEquipment: true);
            return new MatchController(rules, TimeSpan.FromSeconds(durationSeconds));
        }

        private static void SubmitAndResolveContinue(MatchController controller, string pieceId)
        {
            TeamId team = controller.State.Possession;
            PlayerActionSubmissionResult result = controller.TrySubmitAction(new PlayerActionCommand(team, pieceId));
            Assert.That(result.Accepted, Is.True);
            controller.ResolvePhysicalAction(PhysicalActionResolution.ContinuePossession());
        }

        private static bool ContainsPossessionChange(
            MatchController controller,
            PossessionChangeReason reason)
        {
            foreach (MatchDomainEvent domainEvent in controller.DrainEvents())
            {
                if (domainEvent.Type == MatchDomainEventType.PossessionChanged &&
                    domainEvent.PossessionChangeReason == reason)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsEvent(MatchController controller, MatchDomainEventType type)
        {
            foreach (MatchDomainEvent domainEvent in controller.DrainEvents())
            {
                if (domainEvent.Type == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
