using System;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.FormationPlaytestLab;
using BFC.Physics;
using NUnit.Framework;
using UnityEngine;

namespace BFC.Tests.PlayMode
{
    public sealed class LabBallSpinTests
    {
        [Test]
        public void SpinBendsInOppositeDirectionsButDoesNotChangeNoSpinBaseline()
        {
            Vector3 original = new Vector3(5f, 0f, 0f);
            Vector3 neutral = LabSpinMotion.Step(original, 0f, .1f, .02f, 14f);
            Vector3 positive = LabSpinMotion.Step(original, 10f, .1f, .02f, 14f);
            Vector3 negative = LabSpinMotion.Step(original, -10f, .1f, .02f, 14f);
            Assert.That(neutral, Is.EqualTo(original));
            Assert.That(positive.z, Is.GreaterThan(0f));
            Assert.That(negative.z, Is.LessThan(0f));
            Assert.That(positive.x, Is.EqualTo(negative.x).Within(.00001f));
            Assert.That(positive.magnitude, Is.LessThanOrEqualTo(14.0001f));
        }

        [Test]
        public void SpinRejectsInvalidNumbers()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabSpinMotion.Step(Vector3.right, float.NaN, .1f, .02f, 14f));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabSpinMotion.Step(Vector3.right, 1f, -.1f, .02f, 14f));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabSpinMotion.Step(Vector3.right, 1f, .1f, 0f, 14f));
        }

        [Test]
        public void SpinOnlyArmsFromExactLaunchedPieceAndCanBeCanceled()
        {
            var ball = new GameObject("Test Ball");
            var shooter = new GameObject("Shooter");
            var other = new GameObject("Other");
            try
            {
                ball.AddComponent<Rigidbody>();
                var body = ball.AddComponent<PlanarKineticBody>();
                body.Configure(PhysicsBodyKind.Ball, .35f, 2.4f, 14f, .035f);
                var spin = ball.AddComponent<LabBallSpinController>();
                spin.Arm(TeamId.TeamA, "D1", +1);

                var shooterId = shooter.AddComponent<FormationPieceRuntime>();
                shooterId.Initialize(TeamId.TeamA, "D1", BFC.Core.Formations.PieceRole.Outfield);
                var otherId = other.AddComponent<FormationPieceRuntime>();
                otherId.Initialize(TeamId.TeamB, "D1", BFC.Core.Formations.PieceRole.Outfield);
                Assert.That(spin.OnPieceContact(otherId), Is.False);
                Assert.That(spin.IsArmed, Is.True);
                Assert.That(spin.OnPieceContact(shooterId), Is.True);
                Assert.That(spin.CurrentSignedSpin, Is.GreaterThan(0f));
                Assert.That(spin.IsArmed, Is.False);
                spin.Clear();
                Assert.That(spin.CurrentSignedSpin, Is.Zero);
                Assert.That(spin.OnPieceContact(shooterId), Is.False);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(other);
                UnityEngine.Object.DestroyImmediate(shooter);
                UnityEngine.Object.DestroyImmediate(ball);
            }
        }
    }
}
