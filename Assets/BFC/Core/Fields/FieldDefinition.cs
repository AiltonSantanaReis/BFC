using System;

namespace BFC.Core.Fields
{
    /// <summary>
    /// Engine-independent field geometry. Numeric dimensions are supplied by mode/content
    /// and are not universal BFC constants.
    /// </summary>
    public sealed class FieldDefinition
    {
        public FieldDefinition(
            string id,
            float length,
            float width,
            float goalMouthWidth,
            float goalDepth,
            float goalAreaLength,
            float goalAreaWidth)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Field id is required.", nameof(id));
            }

            RequirePositive(length, nameof(length));
            RequirePositive(width, nameof(width));
            RequirePositive(goalMouthWidth, nameof(goalMouthWidth));
            RequirePositive(goalDepth, nameof(goalDepth));
            RequirePositive(goalAreaLength, nameof(goalAreaLength));
            RequirePositive(goalAreaWidth, nameof(goalAreaWidth));

            if (goalMouthWidth >= width)
            {
                throw new ArgumentOutOfRangeException(nameof(goalMouthWidth));
            }

            if (goalAreaWidth < goalMouthWidth || goalAreaWidth > width)
            {
                throw new ArgumentOutOfRangeException(nameof(goalAreaWidth));
            }

            if (goalAreaLength >= length * 0.5f)
            {
                throw new ArgumentOutOfRangeException(nameof(goalAreaLength));
            }

            Id = id;
            Length = length;
            Width = width;
            GoalMouthWidth = goalMouthWidth;
            GoalDepth = goalDepth;
            GoalAreaLength = goalAreaLength;
            GoalAreaWidth = goalAreaWidth;
        }

        public string Id { get; }

        /// <summary>Goal-to-goal X-axis dimension.</summary>
        public float Length { get; }

        /// <summary>Side-to-side Z-axis dimension.</summary>
        public float Width { get; }

        public float GoalMouthWidth { get; }

        public float GoalDepth { get; }

        public float GoalAreaLength { get; }

        public float GoalAreaWidth { get; }

        public float HalfLength => Length * 0.5f;

        public float HalfWidth => Width * 0.5f;

        private static void RequirePositive(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value <= 0f)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
