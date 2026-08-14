using BFC.Core.Formations;
using BFC.Core.Matches;
using UnityEngine;

namespace BFC.FormationLab
{
    [DisallowMultipleComponent]
    public sealed class FormationPieceRuntime : MonoBehaviour
    {
        public TeamId Team { get; private set; }
        public PieceRole Role { get; private set; }
        public string SlotId { get; private set; }

        public void Initialize(TeamId team, string slotId, PieceRole role)
        {
            Team = team;
            SlotId = slotId;
            Role = role;
        }
    }
}
