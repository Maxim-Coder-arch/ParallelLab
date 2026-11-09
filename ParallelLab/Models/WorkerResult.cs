namespace ParallelLab.Models;

public sealed record WorkerResult(
    int WorkerId,
    int StartIndex,
    int EndIndex,
    long ProcessedItems,
    long Checksum
);