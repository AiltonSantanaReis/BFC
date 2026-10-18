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
        private const string ColliderArgumentPrefix = "-bfcPieceCollider=";

        [SerializeField] private FormationLabFieldProfile defaultFieldProfile = FormationLabFieldProfile.Original;
        [SerializeField] private FormationLabColliderProfile defaultColliderProfile =
            FormationLabColliderProfile.SphereBaseline;

        public FormationLabColliderProfile ActiveColliderProfile { get; private set; }

        public FormationLabFieldProfile ActiveProfile { get; private set; }
        public Transform GeneratedFixtures { get; private set; }

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
            ActiveProfile = ResolveFieldProfile();
            ActiveColliderProfile = ResolveColliderProfile();
            GeneratedFixtures = FormationLabRuntimeBuilder.Build(
                transform, ActiveProfile, ActiveColliderProfile);

            if (FormationLabPerformanceProbe.IsRequested() &&
                GetComponent<FormationLabPerformanceProbe>() == null)
            {
                gameObject.AddComponent<FormationLabPerformanceProbe>();
            }
        }

        private FormationLabColliderProfile ResolveColliderProfile()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (!arg.StartsWith(ColliderArgumentPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;
                string requested = arg.Substring(ColliderArgumentPrefix.Length);
                if (Enum.TryParse(requested, true, out FormationLabColliderProfile parsed) &&
                    Enum.IsDefined(typeof(FormationLabColliderProfile), parsed))
                    return parsed;
                Debug.LogWarning(
                    $"[BFC FormationLab] Invalid collider profile '{requested}'; " +
                    $"using {defaultColliderProfile}.");
                break;
            }

            return defaultColliderProfile;
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
