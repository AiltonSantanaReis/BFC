using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace BFC.FormationLab
{
    /// <summary>
    /// Development-only runtime probe used by the Phase 4 Windows performance gate.
    /// It is activated only by the -bfcFormationPerf command-line switch.
    /// No threshold in this class is a product-performance requirement.
    /// </summary>
    public sealed class FormationLabPerformanceProbe : MonoBehaviour
    {
        private const string EnableArgument = "-bfcFormationPerf";
        private const string WarmupArgumentPrefix = "-bfcPerfWarmupSeconds=";
        private const string SampleArgumentPrefix = "-bfcPerfSampleSeconds=";
        private const float DefaultWarmupSeconds = 2f;
        private const float DefaultSampleSeconds = 10f;

        private readonly List<float> _frameTimesMs = new List<float>(4096);

        private float _warmupSeconds;
        private float _sampleSeconds;
        private float _elapsedWarmup;
        private float _elapsedSample;
        private bool _sampling;
        private bool _completed;

        public static bool IsRequested()
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], EnableArgument, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void Awake()
        {
            _warmupSeconds = ReadPositiveFloatArgument(WarmupArgumentPrefix, DefaultWarmupSeconds);
            _sampleSeconds = ReadPositiveFloatArgument(SampleArgumentPrefix, DefaultSampleSeconds);

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
            Application.runInBackground = true;

            Debug.Log(
                $"[BFC FormationPerf] START warmupSeconds={Format(_warmupSeconds)} " +
                $"sampleSeconds={Format(_sampleSeconds)} resolution={Screen.width}x{Screen.height} " +
                $"processor=\"{SystemInfo.processorType}\" gpu=\"{SystemInfo.graphicsDeviceName}\"");
        }

        private void Update()
        {
            if (_completed)
            {
                return;
            }

            float deltaSeconds = Time.unscaledDeltaTime;

            if (!_sampling)
            {
                _elapsedWarmup += deltaSeconds;
                if (_elapsedWarmup >= _warmupSeconds)
                {
                    _sampling = true;
                    Debug.Log("[BFC FormationPerf] Sampling started.");
                }

                return;
            }

            _elapsedSample += deltaSeconds;
            _frameTimesMs.Add(deltaSeconds * 1000f);

            if (_elapsedSample >= _sampleSeconds)
            {
                Complete();
            }
        }

        private void Complete()
        {
            _completed = true;

            if (_frameTimesMs.Count == 0)
            {
                Debug.LogError("[BFC FormationPerf] RESULT unavailable: no frame samples were captured.");
                Quit(2);
                return;
            }

            float[] sorted = _frameTimesMs.ToArray();
            Array.Sort(sorted);

            double sum = 0d;
            for (int i = 0; i < sorted.Length; i++)
            {
                sum += sorted[i];
            }

            float averageMs = (float)(sum / sorted.Length);
            float medianMs = Percentile(sorted, 0.50f);
            float p95Ms = Percentile(sorted, 0.95f);
            float p99Ms = Percentile(sorted, 0.99f);
            float maxMs = sorted[sorted.Length - 1];
            float averageFps = averageMs > 0f ? 1000f / averageMs : 0f;

            Debug.Log(
                $"[BFC FormationPerf] RESULT samples={sorted.Length} " +
                $"durationSeconds={Format(_elapsedSample)} " +
                $"avgMs={Format(averageMs)} medianMs={Format(medianMs)} " +
                $"p95Ms={Format(p95Ms)} p99Ms={Format(p99Ms)} maxMs={Format(maxMs)} " +
                $"avgFps={Format(averageFps)} resolution={Screen.width}x{Screen.height}");

            Quit(0);
        }

        private static float Percentile(float[] sorted, float percentile)
        {
            int index = Mathf.Clamp(
                Mathf.CeilToInt(percentile * sorted.Length) - 1,
                0,
                sorted.Length - 1);
            return sorted[index];
        }

        private static float ReadPositiveFloatArgument(string prefix, float fallback)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (!arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string value = arg.Substring(prefix.Length);
                if (float.TryParse(
                        value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out float parsed) && parsed > 0f)
                {
                    return parsed;
                }
            }

            return fallback;
        }

        private static string Format(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static void Quit(int exitCode)
        {
#if UNITY_EDITOR
            Debug.Log($"[BFC FormationPerf] Editor probe completed with exitCode={exitCode}; Editor remains open.");
#else
            Application.Quit(exitCode);
#endif
        }
    }
}
