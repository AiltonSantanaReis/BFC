using System.Collections;
using System.Linq;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFC.Tests.PlayMode
{
    public sealed class LabGoalkeeperShotExerciseTests
    {
        [UnityTest]
        public IEnumerator GoalkeeperMovesOnlyAfterExplicitAnnouncementAndCommit()
        {
            GameObject host = new GameObject("Keeper Exercise Test Host");
            var field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);
            Transform fixtures = FormationLabRuntimeBuilder.Build(host.transform,
                FormationLabFieldProfile.Tactical);
            yield return null;

            FormationPieceRuntime keeper = fixtures
                .GetComponentsInChildren<FormationPieceRuntime>()
                .First(p => p.Team == TeamId.TeamB && p.Role == PieceRole.Goalkeeper);
            Vector3 original = keeper.transform.position;
            var exercise = new LabGoalkeeperShotExercise(field, fixtures);

            Assert.That(exercise.TryPropose(TeamId.TeamB, 20f, 0f), Is.False);
            Assert.That(exercise.TryBegin(TeamId.TeamA, false), Is.False);
            Assert.That(exercise.TryBegin(TeamId.TeamA, true), Is.True);
            Assert.That(exercise.AuthorizedTeam, Is.EqualTo(TeamId.TeamB));
            Assert.That(exercise.TryPropose(TeamId.TeamA, 20f, 0f), Is.False);
            Assert.That(exercise.TryPropose(TeamId.TeamB, 0f, 0f), Is.False);
            Assert.That(exercise.TryPropose(TeamId.TeamB, 20f, 0f), Is.True);
            Assert.That(keeper.transform.position, Is.EqualTo(original));
            Assert.That(exercise.TryConfirm(TeamId.TeamA), Is.False);
            Assert.That(exercise.TryConfirm(TeamId.TeamB), Is.True);
            Assert.That(keeper.transform.position.x, Is.EqualTo(20f).Within(.001f));
            Assert.That(exercise.TryPropose(TeamId.TeamB, 19f, 0f), Is.False);

            Object.Destroy(host);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ExpiredWindowCannotMoveTheGoalkeeper()
        {
            GameObject host = new GameObject("Keeper Timeout Test Host");
            var field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);
            Transform fixtures = FormationLabRuntimeBuilder.Build(host.transform,
                FormationLabFieldProfile.Tactical);
            yield return null;

            FormationPieceRuntime keeper = fixtures
                .GetComponentsInChildren<FormationPieceRuntime>()
                .First(p => p.Team == TeamId.TeamB && p.Role == PieceRole.Goalkeeper);
            Vector3 original = keeper.transform.position;
            var exercise = new LabGoalkeeperShotExercise(field, fixtures);
            Assert.That(exercise.TryBegin(TeamId.TeamA, true), Is.True);
            Assert.That(exercise.TryPropose(TeamId.TeamB, 20f, 0f), Is.True);
            exercise.Advance(8f);
            Assert.That(exercise.TryConfirm(TeamId.TeamB), Is.False);
            Assert.That(keeper.transform.position, Is.EqualTo(original));

            Object.Destroy(host);
            yield return null;
        }
    }
}
