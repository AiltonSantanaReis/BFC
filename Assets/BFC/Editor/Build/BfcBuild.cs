using System.IO;
using BFC.Editor.ProjectSetup;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

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

            BuildPlayer(
                new[] { FormationLabScenePath },
                FormationLabWindowsOutputPath,
                "BFC FormationLab Windows build");
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

            UnityEngine.Debug.Log(
                $"[BFC] {description} succeeded: {outputPath} " +
                $"({report.summary.totalSize} bytes)");
        }
    }
}
