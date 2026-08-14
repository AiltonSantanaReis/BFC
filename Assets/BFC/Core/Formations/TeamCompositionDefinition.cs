using System;

namespace BFC.Core.Formations
{
    /// <summary>
    /// Immutable team-composition data. Counts belong to a mode/ruleset definition,
    /// not to global match logic.
    /// </summary>
    public sealed class TeamCompositionDefinition
    {
        private static readonly TeamCompositionDefinition LargeFieldElevenValue =
            new TeamCompositionDefinition(totalPieces: 11, goalkeeperCount: 1);

        public TeamCompositionDefinition(int totalPieces, int goalkeeperCount)
        {
            if (totalPieces <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalPieces));
            }

            if (goalkeeperCount < 0 || goalkeeperCount > totalPieces)
            {
                throw new ArgumentOutOfRangeException(nameof(goalkeeperCount));
            }

            TotalPieces = totalPieces;
            GoalkeeperCount = goalkeeperCount;
        }

        /// <summary>
        /// GAME-070: large-field profile only. This is not a universal BFC piece count.
        /// </summary>
        public static TeamCompositionDefinition LargeFieldEleven => LargeFieldElevenValue;

        public int TotalPieces { get; }

        public int GoalkeeperCount { get; }

        public int OutfieldCount => TotalPieces - GoalkeeperCount;
    }
}
