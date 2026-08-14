namespace BFC.Physics
{
    /// <summary>
    /// Centralized Phase 2 laboratory calibration values.
    /// These are benchmark defaults, not final competitive tuning.
    /// </summary>
    public static class PhysicsLabTuning
    {
        public const float FieldWidth = 17f;
        public const float FieldLength = 11f;
        public const float WallThickness = 0.35f;

        public const float PieceRadius = 0.55f;
        public const float PieceHeight = 0.18f;
        public const float BallRadius = 0.24f;

        public const float PieceMass = 1f;
        public const float BallMass = 0.35f;
        public const float PieceDeceleration = 4.8f;
        public const float BallDeceleration = 2.4f;
        public const float RestSpeed = 0.035f;
        public const float PieceMaxSpeed = 9f;
        public const float BallMaxSpeed = 14f;

        public const float MaxDragDistance = 4f;
        public const float MaxLaunchSpeed = 8.5f;

        public const float FixedStepBenchmarkTolerance = 0.02f;
    }
}
