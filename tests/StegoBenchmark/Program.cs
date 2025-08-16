using BenchmarkDotNet.Running;

namespace StegoBenchmark;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<LsbPerformance>();
    }
}