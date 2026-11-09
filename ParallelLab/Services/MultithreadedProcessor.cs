using System.Collections.Concurrent;
using System.Diagnostics;
using ParallelLab.Models;

namespace ParallelLab.Services;

public class MultithreadedProcessor
{
    private readonly ComputationService _computationService;

    public MultithreadedProcessor(ComputationService computationService)
    {
        _computationService = computationService;
    }

    public BenchmarkResult Process(
        int[] data,
        int iterations,
        int threadCount)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (threadCount < 1)
            throw new ArgumentOutOfRangeException(nameof(threadCount));

        Stopwatch stopwatch = Stopwatch.StartNew();

        Thread[] threads = new Thread[threadCount];
        WorkerResult?[] results = new WorkerResult?[threadCount];

        ConcurrentQueue<Exception> errors = new();

        int itemsPerThread = data.Length / threadCount;

        for (int i = 0; i < threadCount; i++)
        {
            int workerId = i + 1;

            int startIndex = i * itemsPerThread;

            int endIndex = i == threadCount - 1
                ? data.Length
                : startIndex + itemsPerThread;

            int currentIndex = i;

            threads[i] = new Thread(() =>
            {
                try
                {
                    ComputationResult computation =
                        _computationService.ComputeRange(
                            data,
                            startIndex,
                            endIndex,
                            iterations
                        );

                    results[currentIndex] = new WorkerResult(
                        workerId,
                        startIndex,
                        endIndex,
                        computation.ProcessedItems,
                        computation.Checksum
                    );
                }
                catch (Exception exception)
                {
                    errors.Enqueue(
                        new InvalidOperationException(
                            $"Ошибка в рабочем потоке №{workerId}.",
                            exception
                        )
                    );
                }
            });

            threads[i].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        stopwatch.Stop();

        if (!errors.IsEmpty)
        {
            throw new AggregateException(
                "Во время многопоточной обработки возникли ошибки.",
                errors
            );
        }

        long totalProcessedItems = 0;
        long totalChecksum = 0;

        foreach (WorkerResult? result in results)
        {
            if (result is null)
                throw new InvalidOperationException(
                    "Один из рабочих потоков не вернул результат."
                );

            totalProcessedItems += result.ProcessedItems;
            totalChecksum += result.Checksum;
        }

        ComputationResult totalComputation = new(
            totalProcessedItems,
            totalChecksum
        );

        return new BenchmarkResult(
            $"Многопоточная обработка ({threadCount} потоков)",
            stopwatch.Elapsed,
            totalComputation
        );
    }
}