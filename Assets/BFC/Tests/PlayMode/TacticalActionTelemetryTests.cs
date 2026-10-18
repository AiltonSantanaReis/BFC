using System;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using NUnit.Framework;
using UnityEngine;

namespace BFC.Tests.PlayMode
{
    public sealed class TacticalActionTelemetryTests
    {
        [Test]
        public void BallPathCanExceedDisplacement_AndProgressUsesCorrectTeamDirection()
        {
            var field = FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Tactical);
            var telemetry = new TacticalActionTelemetry(field);
            telemetry.Begin(TeamId.TeamA, "D1", 0.75f, Vector3.zero);
            telemetry.ObserveBallPosition(new Vector3(3f, 0f, 4f));
            telemetry.ObserveBallPosition(new Vector3(1f, 0f, 4f));

            telemetry.ObserveBallContact(TeamId.TeamA, "D1");
            telemetry.ObserveBallContact(TeamId.TeamA, "D1");
            telemetry.ObserveBallContact(TeamId.TeamB, "F2");
            TacticalActionRecord record = telemetry.Complete(
                TacticalActionEnd.Settled, new Vector3(1f, 0f, 4f));

            Assert.That(record.BallPathLength, Is.EqualTo(7f).Within(0.0001f));
            Assert.That(record.MaximumDisplacement, Is.EqualTo(5f).Within(0.0001f));
            Assert.That(record.MaximumForwardProgress, Is.EqualTo(3f).Within(0.0001f));
            Assert.That(record.NetForwardProgress, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(record.ContactEvents, Is.EqualTo(3));
            Assert.That(record.FriendlyContactEvents, Is.EqualTo(2));
            Assert.That(record.OpponentContactEvents, Is.EqualTo(1));
            Assert.That(record.UniqueContactedPieces, Is.EqualTo(2));
            Assert.That(record.LastContactTeam, Is.EqualTo(TeamId.TeamB));
            Assert.That(record.LastContactPieceId, Is.EqualTo("F2"));
            Assert.That(record.PathToFieldLengthRatio, Is.EqualTo(7f / 42f).Within(0.0001f));

            telemetry.Begin(TeamId.TeamB, "F1", 0.5f, Vector3.zero);
            telemetry.ObserveBallPosition(new Vector3(-6f, 0f, 0f));
            TacticalActionRecord teamB = telemetry.Complete(
                TacticalActionEnd.OutOfBounds, new Vector3(-6f, 0f, 0f));
            Assert.That(teamB.MaximumForwardProgress, Is.EqualTo(6f));
            Assert.That(teamB.NetForwardProgress, Is.EqualTo(6f));
        }

        [Test]
        public void AbortedActionIsNotRecorded_AndCsvHasInvariantNumericValues()
        {
            var telemetry = new TacticalActionTelemetry(
                FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Original));
            telemetry.Begin(TeamId.TeamA, "D1", 0.2f, Vector3.zero);
            telemetry.ObserveBallPosition(new Vector3(1f, 0f, 0f));
            telemetry.CancelActive();
            Assert.That(telemetry.Records.Count, Is.Zero);

            telemetry.Begin(TeamId.TeamB, "P,\"7", 0.25f, Vector3.zero);
            telemetry.ObserveBallPosition(new Vector3(-2.5f, 0f, 0f));
            telemetry.Complete(TacticalActionEnd.Goal, new Vector3(-2.5f, 0f, 0f));
            string csv = telemetry.ToCsv();
            Assert.That(csv, Does.Contain("\"P,\"\"7\""));
            Assert.That(csv, Does.Contain("2.5"));
            Assert.That(csv, Does.Contain("Goal"));
            Assert.That(telemetry.Records.Count, Is.EqualTo(1));
            Assert.That(telemetry.LastRecord.Sequence, Is.EqualTo(1));
        }

        [Test]
        public void InvalidStateAndSamplesAreRejected()
        {
            var telemetry = new TacticalActionTelemetry(
                FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Tactical));
            Assert.Throws<InvalidOperationException>(() =>
                telemetry.Complete(TacticalActionEnd.Settled, Vector3.zero));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                telemetry.Begin(TeamId.None, "D1", 0.5f, Vector3.zero));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                telemetry.Begin(TeamId.TeamA, "D1", float.NaN, Vector3.zero));

            telemetry.Begin(TeamId.TeamA, "D1", 0.5f, Vector3.zero);
            Assert.Throws<InvalidOperationException>(() =>
                telemetry.Begin(TeamId.TeamA, "D1", 0.5f, Vector3.zero));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                telemetry.ObserveBallPosition(new Vector3(float.PositiveInfinity, 0f, 0f)));
            telemetry.CancelActive();
        }
    }
}
