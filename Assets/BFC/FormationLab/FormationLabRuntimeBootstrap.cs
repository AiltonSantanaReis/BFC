using UnityEngine;
using UnityEngine.SceneManagement;

namespace BFC.FormationLab
{
    [DefaultExecutionOrder(-100)]
    public sealed class FormationLabRuntimeBootstrap : MonoBehaviour
    {
        public const string SceneName = "FormationLab";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AttachWhenFormationLabLoads()
        {
            if (SceneManager.GetActiveScene().name != SceneName)
            {
                return;
            }

            if (Object.FindFirstObjectByType<FormationLabRuntimeBootstrap>() != null)
            {
                return;
            }

            new GameObject("BFC FormationLab Runtime")
                .AddComponent<FormationLabRuntimeBootstrap>();
        }

        private void Awake()
        {
            FormationLabRuntimeBuilder.Build(transform);
        }
    }
}
