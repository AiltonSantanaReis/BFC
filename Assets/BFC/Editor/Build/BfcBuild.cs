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

        [MenuItem("BFC/Build/Windows x64")]
        public static void BuildWindows64()
        {
            BfcProjectSetup.ApplyPhase1Setup();
            Directory.CreateDirectory("Builds/Windows");

            var options = new BuildPlayerOptions
            {
                scenes = new[] { BfcProjectSetup.BootstrapScenePath },
                locationPathName = WindowsOutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"BFC Windows build failed: {report.summary.result}; " +
                    $"errors={report.summary.totalErrors}, warnings={report.summary.totalWarnings}");
            }

            UnityEngine.Debug.Log(
                $"[BFC] Windows build succeeded: {WindowsOutputPath} " +
                $"({report.summary.totalSize} bytes)");
        }
    }
}
