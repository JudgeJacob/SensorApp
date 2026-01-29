using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public record SensorReading(string Name, double Value, string Unit, DateTime Timestamp, string Source);

class Program
{
    static void Main()
    {
        var cpuTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        var memAvailableMb = new PerformanceCounter("Memory", "Available MBytes");

        // GPU Engine: utilization per engine instance.
        // This category exists on most Win10/11 systems with WDDM 2.x drivers.
        List<PerformanceCounter> gpuCounters = new();
        try
        {
            var cat = new PerformanceCounterCategory("GPU Engine");
            var instances = cat.GetInstanceNames();

            // "engtype_3D" is the one most people care about.
            // We'll sum all 3D engines to get an overall-ish utilization.
            foreach (var inst in instances.Where(n => n.Contains("engtype_3D", StringComparison.OrdinalIgnoreCase)))
            {
                gpuCounters.Add(new PerformanceCounter("GPU Engine", "Utilization Percentage", inst));
            }
        }
        catch
        {
            // If the category isn't available, gpuCounters stays empty.
        }

        // Warm up counters (first read is often 0)
        _ = cpuTotal.NextValue();
        foreach (var c in gpuCounters) _ = c.NextValue();
        Thread.Sleep(500);

        while (true)
        {
            var now = DateTime.Now;

            var readings = new List<SensorReading>
            {
                new("CPU Usage", cpuTotal.NextValue(), "%", now, "PerfCounter"),
                new("Available Memory", memAvailableMb.NextValue(), "MB", now, "PerfCounter"),
            };

            if (gpuCounters.Count > 0)
            {
                double gpu3d = 0;
                foreach (var c in gpuCounters)
                    gpu3d += c.NextValue();

                // Cap at 100 for nicer display (summing multiple engines can exceed 100)
                gpu3d = Math.Min(100, gpu3d);
                readings.Add(new("GPU 3D Usage", gpu3d, "%", now, "PerfCounter"));
            }
            else
            {
                readings.Add(new("GPU 3D Usage", double.NaN, "%", now, "NotAvailable"));
            }

            Console.Clear();
            foreach (var r in readings)
            {
                var val = double.IsNaN(r.Value) ? "N/A" : r.Value.ToString("0.0");
                Console.WriteLine($"{r.Name,-20} {val,8} {r.Unit}   ({r.Source})");
            }

            Thread.Sleep(1000);
        }
    }
}
