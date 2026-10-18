using System;
using BFC.Core.Fields;
using BFC.Core.Matches;

namespace BFC.Core.Rules
{
    /// <summary>
    /// Event types that MAY permit positioning in a separately configured ruleset.
    /// These are not universal BFC permissions or an interpretation of OPEN decisions.
    /// </summary>
    public enum GoalkeeperWindowKind
    {
        ShotDeclared = 1,
        SetPiece = 2,
        TacticalAdjustment = 3
    }

    /// <summary>
    /// Fail-closed policy: by default no one may reposition the goalkeeper.
    /// Automatic modes NEVER accept a manual positioning window.
    /// </summary>
    public sealed class GoalkeeperControlPolicy
    {
        private readonly GoalkeeperWindowKind[] _allowed;

        public GoalkeeperControlPolicy(bool automatic, float durationSeconds,
            params GoalkeeperWindowKind[] allowed)
        {
            if (float.IsNaN(durationSeconds) || float.IsInfinity(durationSeconds) ||
                durationSeconds < 0f)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));

            allowed = allowed ?? Array.Empty<GoalkeeperWindowKind>();
            if (automatic && allowed.Length != 0)
                throw new ArgumentException(
                    "Automatic goalkeeper mode must not grant manual positioning.", nameof(allowed));
            if (allowed.Length > 0 && durationSeconds <= 0f)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            for (int i = 0; i < allowed.Length; i++)
            {
                if (!Enum.IsDefined(typeof(GoalkeeperWindowKind), allowed[i]))
                    throw new ArgumentOutOfRangeException(nameof(allowed));
                for (int j = 0; j < i; j++)
                    if (allowed[i] == allowed[j])
                        throw new ArgumentException("Duplicate goalkeeper permission.", nameof(allowed));
            }

            Automatic = automatic;
            DurationSeconds = durationSeconds;
            _allowed = (GoalkeeperWindowKind[])allowed.Clone();
        }

        public static GoalkeeperControlPolicy AutomaticCompetitive =>
            new GoalkeeperControlPolicy(true, 0f);

        public static GoalkeeperControlPolicy ManualLocked =>
            new GoalkeeperControlPolicy(false, 0f);

        public bool Automatic { get; }
        public float DurationSeconds { get; }

        public bool Allows(GoalkeeperWindowKind kind)
        {
            if (!Enum.IsDefined(typeof(GoalkeeperWindowKind), kind))
                return false;
            for (int i = 0; i < _allowed.Length; i++)
                if (_allowed[i] == kind) return true;
            return false;
        }
    }

    /// <summary>
    /// Engine-independent authorization and bounded placement for goalkeepers.
    /// The caller MUST map actual ruleset events to TryOpen: this class cannot
    /// decide whether a shot was announced or a restart is legal.
    /// The physical piece is never moved until TryCommit returns true.
    /// </summary>
    public sealed class GoalkeeperPlacementWindow
    {
        private readonly FieldDefinition _field;
        private readonly GoalkeeperControlPolicy _policy;
        private float _pendingX;
        private float _pendingZ;
        private bool _hasPending;

        public GoalkeeperPlacementWindow(FieldDefinition field, GoalkeeperControlPolicy policy)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        public TeamId AuthorizedTeam { get; private set; } = TeamId.None;
        public GoalkeeperWindowKind WindowKind { get; private set; }
        public float TimeRemaining { get; private set; }
        public bool IsOpen { get; private set; }
        public bool HasPendingPlacement => IsOpen && _hasPending;

        public bool TryOpen(GoalkeeperWindowKind kind, TeamId authorizedTeam,
            bool allBodiesAtRest)
        {
            if (IsOpen || !allBodiesAtRest || !_policy.Allows(kind) ||
                authorizedTeam == TeamId.None ||
                (authorizedTeam != TeamId.TeamA && authorizedTeam != TeamId.TeamB))
                return false;

            AuthorizedTeam = authorizedTeam;
            WindowKind = kind;
            TimeRemaining = _policy.DurationSeconds;
            _hasPending = false;
            IsOpen = true;
            return true;
        }

        /// <summary>Rejects moving the entire disc outside its own goal area.</summary>
        public bool TrySetPosition(TeamId requestingTeam, float x, float z, float radius)
        {
            if (!IsOpen || TimeRemaining <= 0f || requestingTeam != AuthorizedTeam ||
                !Finite(x) || !Finite(z) || !Finite(radius) || radius <= 0f)
                return false;

            float longitudinal = requestingTeam == TeamId.TeamA ? -x : x;
            float min = _field.HalfLength - _field.GoalAreaLength + radius;
            float max = _field.HalfLength - radius;
            float maxLateral = _field.GoalAreaWidth * 0.5f - radius;
            if (min > max || maxLateral < 0f ||
                longitudinal < min || longitudinal > max ||
                Math.Abs(z) > maxLateral)
                return false;

            _pendingX = x;
            _pendingZ = z;
            _hasPending = true;
            return true;
        }

        public bool TryCommit(TeamId requestingTeam, out float x, out float z)
        {
            x = 0f;
            z = 0f;
            if (!IsOpen || !_hasPending || TimeRemaining <= 0f ||
                requestingTeam != AuthorizedTeam)
                return false;

            x = _pendingX;
            z = _pendingZ;
            Close();
            return true;
        }

        public void Advance(float seconds)
        {
            if (!Finite(seconds) || seconds < 0f)
                throw new ArgumentOutOfRangeException(nameof(seconds));
            if (!IsOpen) return;
            TimeRemaining = Math.Max(0f, TimeRemaining - seconds);
            if (TimeRemaining <= 0f) Close();
        }

        public void Close()
        {
            IsOpen = false;
            AuthorizedTeam = TeamId.None;
            TimeRemaining = 0f;
            _hasPending = false;
        }

        private static bool Finite(float v) => !float.IsNaN(v) && !float.IsInfinity(v);
    }
}
