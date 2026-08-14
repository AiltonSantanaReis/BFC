using UnityEngine;

namespace BFC.Bootstrap
{
    /// <summary>
    /// Composition root for runtime dependencies.
    /// Keep service construction here rather than introducing global mutable singletons.
    /// </summary>
    public sealed class BfcBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // Foundation only. Runtime services will be composed here as milestones land.
            Debug.Log("[BFC] Bootstrap initialized.");
        }
    }
}
