using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using UnityEngine;


public class MemoryStatsScript : MonoBehaviour
{
    string statsText;
    ProfilerRecorder totalReservedMemoryRecorder;
    ProfilerRecorder gcReservedMemoryRecorder;
    ProfilerRecorder systemUsedMemoryRecorder;
    ProfilerRecorder totalUsedMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        for (var i = 0; i < samples.Count; ++i)
            r += samples[i].Value;
        r /= samplesCount;

        return r;
    }

    void OnEnable()
    {
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);

    }

    void OnDisable()
    {
        totalReservedMemoryRecorder.Dispose();
        gcReservedMemoryRecorder.Dispose();
        systemUsedMemoryRecorder.Dispose();
        totalUsedMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();

    }

    void Update()
    {
        var sb = new StringBuilder(500);
        if (totalReservedMemoryRecorder.Valid)
            sb.AppendLine($"Total Reserved Memory: {totalReservedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        if (gcReservedMemoryRecorder.Valid)
            sb.AppendLine($"GC Reserved Memory: {gcReservedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        if (systemUsedMemoryRecorder.Valid)
            sb.AppendLine($"System Used Memory: {systemUsedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        if (totalUsedMemoryRecorder.Valid)
            sb.AppendLine($"Total Used Memory: {totalUsedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        statsText = sb.ToString();
        if (mainThreadTimeRecorder.Valid)
            sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 20, 250, 80), statsText);
    }
}
