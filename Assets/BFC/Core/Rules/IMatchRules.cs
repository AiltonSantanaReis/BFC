namespace BFC.Core.Rules
{
    /// <summary>
    /// Read-only contract for rules that affect the logical match flow.
    /// Rule implementations must not depend on presentation or scene state.
    /// </summary>
    public interface IMatchRules
    {
        int MaxActionsPerPossession { get; }

        bool AutomaticGoalkeeper { get; }

        bool NormalizeCompetitiveEquipment { get; }
    }
}
