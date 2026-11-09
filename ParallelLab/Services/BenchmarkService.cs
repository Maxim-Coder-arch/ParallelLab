using ParallelLab.Models;

namespace ParallelLab.Services;

public class BenchmarkService
{
    private readonly SequentialProcessor _sequentialProcessor;
    private readonly MultithreadedProcessor _multithreadedProcessor;

    public BenchmarkService(
        SequentialProcessor sequentialProcessor,
        MultithreadedProcessor multithreadedProcessor)
    {
        _sequentialProcessor = sequentialProcessor;
        _multithreadedProcessor = multithreadedProcessor;
    }

    public BenchmarkReport Run(
        int[] data,
        int iterations,
        int threadCount,
        int repetitions = 5)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (repetitions < 1)
            throw new ArgumentOutOfRangeException(nameof(repetitions));

        _sequentialProcessor.Process(data, iterations);

        _multithreadedProcessor.Process(
            data,
            iterations,
            threadCount
        );

        List<BenchmarkResult> sequentialResults = new();
        List<BenchmarkResult> multithreadedResults = new();

        for (int i = 0; i < repetitions; i++)
        {
            BenchmarkResult sequentialResult;
            BenchmarkResult multithreadedResult;

            if (i % 2 == 0)
            {
                sequentialResult = _sequentialProcessor.Process(
                    data,
                    iterations
                );

                multithreadedResult = _multithreadedProcessor.Process(
                    data,
                    iterations,
                    threadCount
                );
            }
            else
            {
                multithreadedResult = _multithreadedProcessor.Process(
                    data,
                    iterations,
                    threadCount
                );

                sequentialResult = _sequentialProcessor.Process(
                    data,
                    iterations
                );
            }

            ValidateResults(sequentialResult, multithreadedResult);

            sequentialResults.Add(sequentialResult);
            multithreadedResults.Add(multithreadedResult);
        }

        return new BenchmarkReport(
            sequentialResults,
            multithreadedResults
        );
    }

    private static void ValidateResults(
        BenchmarkResult sequential,
        BenchmarkResult multithreaded)
    {
        if (sequential.Computation.ProcessedItems !=
            multithreaded.Computation.ProcessedItems)
        {
            throw new InvalidOperationException(
                "Количество обработанных элементов не совпадает."
            );
        }

        if (sequential.Computation.Checksum !=
            multithreaded.Computation.Checksum)
        {
            throw new InvalidOperationException(
                "Контрольные суммы последовательной и многопоточной " +
                "обработки не совпадают."
            );
        }
    }
}