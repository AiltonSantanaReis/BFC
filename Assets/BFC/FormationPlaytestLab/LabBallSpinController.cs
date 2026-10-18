using System;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.Physics;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Simplified experimental planar Magnus acceleration. Direction and velocity
    /// are physical simulation values, not a visual-only distortion. No spin
    /// means the original Phase-2 motion is retained unchanged.
    /// </summary>
    public static class LabSpinMotion
    {
        public static Vector3 Step(Vector3 velocity, float signedSpin,
            float curvatureCoefficient, float fixedDeltaSeconds, float maxSpeed)
        {
            if (!Finite(velocity.x) || !Finite(velocity.y) || !Finite(velocity.z) ||
                !Finite(signedSpin) || !Finite(curvatureCoefficient) ||
                !Finite(fixedDeltaSeconds) || !Finite(maxSpeed) ||
                curvatureCoefficient < 0f || fixedDeltaSeconds <= 0f || maxSpeed <= 0f)
                throw new ArgumentOutOfRangeException(nameof(velocity), "Nonfinite or invalid spin parameters.");

            Vector2 planar = new Vector2(velocity.x, velocity.z);
            if (planar.sqrMagnitude < 0.000001f || signedSpin == 0f ||
                curvatureCoefficient == 0f)
                return new Vector3(velocity.x, 0f, velocity.z);

            // right-handed 90-degree turn in the XZ game plane
            Vector2 transverse = new Vector2(-planar.y, planar.x);
            Vector2 updated = planar + transverse *
                (signedSpin * curvatureCoefficient * fixedDeltaSeconds);
            float speed = updated.magnitude;
            if (speed > maxSpeed) updated *= maxSpeed / speed;
            return new Vector3(updated.x, 0f, updated.y);
        }

        private static bool Finite(float f) => !float.IsNaN(f) && !float.IsInfinity(f);
    }

    /// <summary>
    /// This component may ONLY arm on a legal player launch and activates spin
    /// after a collider belonging to that exact selected piece contacts the ball.
    /// No direct positional editing; PhysX still resolves every collision.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(120)]
    [RequireComponent(typeof(PlanarKineticBody))]
    public sealed class LabBallSpinController : MonoBehaviour
    {
        private const float ExperimentalSpinSpeed = 11f;
        private const float SpinDecayPerSecond = 5f;
        private const float ExperimentalCurvature = 0.055f;

        private PlanarKineticBody _body;
        private TeamId _armedTeam;
        private string _armedPiece;
        private int _armedDirection;
        private float _spin;
        private bool _armed;

        public float CurrentSignedSpin => _spin;
        public bool IsArmed => _armed;

        private void Awake()
        {
            _body = GetComponent<PlanarKineticBody>();
        }

        public void Arm(TeamId team, string pieceId, int direction)
        {
            if (team != TeamId.TeamA && team != TeamId.TeamB)
                throw new ArgumentOutOfRangeException(nameof(team));
            if (string.IsNullOrWhiteSpace(pieceId))
                throw new ArgumentException("A piece id is required.", nameof(pieceId));
            if (direction < -1 || direction > 1)
                throw new ArgumentOutOfRangeException(nameof(direction));

            Clear();
            if (direction == 0) return;
            _armed = true;
            _armedTeam = team;
            _armedPiece = pieceId;
            _armedDirection = direction;
        }

        public bool OnPieceContact(FormationPieceRuntime piece)
        {
            if (!_armed || piece == null ||
                piece.Team != _armedTeam || piece.SlotId != _armedPiece)
                return false;

            _armed = false;
            _spin = ExperimentalSpinSpeed * _armedDirection;
            return true;
        }

        private void FixedUpdate()
        {
            if (_body == null || _body.Kind != PhysicsBodyKind.Ball || _spin == 0f)
                return;

            Rigidbody body = _body.Body;
            if (_body.IsAtRest)
            {
                _spin = 0f;
                body.angularVelocity = Vector3.zero;
                return;
            }

            body.linearVelocity = LabSpinMotion.Step(
                body.linearVelocity, _spin, ExperimentalCurvature,
                Time.fixedDeltaTime, PhysicsLabTuning.BallMaxSpeed);
            _spin = Mathf.MoveTowards(_spin, 0f, SpinDecayPerSecond * Time.fixedDeltaTime);
            body.angularVelocity = new Vector3(0f, _spin, 0f);
        }

        public void Clear()
        {
            _armed = false;
            _armedTeam = TeamId.None;
            _armedPiece = null;
            _armedDirection = 0;
            _spin = 0f;
            if (_body != null) _body.Body.angularVelocity = Vector3.zero;
        }
    }
}
