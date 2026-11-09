namespace ParallelLab.Models;
public sealed record ComputationResult(
    long ProcessedItems,
    long Checksum
);