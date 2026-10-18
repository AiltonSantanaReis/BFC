using System;
using System.Globalization;
using System.IO;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.Core.Rules;
using BFC.Gameplay.Matches;
using BFC.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Opt-in, two-player hot-seat integration experiment: input -> match authorization ->
    /// physical launch -> settle/goal -> match state. NOT an approved gameplay mode.
    /// No goalkeeper movement, ball-out restart, or scoring rule is being standardized.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FormationLabPlaytestController : MonoBehaviour
    {
        private const float MinPower = 0.02f;
        private const float RestConfirmationSeconds = 0.25f;
        private const int ExperimentalActionsPerPossession = 3;

        private Transform _fixtures;
        private FieldDefinition _field;
        private Camera _camera;
        private PlanarKineticBody _ball;
        private PlanarKineticBody[] _bodies;
        private Vector3[] _startPositions;
        private Quaternion[] _startRotations;
        private LineRenderer _aimLine;
        private FormationPieceRuntime _selectedPiece;
        private Vector3 _aimDirection;
        private float _aimPower;
        private bool _shotInProgress;
        private bool _ballOutOfBounds;
        private float _restDuration;
        private Vector3 _previousBallPosition;
        private TeamId _lastScoringTeam;
        private MatchController _match;
        private TacticalActionTelemetry _telemetry;
        private LabTacticalLaneOverlay _laneOverlay;
        private LabKickKind _kickKind = LabKickKind.Shot;

        public TacticalActionTelemetry Telemetry => _telemetry;

        public MatchState State => _match != null ? _match.State : null;
        public bool BallOutOfBounds => _ballOutOfBounds;

        public void Initialize(Transform fixtures, FieldDefinition field, Camera camera)
        {
            if (fixtures == null) throw new ArgumentNullException(nameof(fixtures));
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            if (_match != null) throw new InvalidOperationException("Already initialized.");

            _fixtures = fixtures;
            _field = field;
            _camera = camera;

            Transform ballTransform = fixtures.Find(FormationLabRuntimeBuilder.BallName);
            if (ballTransform == null ||
                !ballTransform.TryGetComponent(out _ball))
            {
                throw new InvalidOperationException("FormationLab ball is missing.");
            }

            _telemetry = new TacticalActionTelemetry(field);
            _laneOverlay = gameObject.AddComponent<LabTacticalLaneOverlay>();
            _laneOverlay.Initialize(_ball.transform,
                fixtures.GetComponentsInChildren<FormationPieceRuntime>());
            FormationLabBallContactObserver observer =
                _ball.GetComponent<FormationLabBallContactObserver>();
            if (observer == null)
                observer = _ball.gameObject.AddComponent<FormationLabBallContactObserver>();
            observer.Initialize(_telemetry);

            _bodies = fixtures.GetComponentsInChildren<PlanarKineticBody>();
            _startPositions = new Vector3[_bodies.Length];
            _startRotations = new Quaternion[_bodies.Length];
            for (int i = 0; i < _bodies.Length; i++)
            {
                _startPositions[i] = _bodies[i].transform.position;
                _startRotations[i] = _bodies[i].transform.rotation;
            }

            _aimLine = gameObject.AddComponent<LineRenderer>();
            _aimLine.positionCount = 2;
            _aimLine.useWorldSpace = true;
            _aimLine.widthMultiplier = 0.065f;
            _aimLine.startColor = Color.cyan;
            _aimLine.endColor = Color.magenta;
            _aimLine.enabled = false;
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                _aimLine.material = new Material(shader) { hideFlags = HideFlags.DontSave };
            }

            ResetMatch();
            Debug.Log("[BFC Playtest] PENDENTE DE VALIDACAO: field match loop enabled. " +
                      "Left-drag a piece of the active team to aim; release to launch. " +
                      "R resets the experiment; Space restarts after a goal.");
        }

        private void Update()
        {
            if (_match == null) return;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            {
                ResetMatch();
                return;
            }

            if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
                ExportTelemetryCsv();

            if (keyboard != null && keyboard.pKey.wasPressedThisFrame &&
                !_shotInProgress && _match.State.Phase == MatchPhase.AwaitingAction)
            {
                _kickKind = _kickKind == LabKickKind.Shot
                    ? LabKickKind.ControlledPass : LabKickKind.Shot;
            }

            if (keyboard != null && keyboard.tKey.wasPressedThisFrame)
                _laneOverlay.Toggle();

            _laneOverlay.Refresh(
                _match.State.Phase == MatchPhase.AwaitingAction && !_ballOutOfBounds
                    ? _match.State.Possession
                    : TeamId.None);

            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame &&
                _match.State.Phase == MatchPhase.AwaitingRestart)
            {
                RestartAfterGoal();
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null || _camera == null || _ballOutOfBounds)
            {
                return;
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                TrySelect(mouse.position.ReadValue());
            }

            if (_selectedPiece != null && mouse.leftButton.isPressed)
            {
                UpdateAim(mouse.position.ReadValue());
            }

            if (_selectedPiece != null && mouse.leftButton.wasReleasedThisFrame)
            {
                FormationPieceRuntime selection = _selectedPiece;
                Vector3 direction = _aimDirection;
                float power = _aimPower;
                ClearSelection();
                TryLaunch(selection, direction, power);
            }
        }

        /// <summary>Public for PlayMode tests; all competitive eligibility is checked here.</summary>
        public bool TryLaunch(FormationPieceRuntime piece, Vector3 direction, float power)
        {
            if (_match == null || _shotInProgress || _ballOutOfBounds ||
                _match.State.Phase != MatchPhase.AwaitingAction ||
                piece == null || !piece.transform.IsChildOf(_fixtures) ||
                piece.Role != PieceRole.Outfield ||
                piece.Team != _match.State.Possession ||
                float.IsNaN(power) || float.IsInfinity(power) ||
                power < MinPower ||
                float.IsNaN(direction.x) || float.IsNaN(direction.z) ||
                float.IsInfinity(direction.x) || float.IsInfinity(direction.z) ||
                (direction.x * direction.x + direction.z * direction.z) < 0.000001f ||
                !AllBodiesAtRest())
            {
                return false;
            }

            PlanarKineticBody body = piece.GetComponent<PlanarKineticBody>();
            if (body == null || body.Kind != PhysicsBodyKind.Piece)
            {
                return false;
            }

            PlayerActionSubmissionResult submission = _match.TrySubmitAction(
                new PlayerActionCommand(piece.Team, piece.SlotId));
            if (!submission.Accepted)
            {
                return false;
            }

            _previousBallPosition = _ball.transform.position;
            _shotInProgress = true;
            _restDuration = 0f;
            _telemetry.Begin(piece.Team, piece.SlotId, Mathf.Clamp01(power),
                _previousBallPosition, _kickKind);
            float launchLimit = LabKickProfile.ResolveMaximumSpeed(
                _kickKind, PhysicsLabTuning.MaxLaunchSpeed);
            body.Launch(direction, Mathf.Clamp01(power), launchLimit);
            return true;
        }

        private void FixedUpdate()
        {
            if (!_shotInProgress || _ball == null)
            {
                return;
            }

            Vector3 currentPosition = _ball.transform.position;
            _telemetry.ObserveBallPosition(currentPosition);
            if (!_ballOutOfBounds && FormationLabGoalLine.TryDetect(
                    _field, _previousBallPosition, currentPosition,
                    PhysicsLabTuning.BallRadius, out TeamId scorer))
            {
                _lastScoringTeam = scorer;
                _telemetry.Complete(TacticalActionEnd.Goal, currentPosition);
                StopAllBodies();
                _shotInProgress = false;
                _match.ResolvePhysicalAction(PhysicalActionResolution.Goal(scorer));
                Debug.Log($"[BFC Playtest] Goal: {scorer}. Space to restart (experimental).");
                return;
            }

            _previousBallPosition = currentPosition;
            if (Mathf.Abs(currentPosition.x) > _field.HalfLength + PhysicsLabTuning.BallRadius ||
                Mathf.Abs(currentPosition.z) > _field.HalfWidth + PhysicsLabTuning.BallRadius)
            {
                // OPEN-002: do not invent a throw-in, corner or goal-kick resolution.
                _ballOutOfBounds = true;
            }

            _restDuration = AllBodiesAtRest()
                ? _restDuration + Time.fixedDeltaTime
                : 0f;
            if (_restDuration < RestConfirmationSeconds)
            {
                return;
            }

            _shotInProgress = false;
            TacticalActionRecord record = _telemetry.Complete(
                _ballOutOfBounds ? TacticalActionEnd.OutOfBounds : TacticalActionEnd.Settled,
                currentPosition);
            Debug.Log(
                $"[BFC Telemetry] #{record.Sequence} ballPath={record.BallPathLength:F2} " +
                $"forward={record.NetForwardProgress:F2} contacts={record.ContactEvents} " +
                $"outcome={record.Outcome} (laboratory units, not a rules verdict).");
            // The temporary lab only transfers turns on the configured action limit.
            // It does not yet infer possession from contacts, fouls or out-of-bounds.
            _match.ResolvePhysicalAction(PhysicalActionResolution.ContinuePossession());
            if (_ballOutOfBounds)
            {
                Debug.LogWarning("[BFC Playtest] Ball out; OPEN-002 unresolved. " +
                                 "Prototype stops accepting actions. Press R to reset.");
            }
        }

        private void TrySelect(Vector2 screenPosition)
        {
            ClearSelection();
            if (_match.State.Phase != MatchPhase.AwaitingAction ||
                _shotInProgress || !AllBodiesAtRest())
            {
                return;
            }

            Ray ray = _camera.ScreenPointToRay(screenPosition);
            if (!UnityEngine.Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                return;
            }

            FormationPieceRuntime piece = hit.collider.GetComponentInParent<FormationPieceRuntime>();
            if (piece == null || piece.Team != _match.State.Possession ||
                piece.Role != PieceRole.Outfield ||
                !piece.transform.IsChildOf(_fixtures))
            {
                return;
            }

            _selectedPiece = piece;
            _aimLine.enabled = true;
        }

        private void UpdateAim(Vector2 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            Plane plane = new Plane(Vector3.up, _selectedPiece.transform.position);
            if (!plane.Raycast(ray, out float enter))
            {
                return;
            }

            Vector3 pull = _selectedPiece.transform.position - ray.GetPoint(enter);
            pull.y = 0f;
            float distance = Mathf.Min(pull.magnitude, PhysicsLabTuning.MaxDragDistance);
            _aimPower = Mathf.Clamp01(distance / PhysicsLabTuning.MaxDragDistance);
            _aimDirection = pull.sqrMagnitude > 0.000001f ? pull.normalized : Vector3.zero;

            Vector3 start = _selectedPiece.transform.position + Vector3.up * 0.65f;
            _aimLine.SetPosition(0, start);
            _aimLine.SetPosition(1, start + _aimDirection * (0.8f + _aimPower * 3.2f));
        }

        private bool AllBodiesAtRest()
        {
            if (_bodies == null) return false;
            foreach (PlanarKineticBody body in _bodies)
            {
                if (body == null || !body.IsAtRest) return false;
            }
            return true;
        }

        private void StopAllBodies()
        {
            foreach (PlanarKineticBody body in _bodies)
            {
                if (body != null) body.StopMotion();
            }
        }

        private void RestorePositions()
        {
            _telemetry?.CancelActive();
            StopAllBodies();
            for (int i = 0; i < _bodies.Length; i++)
            {
                _bodies[i].transform.SetPositionAndRotation(_startPositions[i], _startRotations[i]);
            }
            UnityEngine.Physics.SyncTransforms();
            _previousBallPosition = _ball.transform.position;
            _ballOutOfBounds = false;
            _shotInProgress = false;
            _restDuration = 0f;
            ClearSelection();
        }

        private void ResetMatch()
        {
            RestorePositions();
            // Parameters are an isolated lab experiment, not a Classic/Casual/Liga Justa ruleset.
            var labRules = new MatchRules(
                ExperimentalActionsPerPossession,
                automaticGoalkeeper: false,
                normalizeCompetitiveEquipment: true);
            _match = new MatchController(labRules, TimeSpan.FromMinutes(15));
            _match.StartMatch(TeamId.TeamA);
            _lastScoringTeam = TeamId.None;
            _kickKind = LabKickKind.Shot;
        }

        private void RestartAfterGoal()
        {
            if (_match.State.Phase != MatchPhase.AwaitingRestart) return;
            TeamId next = _lastScoringTeam == TeamId.TeamA ? TeamId.TeamB : TeamId.TeamA;
            RestorePositions();
            // Temporary manual laboratory restart; OPEN-002 still requires ruleset decisions.
            _match.ResumeAfterRestart(next);
        }

        private void ClearSelection()
        {
            _selectedPiece = null;
            _aimDirection = Vector3.zero;
            _aimPower = 0f;
            if (_aimLine != null) _aimLine.enabled = false;
        }

        private void ExportTelemetryCsv()
        {
            if (_telemetry == null || _telemetry.Records.Count == 0)
            {
                Debug.LogWarning("[BFC Telemetry] No completed actions to export.");
                return;
            }

            try
            {
                string directory = Path.Combine(Application.persistentDataPath, "BFC-Playtests");
                Directory.CreateDirectory(directory);
                string name = "field-" + _field.Id + "-" +
                    DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture) +
                    ".csv";
                string path = Path.Combine(directory, name);
                File.WriteAllText(path, _telemetry.ToCsv());
                Debug.Log($"[BFC Telemetry] CSV exported to: {path}");
            }
            catch (Exception error)
            {
                Debug.LogError("[BFC Telemetry] Could not write CSV: " + error.Message);
            }
        }

        private void OnGUI()
        {
            if (_match == null) return;
            MatchState state = _match.State;
            string status = state.Phase == MatchPhase.AwaitingRestart
                ? "GOL! Espaco: reiniciar demonstracao"
                : _ballOutOfBounds
                    ? "Bola fora: OPEN-002 pendente. R reinicia."
                    : state.Phase == MatchPhase.ResolvingAction
                        ? "Aguardando a bola e as pecas pararem..."
                        : $"Vez: {state.Possession} | Acoes: {state.ActionsUsedInPossession}/{ExperimentalActionsPerPossession}";
            GUI.Box(new Rect(12f, 12f, Mathf.Max(240f, Mathf.Min(515f, Screen.width - 24f)), 114f),
                "BFC | PENDENTE DE VALIDACAO (Unity / Codex)\n" +
                $"Equipe A {state.Score.TeamA} x {state.Score.TeamB} Equipe B\n" +
                status + "\n" +
                "Arrastar/soltar: lancar | R: reiniciar | Espaco: apos gol");
            if (_telemetry == null) return;

            TacticalActionRecord last = _telemetry.LastRecord;
            string summary = last == null
                ? "Sem jogadas concluidas para analisar."
                : $"Ultima #{last.Sequence}: trajeto={last.BallPathLength:F1}u " +
                  $"avanco={last.NetForwardProgress:+0.0;-0.0;0.0}u " +
                  $"contatos={last.ContactEvents} (A:{last.FriendlyContactEvents}/" +
                  $"O:{last.OpponentContactEvents}) - {last.Outcome}";
            GUI.Box(new Rect(12f, 132f, Mathf.Max(240f, Mathf.Min(515f, Screen.width - 24f)), 68f),
                "TELEMETRIA TATICA - APENAS OBSERVACAO\n" + summary + "\n" +
                $"Amostras: {_telemetry.Records.Count} | C: exportar CSV | " +
                $"P: {_kickKind} | T: corredores ({_laneOverlay.VisibleLaneCount}/" +
                $"{_laneOverlay.GeometricallyClearLaneCount} livres)");
        }

        private void OnDestroy()
        {
            if (_aimLine != null && _aimLine.material != null &&
                (_aimLine.material.hideFlags & HideFlags.DontSave) != 0)
            {
                Destroy(_aimLine.material);
            }
        }
    }
}
