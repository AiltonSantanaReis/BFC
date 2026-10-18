using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BFC.FormationLab
{
    [DefaultExecutionOrder(-100)]
    public sealed class FormationLabRuntimeBootstrap : MonoBehaviour
    {
        public const string SceneName = "FormationLab";
        private const string FieldProfileArgumentPrefix = "-bfcFieldProfile=";

        [SerializeField] private FormationLabFieldProfile defaultFieldProfile = FormationLabFieldProfile.Original;
        [SerializeField] private bool enablePlayablePrototype;

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
            FormationLabFieldProfile profile = ResolveFieldProfile();
            Transform fixtures = FormationLabRuntimeBuilder.Build(transform, profile);

            if (enablePlayablePrototype || HasArgument("-bfcPlayableLab"))
            {
                var playtest = gameObject.AddComponent<FormationLabPlaytestController>();
                playtest.Initialize(
                    fixtures,
                    FormationLabPreviewProfiles.CreateFieldPreview(profile),
                    Camera.main);
            }

            if (FormationLabPerformanceProbe.IsRequested() &&
                GetComponent<FormationLabPerformanceProbe>() == null)
            {
                gameObject.AddComponent<FormationLabPerformanceProbe>();
            }
        }

        private static bool HasArgument(string argument)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], argument, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private FormationLabFieldProfile ResolveFieldProfile()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith(FieldProfileArgumentPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string requested = args[i].Substring(FieldProfileArgumentPrefix.Length);
                if (Enum.TryParse(requested, true, out FormationLabFieldProfile profile) &&
                    Enum.IsDefined(typeof(FormationLabFieldProfile), profile))
                {
                    return profile;
                }

                Debug.LogWarning(
                    $"[BFC FormationLab] Unknown field profile '{requested}'; " +
                    $"using default {defaultFieldProfile}.");
                break;
            }

            return defaultFieldProfile;
        }
    }
}
