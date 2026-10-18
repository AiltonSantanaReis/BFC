using System;
using System.Collections.Generic;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Temporary laboratory choice, not a new competitive mode/ruleset.
    /// Direction and power remain completely player-controlled.
    /// </summary>
    public enum LabKickKind
    {
        Shot = 0,
        ControlledPass = 1,
        Chip = 2
    }

    public static class LabKickProfile
    {
        // Starting tuning hypothesis only. Evaluate distance distributions before approval.
        public const float PassMaxSpeedRatio = 0.55f;

        public static float ResolveMaximumSpeed(LabKickKind kind, float shotMaximumSpeed)
        {
            if (float.IsNaN(shotMaximumSpeed) || float.IsInfinity(shotMaximumSpeed) ||
                shotMaximumSpeed <= 0f)
                throw new ArgumentOutOfRangeException(nameof(shotMaximumSpeed));

            switch (kind)
            {
                case LabKickKind.Shot: return shotMaximumSpeed;
                case LabKickKind.ControlledPass: return shotMaximumSpeed * PassMaxSpeedRatio;
                case LabKickKind.Chip: return shotMaximumSpeed * 0.75f;
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }
    }

    public readonly struct LabLaneObstacle
    {
        public LabLaneObstacle(Vector2 position, float radius)
        {
            if (radius <= 0f || float.IsNaN(radius) || float.IsInfinity(radius))
                throw new ArgumentOutOfRangeException(nameof(radius));
            if (!IsFinite(position))
                throw new ArgumentOutOfRangeException(nameof(position));
            Position = position;
            Radius = radius;
        }

        public Vector2 Position { get; }
        public float Radius { get; }

        internal static bool IsFinite(Vector2 vector) =>
            !float.IsNaN(vector.x) && !float.IsNaN(vector.y) &&
            !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y);
    }

    public readonly struct LabLaneAssessment
    {
        internal LabLaneAssessment(float length, int blockers, float minimumClearance)
        {
            SegmentLength = length;
            PotentialBlockers = blockers;
            MinimumClearance = minimumClearance;
        }

        public float SegmentLength { get; }
        public int PotentialBlockers { get; }
        // Signed distance to nearest obstacle envelope; + means separation, - means overlap.
        public float MinimumClearance { get; }
        public bool IsGeometricallyClear => PotentialBlockers == 0;
    }

    /// <summary>
    /// Straight-line geometry ONLY. Does not predict rebounds, actual passing
    /// accuracy, possession, goalkeeper actions or probability of completing a pass.
    /// Target/source pieces must be excluded by the caller.
    /// </summary>
    public static class LabTacticalLaneAnalyzer
    {
        public static LabLaneAssessment Assess(
            Vector2 start, Vector2 end,
            IReadOnlyList<LabLaneObstacle> otherPieces,
            float ballRadius, float safetyPadding)
        {
            if (!LabLaneObstacle.IsFinite(start) || !LabLaneObstacle.IsFinite(end))
                throw new ArgumentOutOfRangeException(nameof(start));
            if (otherPieces == null) throw new ArgumentNullException(nameof(otherPieces));
            if (float.IsNaN(ballRadius) || float.IsInfinity(ballRadius) || ballRadius <= 0f)
                throw new ArgumentOutOfRangeException(nameof(ballRadius));
            if (float.IsNaN(safetyPadding) || float.IsInfinity(safetyPadding) || safetyPadding < 0f)
                throw new ArgumentOutOfRangeException(nameof(safetyPadding));

            Vector2 segment = end - start;
            float squared = segment.sqrMagnitude;
            int blockers = 0;
            float smallest = float.PositiveInfinity;
            foreach (LabLaneObstacle obstacle in otherPieces)
            {
                float fraction = squared < 0.000001f
                    ? 0f
                    : Mathf.Clamp01(Vector2.Dot(obstacle.Position - start, segment) / squared);
                Vector2 nearest = start + segment * fraction;
                float clearance = Vector2.Distance(obstacle.Position, nearest)
                    - obstacle.Radius - ballRadius - safetyPadding;
                smallest = Mathf.Min(smallest, clearance);
                if (clearance <= 0f) blockers++;
            }

            return new LabLaneAssessment(Mathf.Sqrt(squared), blockers, smallest);
        }
    }
}
