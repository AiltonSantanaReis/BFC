using System;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.Physics;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Experimental chip: after a valid contact the REAL dynamic Rigidbody ball
    /// enters gravity-driven Y flight, while pieces stay planar.
    /// No scripted parabola or displacement of a visual-only ball.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(125)]
    [RequireComponent(typeof(PlanarKineticBody))]
    public sealed class LabBallChipFlight : MonoBehaviour
    {
        private PlanarKineticBody _ball;
        private TeamId _team;
        private string _pieceId;
        private float _upwardsSpeed;
        private bool _armed;

        public bool IsArmed => _armed;
        public bool IsAirborne => _ball != null && _ball.IsAirborne;

        private void Awake()
        {
            _ball = GetComponent<PlanarKineticBody>();
        }

        public void Arm(TeamId team, string pieceId, float upwardVelocity)
        {
            if (team != TeamId.TeamA && team != TeamId.TeamB)
                throw new ArgumentOutOfRangeException(nameof(team));
            if (string.IsNullOrWhiteSpace(pieceId))
                throw new ArgumentException("Piece identity required.", nameof(pieceId));
            if (float.IsNaN(upwardVelocity) || float.IsInfinity(upwardVelocity) ||
                upwardVelocity <= 0f)
                throw new ArgumentOutOfRangeException(nameof(upwardVelocity));

            _armed = false;
            if (_ball.IsAirborne) return;
            _team = team;
            _pieceId = pieceId;
            _upwardsSpeed = upwardVelocity;
            _armed = true;
        }

        public bool OnPieceContact(FormationPieceRuntime piece)
        {
            if (!_armed || piece == null || piece.Team != _team ||
                piece.SlotId != _pieceId || _ball.IsAirborne)
                return false;

            _armed = false;
            _ball.StartVerticalFlight(_upwardsSpeed);
            return true;
        }

        private void FixedUpdate()
        {
            if (_ball != null && _ball.IsAirborne)
                _ball.TryLand(PhysicsLabTuning.BallRadius);
        }

        public void Clear()
        {
            _armed = false;
            _team = TeamId.None;
            _pieceId = null;
            _upwardsSpeed = 0f;
            // Do not stop or teleport a flying ball: the match controller
            // must first decide whether the simulation should stop.
        }
    }
}
