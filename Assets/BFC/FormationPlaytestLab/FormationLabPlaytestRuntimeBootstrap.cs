using System;
using BFC.FormationLab;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BFC.FormationPlaytestLab
{
    /// <summary>
    /// Separate, opt-in input bridge so FormationLab stays independent from input.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public sealed class FormationLabPlaytestRuntimeBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AttachWhenRequested()
        {
            if (SceneManager.GetActiveScene().name != FormationLabRuntimeBootstrap.SceneName ||
                !WasRequested() ||
                UnityEngine.Object.FindFirstObjectByType<FormationLabPlaytestRuntimeBootstrap>() != null)
            {
                return;
            }

            new GameObject("BFC Playtest Lab (PENDENTE DE VALIDACAO)")
                .AddComponent<FormationLabPlaytestRuntimeBootstrap>();
        }

        private static bool WasRequested()
        {
            foreach (string argument in Environment.GetCommandLineArgs())
            {
                if (string.Equals(argument, "-bfcPlayableLab", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private void Start()
        {
            FormationLabRuntimeBootstrap formation =
                UnityEngine.Object.FindFirstObjectByType<FormationLabRuntimeBootstrap>();
            if (formation == null || formation.GeneratedFixtures == null)
            {
                Debug.LogError("[BFC Playtest] FormationLab fixtures unavailable.");
                return;
            }

            var playtest = gameObject.AddComponent<FormationLabPlaytestController>();
            playtest.Initialize(
                formation.GeneratedFixtures,
                FormationLabPreviewProfiles.CreateFieldPreview(formation.ActiveProfile),
                Camera.main);
        }
    }
}
