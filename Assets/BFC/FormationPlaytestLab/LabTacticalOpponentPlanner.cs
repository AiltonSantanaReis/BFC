using System;
using System.Collections.Generic;
using BFC.Core.Fields;
using BFC.Core.Formations;
using BFC.Core.Matches;
using BFC.Physics;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    public readonly struct LabAIPlayer
    {
        public LabAIPlayer(TeamId team, string id, PieceRole role, Vector2 position)
        {
            if (team != TeamId.TeamA && team != TeamId.TeamB)
                throw new ArgumentOutOfRangeException(nameof(team));
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Piece ID required", nameof(id));
            if (!LabLaneObstacle.IsFinite(position))
                throw new ArgumentOutOfRangeException(nameof(position));
            Team = team;
            Id = id;
            Role = role;
            Position = position;
        }

        public TeamId Team { get; }
        public string Id { get; }
        public PieceRole Role { get; }
        public Vector2 Position { get; }
    }

    public readonly struct LabAIPlan
    {
        internal LabAIPlan(string pieceId, Vector2 direction, float power,
            LabKickKind kind, int blockers)
        {
            PieceId = pieceId;
            Direction = direction;
            Power = power;
            Kind = kind;
            PotentialBlockers = blockers;
        }

        public string PieceId { get; }
        public Vector2 Direction { get; }
        public float Power { get; }
        public LabKickKind Kind { get; }
        public int PotentialBlockers { get; }
    }

    /// <summary>
    /// Very basic, deterministic laboratory opponent: chooses a legally selectable
    /// outfield piece and aims towards the ball. This is NOT predictive full AI.
    /// Final shot still uses exactly the player-facing match command / physics path.
    /// </summary>
    public static class LabTacticalOpponentPlanner
    {
        public static bool TryChoose(
            TeamId team, Vector2 ball, FieldDefinition field,
            IReadOnlyList<LabAIPlayer> allPieces, out LabAIPlan plan)
        {
            if (team != TeamId.TeamA && team != TeamId.TeamB)
                throw new ArgumentOutOfRangeException(nameof(team));
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (allPieces == null) throw new ArgumentNullException(nameof(allPieces));
            if (!LabLaneObstacle.IsFinite(ball))
                throw new ArgumentOutOfRangeException(nameof(ball));

            plan = default;
            float bestScore = float.PositiveInfinity;
            float forwardSign = team == TeamId.TeamA ? 1f : -1f;

            foreach (LabAIPlayer candidate in allPieces)
            {
                if (candidate.Team != team || candidate.Role != PieceRole.Outfield)
                    continue;
                Vector2 vector = ball - candidate.Position;
                float distance = vector.magnitude;
                if (distance <= 0.01f) continue;

                var otherPieces = new List<LabLaneObstacle>(allPieces.Count);
                foreach (LabAIPlayer other in allPieces)
                {
                    if (other.Team == candidate.Team && other.Id == candidate.Id) continue;
                    otherPieces.Add(new LabLaneObstacle(other.Position,
                        PhysicsLabTuning.PieceRadius));
                }

                LabLaneAssessment lane = LabTacticalLaneAnalyzer.Assess(
                    candidate.Position, ball, otherPieces,
                    PhysicsLabTuning.PieceRadius, 0.02f);

                // Prefer a piece behind the ball in the attack direction, where a
                // normal collision is more likely to advance it towards goal.
                float aheadPenalty = (candidate.Position.x - ball.x) * forwardSign > 0f
                    ? 2.2f : 0f;
                float score = distance + 3.5f * lane.PotentialBlockers + aheadPenalty;

                // Deterministic tie-break independent of enumeration order.
                if (score > bestScore ||
                    (Mathf.Abs(score - bestScore) < 0.0001f &&
                     plan.PieceId != null && string.CompareOrdinal(candidate.Id, plan.PieceId) >= 0))
                {
                    continue;
                }

                float goalDistance = field.HalfLength - ball.x * forwardSign;
                LabKickKind kind = goalDistance > field.Length * 0.25f && distance < 4f
                    ? LabKickKind.ControlledPass : LabKickKind.Shot;
                float powerScale = kind == LabKickKind.ControlledPass ? 3.8f : 6f;
                float power = Mathf.Clamp(distance / powerScale + 0.19f, 0.22f, 1f);
                plan = new LabAIPlan(candidate.Id, vector / distance, power,
                    kind, lane.PotentialBlockers);
                bestScore = score;
            }

            return plan.PieceId != null;
        }
    }
}
