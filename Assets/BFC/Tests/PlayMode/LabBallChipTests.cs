using System;
using System.Collections;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using BFC.Physics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BFC.Tests.PlayMode
{
    public sealed class LabBallChipTests
    {
        [Test]
        public void BallAboveCrossbarIsNotScored_AndPlanarBallCanStillScore()
        {
            var field = FormationLabPreviewProfiles.CreateFieldPreview(
                FormationLabFieldProfile.Tactical);
            Assert.That(FormationLabGoalLine.TryDetect(field,
                new Vector3(20f, 2.5f, 0f), new Vector3(23f, 2.5f, 0f),
                PhysicsLabTuning.BallRadius, out _), Is.False);
            Assert.That(FormationLabGoalLine.TryDetect(field,
                new Vector3(20f, PhysicsLabTuning.BallRadius, 0f),
                new Vector3(23f, PhysicsLabTuning.BallRadius, 0f),
                PhysicsLabTuning.BallRadius, out _), Is.True);
        }

        [UnityTest]
        public IEnumerator ChipPhysicallyUsesGravityThenReturnsToPlanarMotion()
        {
            var ballObject = new GameObject("Ball chip test");
            ballObject.transform.position = new Vector3(0f, PhysicsLabTuning.BallRadius, 0f);
            var body = ballObject.AddComponent<PlanarKineticBody>();
            body.Configure(PhysicsBodyKind.Ball, PhysicsLabTuning.BallMass,
                PhysicsLabTuning.BallDeceleration, PhysicsLabTuning.BallMaxSpeed,
                PhysicsLabTuning.RestSpeed);
            ballObject.AddComponent<LabBallChipFlight>();
            body.StartVerticalFlight(3f);
            Assert.That(body.IsAirborne, Is.True);
            Assert.That(body.IsAtRest, Is.False);
            Assert.That(body.Body.useGravity, Is.True);

            bool sawRise = false;
            for (int step = 0; step < 180 && body.IsAirborne; step++)
            {
                yield return new WaitForFixedUpdate();
                if (ballObject.transform.position.y > PhysicsLabTuning.BallRadius + 0.05f)
                    sawRise = true;
            }

            Assert.That(sawRise, Is.True);
            Assert.That(body.IsAirborne, Is.False);
            Assert.That(body.Body.useGravity, Is.False);
            Assert.That(ballObject.transform.position.y,
                Is.EqualTo(PhysicsLabTuning.BallRadius).Within(0.05f));
            UnityEngine.Object.Destroy(ballObject);
            yield return null;
        }

        [Test]
        public void InvalidFlightSpeedIsRejected()
        {
            var ballObject = new GameObject("Chip validation ball");
            try
            {
                var body = ballObject.AddComponent<PlanarKineticBody>();
                body.Configure(PhysicsBodyKind.Ball, .35f, 2.4f, 14f, .035f);
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    body.StartVerticalFlight(float.NaN));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(ballObject);
            }
        }
    }
}
