using System;
using System.Runtime;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace LagKiller
{
    public static class GCInfo
    {
        public static readonly int MaxGeneration = GC.MaxGeneration;

        public static int GetCollectionCount(int generation)
            => generation < MaxGeneration ? GC.CollectionCount(generation) : 0;

        public static (int, int, int) GetGCCounts()
            => (GetCollectionCount(0), GetCollectionCount(1), GetCollectionCount(2));

        public static (int, int, int) GetGCCountDelta(
            (int, int, int) now,
            (int, int, int) earlier)
            => (now.Item1 - earlier.Item1,
                now.Item2 - earlier.Item2,
                now.Item3 - earlier.Item3);

        public static string GetDotNetDescription()
        {
            var sb = new StringBuilder();
            sb.Append(GCSettings.IsServerGC ? "Server" : "Workstation").Append(" GC: ");
            sb.Append($"Generations: {GC.MaxGeneration} ({GetGCCounts()}), ");
            sb.Append($"Latency mode: {GCSettings.LatencyMode}, ");
            sb.Append($"LOH compaction mode: {GCSettings.LargeObjectHeapCompactionMode}");
            return sb.ToString();
        }

        public static string GetUnityDescription()
        {
            var sb = new StringBuilder();
            sb.Append($"Unity GC: {(GarbageCollector.isIncremental ? "Incremental" : "Non-incremental")} & {GarbageCollector.GCMode}, ");
            sb.Append($"GC budget: {GarbageCollector.incrementalTimeSliceNanoseconds}ns, ");
            sb.Append($"vSync count: {QualitySettings.vSyncCount}, ");
            sb.Append($"Target FPS: {Application.targetFrameRate}");
            return sb.ToString();
        }

        public static string GetSummary()
        {
            var incremental = GarbageCollector.isIncremental ? "Incremental" : "Non-incremental";
            var enabled = GarbageCollector.GCMode;
            var budgetMs = GarbageCollector.incrementalTimeSliceNanoseconds / 1_000_000.0;
            return $"{incremental} & {enabled}, budget {budgetMs:F3}ms";
        }
    }
}
