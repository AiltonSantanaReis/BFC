using System;
using BFC.FormationPlaytestLab;
using NUnit.Framework;
using UnityEngine;

namespace BFC.Tests.PlayMode
{
    public sealed class LabTacticalLaneTests
    {
        [Test]
        public void PassAlwaysHasLowerSpeedCap_AndDoesNotChangeAimPower()
        {
            float shot = LabKickProfile.ResolveMaximumSpeed(LabKickKind.Shot, 8.5f);
            float pass = LabKickProfile.ResolveMaximumSpeed(LabKickKind.ControlledPass, 8.5f);
            Assert.That(shot, Is.EqualTo(8.5f));
            Assert.That(pass, Is.GreaterThan(0f).And.LessThan(shot));
            Assert.That(pass, Is.EqualTo(8.5f * LabKickProfile.PassMaxSpeedRatio));
        }

        [Test]
        public void LanesDifferentiateObstructedOpenAndOffAxisRoutes()
        {
            Vector2 from = Vector2.zero;
            Vector2 to = new Vector2(10f, 0f);
            var blocked = LabTacticalLaneAnalyzer.Assess(from, to,
                new[] { new LabLaneObstacle(new Vector2(5f, 0f), 0.55f) },
                0.24f, 0.1f);
            Assert.That(blocked.IsGeometricallyClear, Is.False);
            Assert.That(blocked.PotentialBlockers, Is.EqualTo(1));
            Assert.That(blocked.MinimumClearance, Is.LessThan(0f));

            var clear = LabTacticalLaneAnalyzer.Assess(from, to,
                new[] { new LabLaneObstacle(new Vector2(5f, 3f), 0.55f) },
                0.24f, 0.1f);
            Assert.That(clear.IsGeometricallyClear, Is.True);
            Assert.That(clear.MinimumClearance, Is.GreaterThan(0f));

            var empty = LabTacticalLaneAnalyzer.Assess(from, to,
                Array.Empty<LabLaneObstacle>(), 0.24f, 0f);
            Assert.That(empty.PotentialBlockers, Is.Zero);
            Assert.That(empty.SegmentLength, Is.EqualTo(10f));
        }

        [Test]
        public void SegmentEndpointsAndZeroLengthAreHandled()
        {
            var nearStart = LabTacticalLaneAnalyzer.Assess(Vector2.zero,
                new Vector2(10f, 0f),
                new[] { new LabLaneObstacle(new Vector2(-0.1f, 0f), 0.3f) },
                0.24f, 0f);
            Assert.That(nearStart.IsGeometricallyClear, Is.False);

            var zero = LabTacticalLaneAnalyzer.Assess(Vector2.zero, Vector2.zero,
                Array.Empty<LabLaneObstacle>(), 0.24f, 0f);
            Assert.That(zero.SegmentLength, Is.Zero);
        }

        [Test]
        public void InvalidParametersCannotProduceNanRecommendations()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabKickProfile.ResolveMaximumSpeed(LabKickKind.Shot, float.PositiveInfinity));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabKickProfile.ResolveMaximumSpeed((LabKickKind)40, 8.5f));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                LabTacticalLaneAnalyzer.Assess(Vector2.zero, new Vector2(float.NaN, 0f),
                    Array.Empty<LabLaneObstacle>(), 0.24f, 0f));
        }
    }
}
