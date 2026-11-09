namespace ParallelLab.Models;

public sealed record BenchmarkReport(
    IReadOnlyList<BenchmarkResult> SequentialResults,
    IReadOnlyList<BenchmarkResult> MultithreadedResults
);