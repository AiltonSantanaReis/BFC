using System;
using BFC.Core.Matches;

namespace BFC.Gameplay.Matches
{
    /// <summary>
    /// Logical player command accepted by the match state machine before physics execution.
    /// Direction/force payload belongs to the later gameplay-to-physics bridge.
    /// </summary>
    public readonly struct PlayerActionCommand
    {
        public PlayerActionCommand(TeamId team, string pieceId)
        {
            if (team == TeamId.None)
            {
                throw new ArgumentOutOfRangeException(nameof(team));
            }

            if (string.IsNullOrWhiteSpace(pieceId))
            {
                throw new ArgumentException("Piece identifier is required.", nameof(pieceId));
            }

            Team = team;
            PieceId = pieceId;
        }

        public TeamId Team { get; }

        public string PieceId { get; }
    }
}
