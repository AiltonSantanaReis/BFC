using System;
using System.Collections.Generic;
using BFC.Core.Fields;
using BFC.Core.Matches;

namespace BFC.Core.Formations
{
    /// <summary>
    /// Converts normalized formation slots into safe XZ field positions without UnityEngine.
    /// Team B mirrors Team A longitudinally so both sides use the same formation data.
    /// </summary>
    public static class FormationSpawnPlanner
    {
        public static IReadOnlyList<FormationSpawn> CreateSpawns(
            FormationDefinition formation,
            FieldDefinition field,
            TeamId team,
            float safetyMargin)
        {
            if (formation == null)
            {
                throw new ArgumentNullException(nameof(formation));
            }

            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (team != TeamId.TeamA && team != TeamId.TeamB)
            {
                throw new ArgumentOutOfRangeException(nameof(team));
            }

            if (float.IsNaN(safetyMargin) || float.IsInfinity(safetyMargin) || safetyMargin < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(safetyMargin));
            }

            float usableHalfLength = field.HalfLength - safetyMargin;
            float usableHalfWidth = field.HalfWidth - safetyMargin;
            if (usableHalfLength <= 0f || usableHalfWidth <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(safetyMargin),
                    "Safety margin leaves no playable spawn area.");
            }

            float teamLongitudinalSign = team == TeamId.TeamA ? 1f : -1f;
            var spawns = new FormationSpawn[formation.Slots.Count];

            for (int index = 0; index < formation.Slots.Count; index++)
            {
                FormationSlot slot = formation.Slots[index];
                float x = slot.Longitudinal * usableHalfLength * teamLongitudinalSign;
                float z = slot.Lateral * usableHalfWidth;
                spawns[index] = new FormationSpawn(slot.Id, slot.Role, x, z);
            }

            return Array.AsReadOnly(spawns);
        }
    }
}
