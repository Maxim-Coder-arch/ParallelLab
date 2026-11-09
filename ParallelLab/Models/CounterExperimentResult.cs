namespace ParallelLab.Models;

public sealed record CounterExperimentResult(
    long ExpectedCount,
    long UnsafeCount,
    long SafeCount
);