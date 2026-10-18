using System;
using UnityEngine;

namespace BFC.Physics
{
    public enum PhysicsBodyKind
    {
        Piece = 0,
        Ball = 1
    }

    /// <summary>
    /// Constrains a Rigidbody to the XZ gameplay plane and applies deterministic linear slowdown.
    /// Collision response remains PhysX-owned; post-collision speed is bounded every fixed step.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlanarKineticBody : MonoBehaviour
    {
        [SerializeField] private PhysicsBodyKind bodyKind = PhysicsBodyKind.Piece;
        [SerializeField, Min(0.001f)] private float bodyMass = 1f;
        [SerializeField, Min(0f)] private float deceleration = 4.8f;
        [SerializeField, Min(0.001f)] private float maxSpeed = 9f;
        [SerializeField, Min(0f)] private float restSpeed = 0.035f;

        private Rigidbody _body;
        private bool _airborne;

        public PhysicsBodyKind Kind => bodyKind;
        public Rigidbody Body => EnsureBody();

        public float PlanarSpeed
        {
            get
            {
                Vector3 velocity = EnsureBody().linearVelocity;
                return new Vector2(velocity.x, velocity.z).magnitude;
            }
        }

        public bool IsAirborne => _airborne;
        public bool IsAtRest => !_airborne && PlanarSpeed <= restSpeed;

        private void Awake()
        {
            ApplyBodyConfiguration();
        }

        private void FixedUpdate()
        {
            Rigidbody body = EnsureBody();
            Vector3 velocity = body.linearVelocity;
            Vector2 planar = new Vector2(velocity.x, velocity.z);
            float speed = planar.magnitude;

            if (speed <= restSpeed)
            {
                body.linearVelocity = _airborne
                    ? new Vector3(0f, velocity.y, 0f) : Vector3.zero;
                return;
            }

            float nextSpeed = PlanarMotionMath.StepSpeed(speed, deceleration, Time.fixedDeltaTime);
            nextSpeed = Mathf.Min(nextSpeed, maxSpeed);

            if (nextSpeed <= restSpeed)
            {
                body.linearVelocity = _airborne
                    ? new Vector3(0f, velocity.y, 0f) : Vector3.zero;
                return;
            }

            Vector2 nextPlanar = planar * (nextSpeed / speed);
            body.linearVelocity = new Vector3(nextPlanar.x,
                _airborne ? velocity.y : 0f, nextPlanar.y);
        }

        public void Configure(
            PhysicsBodyKind kind,
            float mass,
            float linearDeceleration,
            float speedLimit,
            float restingSpeed)
        {
            bodyKind = kind;
            bodyMass = Mathf.Max(0.001f, mass);
            deceleration = Mathf.Max(0f, linearDeceleration);
            maxSpeed = Mathf.Max(0.001f, speedLimit);
            restSpeed = Mathf.Max(0f, restingSpeed);
            ApplyBodyConfiguration();
        }

        public void Launch(Vector3 direction, float normalizedPower, float maxLaunchSpeed)
        {
            Vector3 planarDirection = new Vector3(direction.x, 0f, direction.z);
            if (planarDirection.sqrMagnitude <= 0.000001f)
            {
                return;
            }

            StopMotion();
            float targetSpeed = Mathf.Min(maxSpeed, Mathf.Clamp01(normalizedPower) * maxLaunchSpeed);
            Vector3 impulse = planarDirection.normalized * (bodyMass * targetSpeed);
            ApplyPlanarImpulse(impulse);
        }

        public void ApplyPlanarImpulse(Vector3 impulse)
        {
            Rigidbody body = EnsureBody();
            Vector3 planarImpulse = new Vector3(impulse.x, 0f, impulse.z);
            Vector3 current = body.linearVelocity;
            Vector3 next = new Vector3(current.x, 0f, current.z) + (planarImpulse / body.mass);

            float speed = new Vector2(next.x, next.z).magnitude;
            if (speed > maxSpeed)
            {
                next *= maxSpeed / speed;
            }

            body.linearVelocity = next;
        }

        /// <summary>
        /// Experimental chip: the ball remains dynamic under PhysX gravity.
        /// The field has no ball-floor collider yet; landing is handled separately.
        /// </summary>
        public void StartVerticalFlight(float upwardVelocity)
        {
            if (bodyKind != PhysicsBodyKind.Ball)
                throw new InvalidOperationException("Only balls may enter flight.");
            if (float.IsNaN(upwardVelocity) || float.IsInfinity(upwardVelocity) ||
                upwardVelocity <= 0f)
                throw new System.ArgumentOutOfRangeException(nameof(upwardVelocity));
            if (_airborne) throw new InvalidOperationException("Ball is already airborne.");

            Rigidbody body = EnsureBody();
            _airborne = true;
            body.constraints = RigidbodyConstraints.FreezeRotationX |
                               RigidbodyConstraints.FreezeRotationZ;
            body.useGravity = true;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Vector3 previous = body.linearVelocity;
            body.linearVelocity = new Vector3(previous.x, upwardVelocity, previous.z);
        }

        /// <summary>
        /// Re-enters planar motion when the dynamic ball descends through its
        /// ground center height. This early laboratory approximates the bounce
        /// with an inelastic landing; it is not final chip collision physics.
        /// </summary>
        public bool TryLand(float groundCenterY)
        {
            if (float.IsNaN(groundCenterY) || float.IsInfinity(groundCenterY) ||
                groundCenterY <= 0f)
                throw new System.ArgumentOutOfRangeException(nameof(groundCenterY));
            if (!_airborne) return false;

            Rigidbody body = EnsureBody();
            if (body.position.y > groundCenterY || body.linearVelocity.y > 0f)
                return false;

            Vector3 center = body.position;
            body.position = new Vector3(center.x, groundCenterY, center.z);
            Vector3 previous = body.linearVelocity;
            body.linearVelocity = new Vector3(previous.x, 0f, previous.z);
            RestorePlanarConstraints(body);
            return true;
        }

        public void StopMotion()
        {
            Rigidbody body = EnsureBody();
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            if (_airborne) RestorePlanarConstraints(body);
        }

        private void RestorePlanarConstraints(Rigidbody body)
        {
            _airborne = false;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.Discrete;
            body.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
        }

        private void ApplyBodyConfiguration()
        {
            Rigidbody body = EnsureBody();
            _airborne = false;
            body.mass = bodyMass;
            body.useGravity = false;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            // Phase 2 starts from the least invasive PhysX mode. CCD is enabled only
            // after the laboratory proves that tunnelling actually requires it.
            body.collisionDetectionMode = CollisionDetectionMode.Discrete;

            body.constraints =
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
        }

        private Rigidbody EnsureBody()
        {
            if (_body == null)
            {
                _body = GetComponent<Rigidbody>();
            }

            return _body;
        }
    }
}
