using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BFC.Editor.ProjectSetup
{
    /// <summary>
    /// Idempotent Phase 1 project bootstrap. It creates the URP assets when the Unity Editor
    /// first opens the repository and applies only platform/presentation-neutral project settings.
    /// Gameplay tuning does not belong here.
    /// </summary>
    public static class BfcProjectSetup
    {
        public const string BootstrapScenePath = "Assets/BFC/Scenes/Bootstrap.unity";
        public const string PhysicsLabScenePath = "Assets/BFC/Scenes/PhysicsLab.unity";
        public const string PipelineAssetPath = "Assets/BFC/Settings/BFC_URP.asset";
        public const string RendererAssetPath = "Assets/BFC/Settings/BFC_UniversalRenderer.asset";

        private const string SessionKey = "BFC.Phase1ProjectSetupApplied";

        [InitializeOnLoadMethod]
        private static void ScheduleAutomaticSetup()
        {
            if (SessionState.GetBool(SessionKey, false))
            {
                return;
            }

            SessionState.SetBool(SessionKey, true);
            EditorApplication.delayCall += ApplyPhase1Setup;
        }

        [MenuItem("BFC/Setup/Apply Phase 1 Project Setup")]
        public static void ApplyPhase1Setup()
        {
            EnsureDirectories();
            EnsureScenesExist();
            UniversalRenderPipelineAsset pipelineAsset = EnsureUrpAssets();
            ApplyProjectSettings(pipelineAsset);
            ApplyBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[BFC] Phase 1 Unity project setup applied.");
        }

        private static void EnsureDirectories()
        {
            Directory.CreateDirectory("Assets/BFC/Scenes");
            Directory.CreateDirectory("Assets/BFC/Settings");
            Directory.CreateDirectory("Builds/Windows");
            Directory.CreateDirectory("Logs");
        }

        private static UniversalRenderPipelineAsset EnsureUrpAssets()
        {
            UniversalRenderPipelineAsset pipelineAsset =
                AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);

            if (pipelineAsset != null)
            {
                return pipelineAsset;
            }

            pipelineAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            pipelineAsset.name = "BFC_URP";
            pipelineAsset.useSRPBatcher = true;
            AssetDatabase.CreateAsset(pipelineAsset, PipelineAssetPath);

            ScriptableRendererData rendererData =
                pipelineAsset.LoadBuiltinRendererData(RendererType.UniversalRenderer);

            if (rendererData != null && !AssetDatabase.Contains(rendererData))
            {
                rendererData.name = "BFC_UniversalRenderer";
                AssetDatabase.CreateAsset(rendererData, RendererAssetPath);
            }

            EditorUtility.SetDirty(pipelineAsset);
            AssetDatabase.SaveAssets();
            return pipelineAsset;
        }

        private static void ApplyProjectSettings(UniversalRenderPipelineAsset pipelineAsset)
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;

            PlayerSettings.productName = "BFC";
            PlayerSettings.companyName = "BFC";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.resizableWindow = true;

            GraphicsSettings.defaultRenderPipeline = pipelineAsset;
            QualitySettings.renderPipeline = null;
        }

        private static void ApplyBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(PhysicsLabScenePath, false)
            };
        }

        private static void EnsureScenesExist()
        {
            if (!File.Exists(BootstrapScenePath))
            {
                CreateBootstrapScene();
            }

            if (!File.Exists(PhysicsLabScenePath))
            {
                CreatePhysicsLabScene();
            }
        }

        private static void CreateBootstrapScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera("Main Camera", new Vector3(0f, 2f, -10f), Quaternion.identity);
            CreateDirectionalLight();
            new GameObject("BFC Bootstrap").AddComponent<BFC.Bootstrap.BfcBootstrap>();
            EditorSceneManager.SaveScene(scene, BootstrapScenePath);
        }

        private static void CreatePhysicsLabScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera(
                "Main Camera",
                new Vector3(0f, 10f, -12f),
                Quaternion.Euler(35f, 0f, 0f));
            CreateDirectionalLight();

            var root = new GameObject("PhysicsLab Root");
            new GameObject("Surface Fixture").transform.SetParent(root.transform, false);
            new GameObject("Piece A Fixture").transform.SetParent(root.transform, false);
            new GameObject("Ball Fixture").transform.SetParent(root.transform, false);
            new GameObject("Piece B Fixture").transform.SetParent(root.transform, false);

            EditorSceneManager.SaveScene(scene, PhysicsLabScenePath);
        }

        private static void CreateCamera(string name, Vector3 position, Quaternion rotation)
        {
            var cameraObject = new GameObject(name);
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetPositionAndRotation(position, rotation);
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreateDirectionalLight()
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
        }
    }
}
