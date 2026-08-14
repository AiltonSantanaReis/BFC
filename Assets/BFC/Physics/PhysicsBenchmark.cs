using System;

namespace BFC.Physics
{
    public readonly struct StopBenchmarkResult
    {
        public StopBenchmarkResult(float distance, float elapsedTime, int steps)
        {
            Distance = distance;
            ElapsedTime = elapsedTime;
            Steps = steps;
        }

        public float Distance { get; }
        public float ElapsedTime { get; }
        public int Steps { get; }
    }

    /// <summary>
    /// Headless benchmark for the same constant-deceleration rule used by PlanarKineticBody.
    /// It intentionally does not model collisions; collision benchmarks are a separate gate.
    /// </summary>
    public static class PhysicsBenchmark
    {
        public static StopBenchmarkResult SimulateStop(
            float initialSpeed,
            float deceleration,
            float fixedDeltaTime,
            float restSpeed)
        {
            if (initialSpeed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(initialSpeed));
            }

            if (deceleration <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deceleration));
            }

            if (fixedDeltaTime <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(fixedDeltaTime));
            }

            if (restSpeed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(restSpeed));
            }

            float speed = initialSpeed;
            float distance = 0f;
            float elapsed = 0f;
            int steps = 0;

            while (speed > restSpeed)
            {
                speed = PlanarMotionMath.StepSpeed(speed, deceleration, fixedDeltaTime);
                distance += speed * fixedDeltaTime;
                elapsed += fixedDeltaTime;
                steps++;

                if (steps > 100000)
                {
                    throw new InvalidOperationException("Stop benchmark exceeded its safety step limit.");
                }
            }

            return new StopBenchmarkResult(distance, elapsed, steps);
        }

        public static float RelativeSpread(float a, float b, float c)
        {
            float min = Math.Min(a, Math.Min(b, c));
            float max = Math.Max(a, Math.Max(b, c));
            float average = (a + b + c) / 3f;
            return average <= 0f ? 0f : (max - min) / average;
        }
    }
}
