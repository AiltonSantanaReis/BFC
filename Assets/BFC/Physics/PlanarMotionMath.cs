using System;

namespace BFC.Physics
{
    /// <summary>
    /// Small deterministic helpers shared by runtime motion and benchmark tests.
    /// </summary>
    public static class PlanarMotionMath
    {
        public static float StepSpeed(float currentSpeed, float deceleration, float deltaTime)
        {
            if (currentSpeed < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(currentSpeed));
            }

            if (deceleration < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deceleration));
            }

            if (deltaTime <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            float next = currentSpeed - (deceleration * deltaTime);
            return next > 0f ? next : 0f;
        }
    }
}
