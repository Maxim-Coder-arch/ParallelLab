using System.Diagnostics;
using ParallelLab.Models;

namespace ParallelLab.Services;

public class SequentialProcessor
{
    private readonly ComputationService _computationService;

    public SequentialProcessor(ComputationService computationService)
    {
        _computationService = computationService;
    }

    public BenchmarkResult Process(int[] data, int iterations)
    {
        ArgumentNullException.ThrowIfNull(data);

        Stopwatch stopwatch = Stopwatch.StartNew();

        ComputationResult computation = _computationService.ComputeRange(
            data,
            0,
            data.Length,
            iterations
        );

        stopwatch.Stop();

        return new BenchmarkResult(
            "Последовательная обработка",
            stopwatch.Elapsed,
            computation
        );
    }
}