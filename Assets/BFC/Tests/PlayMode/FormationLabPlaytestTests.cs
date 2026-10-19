using System.Collections;
using System.Linq;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using BFC.Gameplay.Matches;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFC.Tests.PlayMode
{
    public sealed class FormationLabPlaytestTests
    {
        [Test]
        public void SweptGoalLineDetectsBothEndsAndDoesNotCountWideShots()
        {
            FieldDefinition field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);

            Assert.That(FormationLabGoalLine.TryDetect(field,
                new Vector3(20f, 0f, 0f), new Vector3(23f, 0f, 0f),
                0.24f, out TeamId goalA), Is.True);
            Assert.That(goalA, Is.EqualTo(TeamId.TeamA));

            Assert.That(FormationLabGoalLine.TryDetect(field,
                new Vector3(-20f, 0f, 0f), new Vector3(-23f, 0f, 0f),
                0.24f, out TeamId goalB), Is.True);
            Assert.That(goalB, Is.EqualTo(TeamId.TeamB));

            Assert.That(FormationLabGoalLine.TryDetect(field,
                new Vector3(20f, 0f, 3f), new Vector3(23f, 0f, 3f),
                0.24f, out TeamId offTarget), Is.False);
            Assert.That(offTarget, Is.EqualTo(TeamId.None));
        }

        [UnityTest]
        public IEnumerator AnnouncedShotBlocksAttackerUntilDefenderCommits()
        {
            GameObject host = new GameObject("Goalkeeper Announcement Test Host");
            FieldDefinition field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);
            Transform fixtures = FormationLabRuntimeBuilder.Build(
                host.transform, FormationLabFieldProfile.Tactical);
            var playtest = host.AddComponent<FormationLabPlaytestController>();
            playtest.Initialize(fixtures, field, Camera.main);
            yield return null;

            FormationPieceRuntime shooter = fixtures
                .GetComponentsInChildren<FormationPieceRuntime>()
                .First(p => p.Team == TeamId.TeamA && p.Role == PieceRole.Outfield);
            FormationPieceRuntime goalkeeper = fixtures
                .GetComponentsInChildren<FormationPieceRuntime>()
                .First(p => p.Team == TeamId.TeamB && p.Role == PieceRole.Goalkeeper);

            Vector3 original = goalkeeper.transform.position;
            Assert.That(playtest.TryAnnounceShot(), Is.True);
            Assert.That(playtest.IsGoalkeeperWindowOpen, Is.True);
            Assert.That(playtest.TryLaunch(shooter, Vector3.right, 1f), Is.False);
            Assert.That(playtest.TrySetKickKind(LabKickKind.Chip), Is.False);
            Assert.That(playtest.TryProposeGoalkeeper(TeamId.TeamB, 20f, 0f), Is.True);
            Assert.That(goalkeeper.transform.position, Is.EqualTo(original));
            Assert.That(playtest.TryConfirmGoalkeeper(TeamId.TeamA), Is.False);
            Assert.That(playtest.TryConfirmGoalkeeper(TeamId.TeamB), Is.True);
            Assert.That(playtest.IsGoalkeeperWindowOpen, Is.False);
            Assert.That(goalkeeper.GetComponent<Rigidbody>().position.x,
                Is.EqualTo(20f).Within(.001f), "The committed placement must update the physics body immediately.");
            // Interpolated presentation follows the committed body on the next physics step.
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That(goalkeeper.transform.position.x, Is.EqualTo(20f).Within(.001f));
            Assert.That(playtest.TryLaunch(shooter, Vector3.right, 0.5f), Is.True);

            Object.Destroy(host);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ControlledPassHasReducedActualLaunchSpeed_AndLocksModeDuringAction()
        {
            GameObject host = new GameObject("BFC Pass Validation Host");
            FieldDefinition field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);
            Transform fixtures = FormationLabRuntimeBuilder.Build(
                host.transform, FormationLabFieldProfile.Tactical);

            var playtest = host.AddComponent<FormationLabPlaytestController>();
            playtest.Initialize(fixtures, field, Camera.main);
            yield return null;

            FormationPieceRuntime piece = fixtures.GetComponentsInChildren<FormationPieceRuntime>()
                .First(p => p.Team == TeamId.TeamA && p.Role == PieceRole.Outfield);
            var body = piece.GetComponent<BFC.Physics.PlanarKineticBody>();

            Assert.That(playtest.TrySetKickKind((LabKickKind)999), Is.False);
            Assert.That(playtest.TrySetKickKind(LabKickKind.ControlledPass), Is.True);
            Assert.That(playtest.CurrentKickKind, Is.EqualTo(LabKickKind.ControlledPass));
            Assert.That(playtest.TryLaunch(piece, Vector3.right, 1f), Is.True);
            Assert.That(body.PlanarSpeed, Is.EqualTo(
                LabKickProfile.ResolveMaximumSpeed(LabKickKind.ControlledPass,
                    BFC.Physics.PhysicsLabTuning.MaxLaunchSpeed)).Within(0.0001f));
            Assert.That(playtest.Telemetry.IsRecording, Is.True);
            Assert.That(playtest.TrySetKickKind(LabKickKind.Shot), Is.False);
            Assert.That(playtest.CurrentKickKind, Is.EqualTo(LabKickKind.ControlledPass));

            Object.Destroy(host);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlaytestAcceptsOwningOutfieldPieceAndBlocksGoalkeeperAndSecondShot()
        {
            GameObject host = new GameObject("BFC Playtest Validation Host");
            FieldDefinition field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);
            Transform fixtures = FormationLabRuntimeBuilder.Build(
                host.transform, FormationLabFieldProfile.Tactical);

            var playtest = host.AddComponent<FormationLabPlaytestController>();
            playtest.Initialize(fixtures, field, Camera.main);
            yield return null;

            FormationPieceRuntime[] pieces = fixtures.GetComponentsInChildren<FormationPieceRuntime>();
            FormationPieceRuntime teamA = pieces.First(p =>
                p.Team == TeamId.TeamA && p.Role == PieceRole.Outfield);
            FormationPieceRuntime teamB = pieces.First(p =>
                p.Team == TeamId.TeamB && p.Role == PieceRole.Outfield);
            FormationPieceRuntime goalkeeper = pieces.First(p =>
                p.Team == TeamId.TeamA && p.Role == PieceRole.Goalkeeper);

            Assert.That(playtest.State.Phase, Is.EqualTo(MatchPhase.AwaitingAction));
            Assert.That(playtest.TryLaunch(teamB, Vector3.right, 0.5f), Is.False);
            Assert.That(playtest.TryLaunch(goalkeeper, Vector3.right, 0.5f), Is.False);
            Assert.That(playtest.TryLaunch(teamA, Vector3.right, 0f), Is.False);
            Assert.That(playtest.TryLaunch(teamA, Vector3.up, 0.5f), Is.False);
            Assert.That(playtest.TryLaunch(teamA, Vector3.right, 0.5f), Is.True);
            Assert.That(playtest.State.Phase, Is.EqualTo(MatchPhase.ResolvingAction));
            Assert.That(playtest.TryLaunch(teamA, Vector3.right, 0.5f), Is.False);

            Object.Destroy(host);
            yield return null;
        }
    }
}
