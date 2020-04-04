using System;
using System.Runtime;
using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace ShutterStopper
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
            sb.Append(GCSettings.IsServerGC ? "Server" : "Workstation").Append(" GC, ");
            sb.Append($"Generations: 0..{GC.MaxGeneration}, ");
            sb.Append($"Generation counts: {GetGCCounts()}, ");
            sb.Append($"Latency mode: {GCSettings.LatencyMode}, ");
            sb.Append($"LOH compaction mode: {GCSettings.LargeObjectHeapCompactionMode}, ");
            return sb.ToString();
        }

        public static string GetUnityDescription()
        {
            var sb = new StringBuilder();
            sb.Append($"vSync count: {QualitySettings.vSyncCount}, ");
            sb.Append($"Target FPS: {Application.targetFrameRate}, ");
            sb.Append($"Incremental GC: {GarbageCollector.isIncremental}, {GarbageCollector.incrementalTimeSliceNanoseconds}ns");
            return sb.ToString();
        }
    }
}
