using System;
using BFC.Core.Fields;
using BFC.Core.Matches;
using BFC.Core.Rules;
using BFC.FormationLab;
using BFC.Physics;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Opt-in shot-declaration exercise using the fail-closed Core goalkeeper
    /// policy. Does not enact Classic or competitive goalkeeper rules.
    /// </summary>
    public sealed class LabGoalkeeperShotExercise
    {
        private readonly Transform _fixtures;
        private readonly GoalkeeperPlacementWindow _window;
        private FormationPieceRuntime _keeper;
        private float _candidateX;
        private float _candidateZ;

        public LabGoalkeeperShotExercise(FieldDefinition field, Transform fixtures)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            _fixtures = fixtures != null ? fixtures : throw new ArgumentNullException(nameof(fixtures));
            // 6 seconds is experimental and does not represent an approved modality.
            _window = new GoalkeeperPlacementWindow(field,
                new GoalkeeperControlPolicy(false, 6f,
                    GoalkeeperWindowKind.ShotDeclared));
        }

        public bool HasActiveWindow => _window.IsOpen;
        public bool HasCandidate => _window.HasPendingPlacement;
        public TeamId AuthorizedTeam => _window.AuthorizedTeam;
        public float RemainingSeconds => _window.TimeRemaining;
        public float CandidateX => _candidateX;
        public float CandidateZ => _candidateZ;

        public bool TryBegin(TeamId attackingTeam, bool bodiesAtRest)
        {
            TeamId defender = attackingTeam == TeamId.TeamA ? TeamId.TeamB
                : attackingTeam == TeamId.TeamB ? TeamId.TeamA : TeamId.None;
            if (defender == TeamId.None || !bodiesAtRest) return false;
            _keeper = null;

            foreach (FormationPieceRuntime piece in
                _fixtures.GetComponentsInChildren<FormationPieceRuntime>())
            {
                if (piece.Team == defender && piece.Role ==
                    BFC.Core.Formations.PieceRole.Goalkeeper)
                {
                    _keeper = piece;
                    break;
                }
            }
            return _keeper != null && _window.TryOpen(
                GoalkeeperWindowKind.ShotDeclared, defender, bodiesAtRest);
        }

        public bool TryPropose(TeamId requester, float x, float z)
        {
            if (_keeper == null || !_window.IsOpen || requester != _window.AuthorizedTeam ||
                float.IsNaN(x) || float.IsInfinity(x) ||
                float.IsNaN(z) || float.IsInfinity(z))
                return false;

            // A legal geometry target must also not overlap another physical piece.
            foreach (PlanarKineticBody body in
                _fixtures.GetComponentsInChildren<PlanarKineticBody>())
            {
                if (body.transform == _keeper.transform) continue;
                float radius = body.Kind == PhysicsBodyKind.Ball
                    ? PhysicsLabTuning.BallRadius : PhysicsLabTuning.PieceRadius;
                Vector3 current = body.transform.position;
                float dx = current.x - x;
                float dz = current.z - z;
                float limit = radius + PhysicsLabTuning.PieceRadius + 0.03f;
                if (dx * dx + dz * dz < limit * limit) return false;
            }

            if (!_window.TrySetPosition(requester, x, z, PhysicsLabTuning.PieceRadius))
                return false;
            _candidateX = x;
            _candidateZ = z;
            return true;
        }

        public bool TryConfirm(TeamId requester)
        {
            if (!_window.TryCommit(requester, out float x, out float z))
                return false;

            PlanarKineticBody physicalKeeper = _keeper.GetComponent<PlanarKineticBody>();
            if (physicalKeeper != null)
            {
                physicalKeeper.StopMotion();
                physicalKeeper.Body.position = new Vector3(
                    x, PhysicsLabTuning.PieceHeight * 0.5f, z);
            }
            else
            {
                _keeper.transform.position = new Vector3(
                    x, PhysicsLabTuning.PieceHeight * 0.5f, z);
            }

            UnityEngine.Physics.SyncTransforms();
            return true;
        }

        public void Advance(float seconds) => _window.Advance(seconds);

        public void Cancel()
        {
            _window.Close();
            _keeper = null;
        }
    }
}
