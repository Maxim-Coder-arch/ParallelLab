using ParallelLab.Models;

namespace ParallelLab.Services;

public class CounterExperimentService
{
    public CounterExperimentResult Run(
        int threadCount,
        int incrementsPerThread)
    {
        if (threadCount < 1)
            throw new ArgumentOutOfRangeException(nameof(threadCount));

        if (incrementsPerThread < 1)
            throw new ArgumentOutOfRangeException(nameof(incrementsPerThread));

        long expectedCount = (long)threadCount * incrementsPerThread;

        long unsafeCounter = 0;

        Thread[] unsafeThreads = new Thread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            unsafeThreads[i] = new Thread(() =>
            {
                for (int j = 0; j < incrementsPerThread; j++)
                {
                    unsafeCounter++;
                }
            });

            unsafeThreads[i].Start();
        }

        foreach (Thread thread in unsafeThreads)
        {
            thread.Join();
        }

        long safeCounter = 0;

        Thread[] safeThreads = new Thread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            safeThreads[i] = new Thread(() =>
            {
                for (int j = 0; j < incrementsPerThread; j++)
                {
                    Interlocked.Increment(ref safeCounter);
                }
            });

            safeThreads[i].Start();
        }

        foreach (Thread thread in safeThreads)
        {
            thread.Join();
        }

        return new CounterExperimentResult(
            expectedCount,
            unsafeCounter,
            safeCounter
        );
    }
}