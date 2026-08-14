using UnityEngine;

namespace BFC.Bootstrap
{
    /// <summary>
    /// Composition root for runtime dependencies.
    /// Service construction belongs here; gameplay systems must not become global singletons.
    /// </summary>
    public sealed class BfcBootstrap : MonoBehaviour
    {
        private static BfcBootstrap instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[BFC] Bootstrap initialized.");
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
