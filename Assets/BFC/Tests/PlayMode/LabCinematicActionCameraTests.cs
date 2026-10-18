using System.Collections;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFC.Tests.PlayMode
{
    public sealed class LabCinematicActionCameraTests
    {
        [UnityTest]
        public IEnumerator CameraFollowsOnlyDuringAction_AndRestoresWithoutMovingBall()
        {
            GameObject cameraObj = new GameObject("Experimental Camera Test");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.transform.SetPositionAndRotation(
                new Vector3(2f, 25f, -21f), Quaternion.Euler(55f, 0f, 0f));
            camera.fieldOfView = 47f;
            Vector3 overview = camera.transform.position;
            Quaternion rotation = camera.transform.rotation;

            GameObject ball = new GameObject("Ball - Camera Test");
            ball.transform.position = new Vector3(3f, 0.24f, 1f);
            Vector3 initialBall = ball.transform.position;

            var director = cameraObj.AddComponent<LabCinematicActionCamera>();
            director.Initialize(camera, ball.transform,
                FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Tactical));

            Assert.That(director.IsTacticalViewReady, Is.True);
            director.BeginAction(TeamId.TeamA);
            Assert.That(director.Following, Is.True);
            yield return null;
            Assert.That(director.IsTacticalViewReady, Is.False);
            Assert.That(ball.transform.position, Is.EqualTo(initialBall));

            director.FinishAction(false);
            Assert.That(director.IsReturning, Is.True);
            director.RestoreTacticalImmediately();
            Assert.That(camera.transform.position, Is.EqualTo(overview));
            Assert.That(Quaternion.Angle(camera.transform.rotation, rotation),
                Is.LessThan(0.001f));
            Assert.That(camera.fieldOfView, Is.EqualTo(47f));
            Assert.That(ball.transform.position, Is.EqualTo(initialBall));
            Assert.That(director.IsTacticalViewReady, Is.True);

            Object.Destroy(ball);
            Object.Destroy(cameraObj);
            yield return null;
        }

        [Test]
        public void CameraRejectsUndefinedTeams_AndCanBeDisabled()
        {
            GameObject cameraObj = new GameObject("Camera Test");
            GameObject ballObj = new GameObject("Ball Test");
            try
            {
                var camera = cameraObj.AddComponent<Camera>();
                var director = cameraObj.AddComponent<LabCinematicActionCamera>();
                director.Initialize(camera, ballObj.transform,
                    FormationLabPreviewProfiles.CreateFieldPreview(FormationLabFieldProfile.Original));

                Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                    director.BeginAction(TeamId.None));
                director.Toggle();
                Assert.That(director.CinematicEnabled, Is.False);
                director.BeginAction(TeamId.TeamA);
                Assert.That(director.Following, Is.False);
                director.Toggle();
                Assert.That(director.CinematicEnabled, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(ballObj);
                Object.DestroyImmediate(cameraObj);
            }
        }
    }
}
