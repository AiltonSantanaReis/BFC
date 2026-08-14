using System;
using System.Collections.Generic;

namespace BFC.Core.Formations
{
    /// <summary>
    /// Immutable formation layout validated against a team-composition definition.
    /// </summary>
    public sealed class FormationDefinition
    {
        private readonly FormationSlot[] _slots;

        public FormationDefinition(
            string id,
            TeamCompositionDefinition composition,
            IReadOnlyList<FormationSlot> slots)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Formation id is required.", nameof(id));
            }

            Composition = composition ?? throw new ArgumentNullException(nameof(composition));
            if (slots == null)
            {
                throw new ArgumentNullException(nameof(slots));
            }

            if (slots.Count != composition.TotalPieces)
            {
                throw new ArgumentException(
                    "Formation slot count must match the team composition.",
                    nameof(slots));
            }

            _slots = new FormationSlot[slots.Count];
            var ids = new HashSet<string>(StringComparer.Ordinal);
            int goalkeeperCount = 0;

            for (int index = 0; index < slots.Count; index++)
            {
                FormationSlot slot = slots[index];
                if (!ids.Add(slot.Id))
                {
                    throw new ArgumentException(
                        $"Duplicate formation slot id: {slot.Id}",
                        nameof(slots));
                }

                if (slot.Role == PieceRole.Goalkeeper)
                {
                    goalkeeperCount++;
                }

                _slots[index] = slot;
            }

            if (goalkeeperCount != composition.GoalkeeperCount)
            {
                throw new ArgumentException(
                    "Formation goalkeeper count must match the team composition.",
                    nameof(slots));
            }

            Id = id;
        }

        public string Id { get; }

        public TeamCompositionDefinition Composition { get; }

        public IReadOnlyList<FormationSlot> Slots => _slots;
    }
}
