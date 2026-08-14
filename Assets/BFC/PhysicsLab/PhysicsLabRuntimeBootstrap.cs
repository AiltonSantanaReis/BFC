using System;
using BFC.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BFC.PhysicsLab
{
    /// <summary>
    /// Runtime-only Phase 2 laboratory. Production visuals and match rules do not belong here.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class PhysicsLabRuntimeBootstrap : MonoBehaviour
    {
        private PlanarKineticBody _pieceA;
        private PlanarKineticBody _pieceB;
        private PlanarKineticBody _ball;
        private Vector3 _pieceAStart;
        private Vector3 _pieceBStart;
        private Vector3 _ballStart;

        private void Awake()
        {
            BuildScenario();
            LogFixedStepBenchmark();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetScenario();
            }
        }

        private void BuildScenario()
        {
            if (transform.Find("Runtime Fixtures") != null)
            {
                return;
            }

            Camera camera = Camera.main;
            if (camera == null)
            {
                throw new InvalidOperationException("PhysicsLab requires a camera tagged MainCamera.");
            }

            camera.transform.SetPositionAndRotation(
                new Vector3(0f, 12f, -10f),
                Quaternion.Euler(50f, 0f, 0f));
            camera.fieldOfView = 52f;

            Transform fixtures = new GameObject("Runtime Fixtures").transform;
            fixtures.SetParent(transform, false);

            Material fieldMaterial = CreateMaterial(new Color(0.035f, 0.09f, 0.08f));
            Material wallMaterial = CreateMaterial(new Color(0.05f, 0.08f, 0.12f));
            Material pieceAMaterial = CreateMaterial(new Color(0.05f, 0.85f, 1f));
            Material pieceBMaterial = CreateMaterial(new Color(1f, 0.08f, 0.72f));
            Material ballMaterial = CreateMaterial(new Color(0.92f, 0.95f, 1f));
            PhysicMaterial collisionMaterial = CreateCollisionMaterial();

            CreateField(fixtures, fieldMaterial, wallMaterial, collisionMaterial);

            _pieceAStart = new Vector3(-3.2f, PhysicsLabTuning.PieceHeight * 0.5f, 0f);
            _pieceBStart = new Vector3(3.2f, PhysicsLabTuning.PieceHeight * 0.5f, 0f);
            _ballStart = new Vector3(0f, PhysicsLabTuning.BallRadius, 0f);

            _pieceA = CreatePiece("Piece A", _pieceAStart, pieceAMaterial, collisionMaterial, fixtures);
            _pieceB = CreatePiece("Piece B", _pieceBStart, pieceBMaterial, collisionMaterial, fixtures);
            _ball = CreateBall("Ball", _ballStart, ballMaterial, collisionMaterial, fixtures);

            PhysicsLabDragLauncher launcher = gameObject.AddComponent<PhysicsLabDragLauncher>();
            launcher.Initialize(camera);

            Debug.Log(
                "[BFC PhysicsLab] Ready. Left-click a resting piece, drag opposite the shot direction, " +
                "release to launch. Press R to reset.");
        }

        public void ResetScenario()
        {
            ResetBody(_pieceA, _pieceAStart);
            ResetBody(_pieceB, _pieceBStart);
            ResetBody(_ball, _ballStart);
            UnityEngine.Physics.SyncTransforms();
            Debug.Log("[BFC PhysicsLab] Scenario reset.");
        }

        private static void ResetBody(PlanarKineticBody body, Vector3 position)
        {
            if (body == null)
            {
                return;
            }

            body.StopMotion();
            body.transform.position = position;
            body.transform.rotation = Quaternion.identity;
        }

        private static void CreateField(
            Transform parent,
            Material fieldMaterial,
            Material wallMaterial,
            PhysicMaterial collisionMaterial)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Surface Fixture";
            floor.transform.SetParent(parent, false);
            floor.transform.position = new Vector3(0f, -0.08f, 0f);
            floor.transform.localScale = new Vector3(
                PhysicsLabTuning.FieldWidth,
                0.16f,
                PhysicsLabTuning.FieldLength);
            floor.GetComponent<Renderer>().sharedMaterial = fieldMaterial;
            floor.GetComponent<Collider>().enabled = false;

            float halfWidth = PhysicsLabTuning.FieldWidth * 0.5f;
            float halfLength = PhysicsLabTuning.FieldLength * 0.5f;
            float thickness = PhysicsLabTuning.WallThickness;

            CreateWall(
                "Boundary Left",
                new Vector3(-halfWidth - (thickness * 0.5f), 0.5f, 0f),
                new Vector3(thickness, 1f, PhysicsLabTuning.FieldLength + (thickness * 2f)),
                parent,
                wallMaterial,
                collisionMaterial);
            CreateWall(
                "Boundary Right",
                new Vector3(halfWidth + (thickness * 0.5f), 0.5f, 0f),
                new Vector3(thickness, 1f, PhysicsLabTuning.FieldLength + (thickness * 2f)),
                parent,
                wallMaterial,
                collisionMaterial);
            CreateWall(
                "Boundary Near",
                new Vector3(0f, 0.5f, -halfLength - (thickness * 0.5f)),
                new Vector3(PhysicsLabTuning.FieldWidth, 1f, thickness),
                parent,
                wallMaterial,
                collisionMaterial);
            CreateWall(
                "Boundary Far",
                new Vector3(0f, 0.5f, halfLength + (thickness * 0.5f)),
                new Vector3(PhysicsLabTuning.FieldWidth, 1f, thickness),
                parent,
                wallMaterial,
                collisionMaterial);
        }

        private static void CreateWall(
            string name,
            Vector3 position,
            Vector3 scale,
            Transform parent,
            Material material,
            PhysicMaterial collisionMaterial)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent, false);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().sharedMaterial = material;
            wall.GetComponent<BoxCollider>().material = collisionMaterial;
        }

        private static PlanarKineticBody CreatePiece(
            string name,
            Vector3 position,
            Material material,
            PhysicMaterial collisionMaterial,
            Transform parent)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            piece.name = name;
            piece.transform.SetParent(parent, false);
            piece.transform.position = position;
            piece.transform.localScale = new Vector3(
                PhysicsLabTuning.PieceRadius,
                PhysicsLabTuning.PieceHeight * 0.5f,
                PhysicsLabTuning.PieceRadius);
            piece.GetComponent<Renderer>().sharedMaterial = material;

            Collider primitiveCollider = piece.GetComponent<Collider>();
            if (primitiveCollider != null)
            {
                primitiveCollider.enabled = false;
            }

            SphereCollider collider = piece.AddComponent<SphereCollider>();
            collider.radius = 1f;
            collider.material = collisionMaterial;

            PlanarKineticBody body = piece.AddComponent<PlanarKineticBody>();
            body.Configure(
                PhysicsBodyKind.Piece,
                PhysicsLabTuning.PieceMass,
                PhysicsLabTuning.PieceDeceleration,
                PhysicsLabTuning.PieceMaxSpeed,
                PhysicsLabTuning.RestSpeed);
            return body;
        }

        private static PlanarKineticBody CreateBall(
            string name,
            Vector3 position,
            Material material,
            PhysicMaterial collisionMaterial,
            Transform parent)
        {
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = name;
            ball.transform.SetParent(parent, false);
            ball.transform.position = position;
            ball.transform.localScale = Vector3.one * (PhysicsLabTuning.BallRadius * 2f);
            ball.GetComponent<Renderer>().sharedMaterial = material;
            ball.GetComponent<SphereCollider>().material = collisionMaterial;

            PlanarKineticBody body = ball.AddComponent<PlanarKineticBody>();
            body.Configure(
                PhysicsBodyKind.Ball,
                PhysicsLabTuning.BallMass,
                PhysicsLabTuning.BallDeceleration,
                PhysicsLabTuning.BallMaxSpeed,
                PhysicsLabTuning.RestSpeed);
            return body;
        }

        private static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                throw new InvalidOperationException("PhysicsLab could not resolve a runtime material shader.");
            }

            var material = new Material(shader)
            {
                color = color,
                hideFlags = HideFlags.DontSave
            };
            return material;
        }

        private static PhysicMaterial CreateCollisionMaterial()
        {
            var material = new PhysicMaterial("BFC PhysicsLab No Bounce")
            {
                dynamicFriction = 0f,
                staticFriction = 0f,
                bounciness = 0f,
                frictionCombine = PhysicMaterialCombine.Minimum,
                bounceCombine = PhysicMaterialCombine.Minimum,
                hideFlags = HideFlags.DontSave
            };
            return material;
        }

        private static void LogFixedStepBenchmark()
        {
            StopBenchmarkResult step30 = PhysicsBenchmark.SimulateStop(
                PhysicsLabTuning.MaxLaunchSpeed,
                PhysicsLabTuning.PieceDeceleration,
                1f / 30f,
                PhysicsLabTuning.RestSpeed);
            StopBenchmarkResult step60 = PhysicsBenchmark.SimulateStop(
                PhysicsLabTuning.MaxLaunchSpeed,
                PhysicsLabTuning.PieceDeceleration,
                1f / 60f,
                PhysicsLabTuning.RestSpeed);
            StopBenchmarkResult step120 = PhysicsBenchmark.SimulateStop(
                PhysicsLabTuning.MaxLaunchSpeed,
                PhysicsLabTuning.PieceDeceleration,
                1f / 120f,
                PhysicsLabTuning.RestSpeed);

            float spread = PhysicsBenchmark.RelativeSpread(
                step30.Distance,
                step60.Distance,
                step120.Distance);

            Debug.Log(
                $"[BFC PhysicsLab] fixed-step stop benchmark: " +
                $"30Hz={step30.Distance:F3}m, 60Hz={step60.Distance:F3}m, " +
                $"120Hz={step120.Distance:F3}m, spread={spread:P2}, " +
                $"tolerance={PhysicsLabTuning.FixedStepBenchmarkTolerance:P2}.");
        }
    }

    internal sealed class PhysicsLabDragLauncher : MonoBehaviour
    {
        private Camera _camera;
        private PlanarKineticBody _selected;
        private LineRenderer _aimLine;
        private Vector3 _currentPull;
        private float _currentPower;

        public void Initialize(Camera camera)
        {
            _camera = camera;
            _aimLine = gameObject.AddComponent<LineRenderer>();
            _aimLine.positionCount = 2;
            _aimLine.useWorldSpace = true;
            _aimLine.widthMultiplier = 0.055f;
            _aimLine.enabled = false;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null)
            {
                _aimLine.material = new Material(shader) { hideFlags = HideFlags.DontSave };
            }

            _aimLine.startColor = new Color(0.15f, 0.95f, 1f);
            _aimLine.endColor = new Color(1f, 0.2f, 0.75f);
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || _camera == null)
            {
                return;
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                TrySelect(mouse.position.ReadValue());
            }

            if (_selected != null && mouse.leftButton.isPressed)
            {
                UpdateAim(mouse.position.ReadValue());
            }

            if (_selected != null && mouse.leftButton.wasReleasedThisFrame)
            {
                Release();
            }
        }

        private void TrySelect(Vector2 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (!UnityEngine.Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                return;
            }

            PlanarKineticBody candidate = hit.collider.GetComponentInParent<PlanarKineticBody>();
            if (candidate == null || candidate.Kind != PhysicsBodyKind.Piece || !candidate.IsAtRest)
            {
                return;
            }

            _selected = candidate;
            _currentPull = Vector3.zero;
            _currentPower = 0f;
            _aimLine.enabled = true;
        }

        private void UpdateAim(Vector2 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            var dragPlane = new Plane(Vector3.up, _selected.transform.position);
            if (!dragPlane.Raycast(ray, out float enter))
            {
                return;
            }

            Vector3 groundPoint = ray.GetPoint(enter);
            Vector3 pull = _selected.transform.position - groundPoint;
            pull.y = 0f;

            float distance = Mathf.Min(pull.magnitude, PhysicsLabTuning.MaxDragDistance);
            _currentPower = Mathf.Clamp01(distance / PhysicsLabTuning.MaxDragDistance);
            _currentPull = pull.sqrMagnitude > 0.000001f ? pull.normalized : Vector3.zero;

            Vector3 start = _selected.transform.position + (Vector3.up * 0.65f);
            float visualLength = 0.8f + (_currentPower * 3.2f);
            _aimLine.SetPosition(0, start);
            _aimLine.SetPosition(1, start + (_currentPull * visualLength));
        }

        private void Release()
        {
            PlanarKineticBody selected = _selected;
            _selected = null;
            _aimLine.enabled = false;

            if (_currentPower < 0.02f || _currentPull.sqrMagnitude <= 0.000001f)
            {
                return;
            }

            selected.Launch(_currentPull, _currentPower, PhysicsLabTuning.MaxLaunchSpeed);
            Debug.Log(
                $"[BFC PhysicsLab] {selected.name} launch power={_currentPower:P0}, " +
                $"targetSpeed={_currentPower * PhysicsLabTuning.MaxLaunchSpeed:F2} m/s.");
        }
    }
}
