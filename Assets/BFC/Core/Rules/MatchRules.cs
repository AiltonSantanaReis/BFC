using System;

namespace BFC.Core.Rules
{
    /// <summary>
    /// Immutable logical ruleset. Numeric tuning should be supplied by configuration,
    /// while rule semantics remain traceable to governance rule IDs.
    /// </summary>
    public sealed class MatchRules : IMatchRules
    {
        public MatchRules(
            int maxActionsPerPossession,
            bool automaticGoalkeeper,
            bool normalizeCompetitiveEquipment)
        {
            if (maxActionsPerPossession <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxActionsPerPossession),
                    "Actions per possession must be greater than zero.");
            }

            MaxActionsPerPossession = maxActionsPerPossession;
            AutomaticGoalkeeper = automaticGoalkeeper;
            NormalizeCompetitiveEquipment = normalizeCompetitiveEquipment;
        }

        public int MaxActionsPerPossession { get; }

        public bool AutomaticGoalkeeper { get; }

        public bool NormalizeCompetitiveEquipment { get; }
    }
}
