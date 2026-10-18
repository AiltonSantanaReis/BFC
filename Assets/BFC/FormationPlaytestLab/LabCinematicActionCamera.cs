using System;
using BFC.Core.Fields;
using BFC.Core.Matches;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Presentation-only camera for experimental playable FormationLab.
    /// Never modifies transforms, velocities, collision callbacks or match outcomes
    /// for game pieces. The default gameplay aim stays in tactical overview.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LabCinematicActionCamera : MonoBehaviour
    {
        private Camera _camera;
        private Transform _ball;
        private Vector3 _overviewPosition;
        private Quaternion _overviewRotation;
        private float _overviewFov;
        private float _scale;
        private float _goalLingerSeconds;
        private float _forwardSign = 1f;
        private bool _active;
        private bool _returning;
        private bool _enabled = true;

        public bool CinematicEnabled => _enabled;
        public bool Following => _active;
        public bool IsTacticalViewReady => !_active && !_returning;
        public bool IsReturning => _returning;

        public void Initialize(Camera camera, Transform ball, FieldDefinition field)
        {
            if (_camera != null) throw new InvalidOperationException("Camera director already initialized.");
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            _ball = ball != null ? ball : throw new ArgumentNullException(nameof(ball));
            if (field == null) throw new ArgumentNullException(nameof(field));

            _scale = Mathf.Clamp(field.Length / 28f, 1f, 2.5f);
            _overviewPosition = _camera.transform.position;
            _overviewRotation = _camera.transform.rotation;
            _overviewFov = _camera.fieldOfView;
        }

        public void Toggle()
        {
            _enabled = !_enabled;
            if (!_enabled) RestoreTacticalImmediately();
        }

        public void BeginAction(TeamId actingTeam)
        {
            if (_camera == null) return;
            if (actingTeam != TeamId.TeamA && actingTeam != TeamId.TeamB)
                throw new ArgumentOutOfRangeException(nameof(actingTeam));

            if (!_enabled) return;
            _forwardSign = actingTeam == TeamId.TeamA ? 1f : -1f;
            _active = true;
            _returning = false;
            _goalLingerSeconds = 0f;
        }

        public void FinishAction(bool wasGoal)
        {
            if (!_active) return;
            if (wasGoal)
            {
                // A brief moment at ball level celebrates the *actual* physical result.
                _goalLingerSeconds = 1.25f;
            }
            else
            {
                _active = false;
                _returning = true;
            }
        }

        public void RestoreTacticalImmediately()
        {
            _active = false;
            _returning = false;
            _goalLingerSeconds = 0f;
            if (_camera == null) return;
            _camera.transform.SetPositionAndRotation(_overviewPosition, _overviewRotation);
            _camera.fieldOfView = _overviewFov;
        }

        private void LateUpdate()
        {
            if (_camera == null || _ball == null) return;

            if (_active)
            {
                // Camera remains behind the attack, close to the tabletop.
                // Frame offsets are artistic only. No physical object is moved.
                Vector3 pos = _ball.position +
                    new Vector3(-_forwardSign * 4.7f * _scale, 2.15f * _scale, -3.9f * _scale);
                Vector3 lookAt = _ball.position +
                    new Vector3(_forwardSign * 1.65f * _scale, 0.18f * _scale, 0f);
                Vector3 forward = lookAt - pos;
                if (forward.sqrMagnitude < 0.0001f) return;
                Quaternion rot = Quaternion.LookRotation(forward.normalized, Vector3.up);
                float weight = 1f - Mathf.Exp(-5f * Time.unscaledDeltaTime);
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, pos, weight);
                _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, rot, weight);
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, 53f, weight);

                if (_goalLingerSeconds > 0f)
                {
                    _goalLingerSeconds -= Time.unscaledDeltaTime;
                    if (_goalLingerSeconds <= 0f)
                    {
                        _active = false;
                        _returning = true;
                    }
                }

                return;
            }

            if (!_returning) return;
            float t = 1f - Mathf.Exp(-9f * Time.unscaledDeltaTime);
            _camera.transform.position = Vector3.Lerp(
                _camera.transform.position, _overviewPosition, t);
            _camera.transform.rotation = Quaternion.Slerp(
                _camera.transform.rotation, _overviewRotation, t);
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _overviewFov, t);

            if (Vector3.Distance(_camera.transform.position, _overviewPosition) < 0.06f &&
                Quaternion.Angle(_camera.transform.rotation, _overviewRotation) < 0.65f &&
                Mathf.Abs(_camera.fieldOfView - _overviewFov) < 0.1f)
            {
                RestoreTacticalImmediately();
            }
        }
    }
}
