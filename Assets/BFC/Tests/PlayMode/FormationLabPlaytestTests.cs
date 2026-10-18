using System.Collections;
using System.Linq;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.FormationLab;
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
            Assert.That(playtest.TryLaunch(teamA, Vector3.right, 0.5f), Is.True);
            Assert.That(playtest.State.Phase, Is.EqualTo(MatchPhase.ResolvingAction));
            Assert.That(playtest.TryLaunch(teamA, Vector3.right, 0.5f), Is.False);

            Object.Destroy(host);
            yield return null;
        }
    }
}
