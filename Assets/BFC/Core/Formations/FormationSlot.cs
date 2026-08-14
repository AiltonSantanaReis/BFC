using System;

namespace BFC.Core.Formations
{
    /// <summary>
    /// One logical starting slot expressed in normalized field coordinates.
    /// Longitudinal -1 is Team A's own goal side and +1 is the opponent side.
    /// Lateral uses -1..+1 across field width.
    /// </summary>
    public readonly struct FormationSlot
    {
        public FormationSlot(string id, PieceRole role, float longitudinal, float lateral)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Formation slot id is required.", nameof(id));
            }

            if (!Enum.IsDefined(typeof(PieceRole), role))
            {
                throw new ArgumentOutOfRangeException(nameof(role));
            }

            ValidateNormalized(longitudinal, nameof(longitudinal));
            ValidateNormalized(lateral, nameof(lateral));

            Id = id;
            Role = role;
            Longitudinal = longitudinal;
            Lateral = lateral;
        }

        public string Id { get; }

        public PieceRole Role { get; }

        public float Longitudinal { get; }

        public float Lateral { get; }

        private static void ValidateNormalized(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || value < -1f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
