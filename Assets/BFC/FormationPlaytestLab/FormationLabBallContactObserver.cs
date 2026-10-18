using BFC.FormationLab;
using UnityEngine;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Observes physical ball-to-piece OnCollisionEnter events only.
    /// Does not assign possession, determine passes, or change physics.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FormationLabBallContactObserver : MonoBehaviour
    {
        private TacticalActionTelemetry _telemetry;
        private LabBallSpinController _spin;
        private LabBallChipFlight _chip;

        public void Initialize(TacticalActionTelemetry telemetry, LabBallSpinController spin,
            LabBallChipFlight chip)
        {
            _telemetry = telemetry;
            _spin = spin;
            _chip = chip;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_telemetry == null || !_telemetry.IsRecording || collision == null)
                return;

            FormationPieceRuntime piece =
                collision.collider.GetComponentInParent<FormationPieceRuntime>();
            if (piece != null)
            {
                _telemetry.ObserveBallContact(piece.Team, piece.SlotId);
                _spin?.OnPieceContact(piece);
                _chip?.OnPieceContact(piece);
            }
        }
    }
}
