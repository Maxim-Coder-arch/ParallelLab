using ParallelLab.Models;

namespace ParallelLab.Models;

public sealed record BenchmarkResult(
    string Name,
    TimeSpan Elapsed,
    ComputationResult Computation
);