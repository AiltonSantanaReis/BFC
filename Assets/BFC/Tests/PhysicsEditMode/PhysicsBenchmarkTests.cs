using BFC.Physics;
using NUnit.Framework;

namespace BFC.Tests.PhysicsEditMode
{
    public sealed class PhysicsBenchmarkTests
    {
        [Test]
        public void ConstantDeceleration_NeverIncreasesSpeed()
        {
            float speed = 8.5f;
            for (int i = 0; i < 300; i++)
            {
                float next = PlanarMotionMath.StepSpeed(speed, 4.8f, 1f / 120f);
                Assert.That(next, Is.LessThanOrEqualTo(speed));
                speed = next;
            }
        }

        [Test]
        public void StopDistance_RemainsWithinTwoPercentAcrossFixedSteps()
        {
            StopBenchmarkResult step30 = PhysicsBenchmark.SimulateStop(
                PhysicsLabTuning.MaxLaunchSpeed,
                PhysicsLabTuning.PieceDeceleration,
                1f / 30f,
                PhysicsLabTuning.RestSpeed);
            StopBenchmarkResult step60 = PhysicsBenchmark.SimulateStop(
                PhysicsLabTuning.MaxLaunchSpeed,
                PhysicsLabTuning.PieceDeceleration,
                1f / 60f,
                PhysicsLabTuning.RestSpeed);
            StopBenchmarkResult step120 = PhysicsBenchmark.SimulateStop(
                PhysicsLabTuning.MaxLaunchSpeed,
                PhysicsLabTuning.PieceDeceleration,
                1f / 120f,
                PhysicsLabTuning.RestSpeed);

            float spread = PhysicsBenchmark.RelativeSpread(
                step30.Distance,
                step60.Distance,
                step120.Distance);

            Assert.That(spread, Is.LessThanOrEqualTo(PhysicsLabTuning.FixedStepBenchmarkTolerance));
        }

        [Test]
        public void StopBenchmark_RejectsZeroDeceleration()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                PhysicsBenchmark.SimulateStop(5f, 0f, 1f / 60f, 0.01f));
        }
    }
}
