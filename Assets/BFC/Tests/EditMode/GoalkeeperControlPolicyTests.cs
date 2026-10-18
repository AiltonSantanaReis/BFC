using System;
using BFC.Core.Fields;
using BFC.Core.Matches;
using BFC.Core.Rules;
using NUnit.Framework;

namespace BFC.Tests.EditMode
{
    public sealed class GoalkeeperControlPolicyTests
    {
        private static FieldDefinition Field() => new FieldDefinition(
            "test", 42f, 27f, 4.4f, 1.6f, 3.5f, 8f);

        [Test]
        public void CompetitiveAutomaticAndLockedManualFailClosed()
        {
            var automatic = new GoalkeeperPlacementWindow(Field(),
                GoalkeeperControlPolicy.AutomaticCompetitive);
            var locked = new GoalkeeperPlacementWindow(Field(),
                GoalkeeperControlPolicy.ManualLocked);
            Assert.That(automatic.TryOpen(GoalkeeperWindowKind.ShotDeclared, TeamId.TeamB, true),
                Is.False);
            Assert.That(locked.TryOpen(GoalkeeperWindowKind.ShotDeclared, TeamId.TeamB, true),
                Is.False);
            Assert.Throws<ArgumentException>(() =>
                new GoalkeeperControlPolicy(true, 5f, GoalkeeperWindowKind.ShotDeclared));
        }

        [Test]
        public void AnnouncedShotWindowAllowsOnlyAuthorizedTeamInBounds()
        {
            var policy = new GoalkeeperControlPolicy(false, 5f,
                GoalkeeperWindowKind.ShotDeclared);
            var window = new GoalkeeperPlacementWindow(Field(), policy);
            Assert.That(window.TryOpen(GoalkeeperWindowKind.SetPiece, TeamId.TeamB, true), Is.False);
            Assert.That(window.TryOpen(GoalkeeperWindowKind.ShotDeclared, TeamId.TeamB, false), Is.False);
            Assert.That(window.TryOpen(GoalkeeperWindowKind.ShotDeclared, TeamId.TeamB, true), Is.True);
            Assert.That(window.TryOpen(GoalkeeperWindowKind.ShotDeclared, TeamId.TeamA, true), Is.False);
            Assert.That(window.TrySetPosition(TeamId.TeamA, 20f, 0f, 0.55f), Is.False);
            Assert.That(window.TrySetPosition(TeamId.TeamB, 0f, 0f, 0.55f), Is.False);
            Assert.That(window.TrySetPosition(TeamId.TeamB, 20f, 4f, 0.55f), Is.False);
            Assert.That(window.TrySetPosition(TeamId.TeamB, 20f, 0f, 0.55f), Is.True);
            Assert.That(window.TryCommit(TeamId.TeamA, out _, out _), Is.False);
            Assert.That(window.TryCommit(TeamId.TeamB, out float x, out float z), Is.True);
            Assert.That(x, Is.EqualTo(20f));
            Assert.That(z, Is.Zero);
            Assert.That(window.IsOpen, Is.False);
            Assert.That(window.TrySetPosition(TeamId.TeamB, 20f, 0f, 0.55f), Is.False);
        }

        [Test]
        public void OppositeEndAndTimeoutNeverApplyUncommittedPosition()
        {
            var window = new GoalkeeperPlacementWindow(Field(),
                new GoalkeeperControlPolicy(false, 2f, GoalkeeperWindowKind.SetPiece));
            Assert.That(window.TryOpen(GoalkeeperWindowKind.SetPiece, TeamId.TeamA, true), Is.True);
            Assert.That(window.TrySetPosition(TeamId.TeamA, -20f, 0f, 0.55f), Is.True);
            window.Advance(2f);
            Assert.That(window.IsOpen, Is.False);
            Assert.That(window.TryCommit(TeamId.TeamA, out _, out _), Is.False);
            Assert.That(window.HasPendingPlacement, Is.False);
        }

        [Test]
        public void RejectsInvalidPermissionsAndNumericalValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new GoalkeeperControlPolicy(false, float.NaN));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new GoalkeeperControlPolicy(false, 0f, GoalkeeperWindowKind.ShotDeclared));
            Assert.Throws<ArgumentException>(() =>
                new GoalkeeperControlPolicy(false, 3f, GoalkeeperWindowKind.SetPiece,
                    GoalkeeperWindowKind.SetPiece));
            var window = new GoalkeeperPlacementWindow(Field(),
                new GoalkeeperControlPolicy(false, 5f, GoalkeeperWindowKind.ShotDeclared));
            Assert.That(window.TryOpen((GoalkeeperWindowKind)99, TeamId.TeamA, true), Is.False);
            Assert.That(window.TryOpen(GoalkeeperWindowKind.ShotDeclared, TeamId.TeamA, true), Is.True);
            Assert.That(window.TrySetPosition(TeamId.TeamA, -20f, float.NaN, .55f), Is.False);
            Assert.Throws<ArgumentOutOfRangeException>(() => window.Advance(-1f));
            window.Close();
            Assert.That(window.IsOpen, Is.False);
        }
    }
}
