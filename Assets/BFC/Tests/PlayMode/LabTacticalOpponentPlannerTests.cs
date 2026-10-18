using System;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using NUnit.Framework;
using UnityEngine;

namespace BFC.Tests.PlayMode
{
    public sealed class LabTacticalOpponentPlannerTests
    {
        [Test]
        public void AiChoosesPieceBehindBallEvenWhenAnotherIsSlightlyCloser()
        {
            var pieces = new[]
            {
                new LabAIPlayer(TeamId.TeamB, "behind", PieceRole.Outfield, new Vector2(2f, 0f)),
                new LabAIPlayer(TeamId.TeamB, "ahead", PieceRole.Outfield, new Vector2(-1.7f, 0f)),
                new LabAIPlayer(TeamId.TeamB, "keeper", PieceRole.Goalkeeper, new Vector2(-10f, 5f))
            };
            Assert.That(LabTacticalOpponentPlanner.TryChoose(
                TeamId.TeamB, Vector2.zero,
                FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Tactical),
                pieces, out LabAIPlan plan), Is.True);
            Assert.That(plan.PieceId, Is.EqualTo("behind"));
            Assert.That(plan.Direction.x, Is.LessThan(-0.99f));
            Assert.That(plan.Power, Is.GreaterThan(0f).And.LessThanOrEqualTo(1f));
        }

        [Test]
        public void AiExcludesGoalkeeperOtherTeamAndEmptySlots()
        {
            var pieces = new[]
            {
                new LabAIPlayer(TeamId.TeamA, "A", PieceRole.Outfield, new Vector2(2f, 0f)),
                new LabAIPlayer(TeamId.TeamB, "GK", PieceRole.Goalkeeper, new Vector2(-5f, 0f)),
                new LabAIPlayer(TeamId.TeamB, "center", PieceRole.Outfield, Vector2.zero)
            };
            var field = FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Tactical);
            Assert.That(LabTacticalOpponentPlanner.TryChoose(TeamId.TeamB, Vector2.zero,
                field, pieces, out LabAIPlan unused), Is.False);
            Assert.That(unused.PieceId, Is.Null);
        }

        [Test]
        public void AiPrefersClearCorridorAndIsIndependentOfCandidateOrder()
        {
            var field = FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Original);
            var pieces = new[]
            {
                new LabAIPlayer(TeamId.TeamA, "blocked", PieceRole.Outfield, new Vector2(-5f, 0f)),
                new LabAIPlayer(TeamId.TeamA, "clear", PieceRole.Outfield, new Vector2(-5f, 3f)),
                new LabAIPlayer(TeamId.TeamB, "obstacle", PieceRole.Outfield, new Vector2(-2.5f, 0f))
            };
            Assert.That(LabTacticalOpponentPlanner.TryChoose(TeamId.TeamA, Vector2.zero,
                field, pieces, out LabAIPlan plan), Is.True);
            Assert.That(plan.PieceId, Is.EqualTo("clear"));
            Assert.That(plan.PotentialBlockers, Is.Zero);
        }

        [Test]
        public void AiRejectsInvalidInput()
        {
            var field = FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Original);
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabTacticalOpponentPlanner.TryChoose(TeamId.None, Vector2.zero,
                    field, Array.Empty<LabAIPlayer>(), out _));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabTacticalOpponentPlanner.TryChoose(TeamId.TeamB,
                    new Vector2(float.NaN, 0f), field, Array.Empty<LabAIPlayer>(), out _));
        }
    }
}
