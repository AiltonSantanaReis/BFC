using System;
using System.IO;
using BFC.Editor.ProjectSetup;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BFC.Editor.Build
{
    /// <summary>
    /// Reproducible player build entry points for local development and CI.
    /// </summary>
    public static class BfcBuild
    {
        public const string WindowsOutputPath = "Builds/Windows/BFC.exe";
        public const string FormationLabWindowsOutputPath = "Builds/FormationLab/BFC-FormationLab.exe";
        public const string FormationLabScenePath = "Assets/BFC/Scenes/FormationLab.unity";

        private const string FormationLabBuildAssetsDirectory =
            "Assets/BFC/Settings/FormationLabBuildAssets";
        private const string FormationLabBuildResourcesDirectory =
            FormationLabBuildAssetsDirectory + "/Resources";
        private const string FormationLabRuntimeMaterialAssetPath =
            FormationLabBuildResourcesDirectory + "/BFCFormationLabRuntimeMaterial.mat";

        [MenuItem("BFC/Build/Windows x64")]
        public static void BuildWindows64()
        {
            BfcProjectSetup.ApplyPhase1Setup();
            Directory.CreateDirectory("Builds/Windows");

            BuildPlayer(
                new[] { BfcProjectSetup.BootstrapScenePath },
                WindowsOutputPath,
                "BFC Windows build");
        }

        [MenuItem("BFC/Build/FormationLab Performance Windows x64")]
        public static void BuildFormationLabWindows64()
        {
            BfcProjectSetup.ApplyPhase1Setup();
            Directory.CreateDirectory("Builds/FormationLab");

            CreateFormationLabRuntimeMaterialResource();
            try
            {
                BuildPlayer(
                    new[] { FormationLabScenePath },
                    FormationLabWindowsOutputPath,
                    "BFC FormationLab Windows build");
            }
            finally
            {
                RemoveFormationLabBuildAssets();
            }
        }

        private static void CreateFormationLabRuntimeMaterialResource()
        {
            RemoveFormationLabBuildAssets();

            Directory.CreateDirectory(FormationLabBuildResourcesDirectory);
            AssetDatabase.Refresh();

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new InvalidOperationException(
                    "Could not resolve the URP Lit shader while preparing the FormationLab build resource.");
            }

            var material = new Material(shader)
            {
                name = "BFCFormationLabRuntimeMaterial"
            };

            AssetDatabase.CreateAsset(material, FormationLabRuntimeMaterialAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void RemoveFormationLabBuildAssets()
        {
            if (!AssetDatabase.IsValidFolder(FormationLabBuildAssetsDirectory))
            {
                return;
            }

            AssetDatabase.DeleteAsset(FormationLabBuildAssetsDirectory);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void BuildPlayer(string[] scenes, string outputPath, string description)
        {
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"{description} failed: {report.summary.result}; " +
                    $"errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}");
            }

            Debug.Log(
                $"[BFC] {description} succeeded: {outputPath} " +
                $"({report.summary.totalSize} bytes)");
        }
    }
}
