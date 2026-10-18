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

        public void Initialize(TacticalActionTelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_telemetry == null || !_telemetry.IsRecording || collision == null)
                return;

            FormationPieceRuntime piece =
                collision.collider.GetComponentInParent<FormationPieceRuntime>();
            if (piece != null)
                _telemetry.ObserveBallContact(piece.Team, piece.SlotId);
        }
    }
}
