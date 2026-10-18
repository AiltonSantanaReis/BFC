using System.Collections.Generic;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.FormationLab;
using BFC.Physics;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Optional visual scouting of straight ball-to-teammate corridors.
    /// This is NOT a trajectory predictor or auto-targeting assistance.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LabTacticalLaneOverlay : MonoBehaviour
    {
        private const int MaxLanes = 4;
        private const float ClearancePadding = 0.15f;
        private readonly List<LineRenderer> _lines = new List<LineRenderer>();
        private Material _material;
        private FormationPieceRuntime[] _pieces;
        private Transform _ball;
        private bool _visible;
        private int _clearCount;
        private int _shownCount;

        public bool Visible => _visible;
        public int VisibleLaneCount => _shownCount;
        public int GeometricallyClearLaneCount => _clearCount;

        public void Initialize(Transform ball, FormationPieceRuntime[] pieces)
        {
            _ball = ball;
            _pieces = pieces;
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader != null)
                _material = new Material(shader) { hideFlags = HideFlags.DontSave };

            for (int i = 0; i < MaxLanes; i++)
            {
                GameObject visual = new GameObject("Experimental Tactical Lane " + (i + 1));
                visual.transform.SetParent(transform, false);
                var line = visual.AddComponent<LineRenderer>();
                line.useWorldSpace = true;
                line.positionCount = 2;
                line.widthMultiplier = 0.045f;
                line.numCapVertices = 2;
                if (_material != null) line.sharedMaterial = _material;
                line.enabled = false;
                _lines.Add(line);
            }
        }

        public void Toggle() => SetVisible(!_visible);

        public void SetVisible(bool visible)
        {
            _visible = visible;
            if (!visible) Clear();
        }

        public void Refresh(TeamId team)
        {
            if (!_visible || _pieces == null || _ball == null)
            {
                Clear();
                return;
            }

            var candidates = new List<FormationPieceRuntime>();
            foreach (FormationPieceRuntime piece in _pieces)
            {
                if (piece != null && piece.Team == team && piece.Role == PieceRole.Outfield)
                    candidates.Add(piece);
            }

            Vector3 origin = _ball.position;
            candidates.Sort((a, b) =>
            {
                float da = (a.transform.position - origin).sqrMagnitude;
                float db = (b.transform.position - origin).sqrMagnitude;
                int compare = da.CompareTo(db);
                return compare != 0 ? compare :
                    string.CompareOrdinal(a.SlotId, b.SlotId);
            });

            _shownCount = Mathf.Min(MaxLanes, candidates.Count);
            _clearCount = 0;

            for (int i = 0; i < _shownCount; i++)
            {
                FormationPieceRuntime destination = candidates[i];
                var obstacles = new List<LabLaneObstacle>(_pieces.Length);
                foreach (FormationPieceRuntime piece in _pieces)
                {
                    if (piece == null || piece == destination) continue;
                    obstacles.Add(new LabLaneObstacle(
                        new Vector2(piece.transform.position.x, piece.transform.position.z),
                        PhysicsLabTuning.PieceRadius));
                }

                LabLaneAssessment assessment = LabTacticalLaneAnalyzer.Assess(
                    new Vector2(origin.x, origin.z),
                    new Vector2(destination.transform.position.x, destination.transform.position.z),
                    obstacles, PhysicsLabTuning.BallRadius, ClearancePadding);

                if (assessment.IsGeometricallyClear) _clearCount++;
                LineRenderer line = _lines[i];
                Color color = assessment.IsGeometricallyClear
                    ? new Color(0.15f, 1f, 0.75f, 0.65f)
                    : new Color(1f, 0.62f, 0.19f, 0.6f);
                line.startColor = color;
                line.endColor = color;
                line.SetPosition(0, origin + Vector3.up * 0.15f);
                line.SetPosition(1, destination.transform.position + Vector3.up * 0.15f);
                line.enabled = true;
            }

            for (int i = _shownCount; i < _lines.Count; i++) _lines[i].enabled = false;
        }

        private void Clear()
        {
            _shownCount = 0;
            _clearCount = 0;
            foreach (LineRenderer line in _lines)
                if (line != null) line.enabled = false;
        }

        private void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }
    }
}
