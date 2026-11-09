using ParallelLab.Models;

namespace ParallelLab.UI;

public class ConsoleRenderer
{
    public void RenderReport(BenchmarkReport report)
    {
        Console.WriteLine();
        Console.WriteLine("==============================================");
        Console.WriteLine("             РЕЗУЛЬТАТЫ ТЕСТИРОВАНИЯ");
        Console.WriteLine("==============================================");

        Console.WriteLine();

        Console.WriteLine(
            $"{"Запуск",-10} {"Последовательно",-22} {"Многопоточно",-22}"
        );

        Console.WriteLine(new string('-', 56));

        for (int i = 0; i < report.SequentialResults.Count; i++)
        {
            double sequentialTime =
                report.SequentialResults[i].Elapsed.TotalMilliseconds;

            double multithreadedTime =
                report.MultithreadedResults[i].Elapsed.TotalMilliseconds;

            Console.WriteLine(
                $"{i + 1,-10} {sequentialTime,-22:F2} {multithreadedTime,-22:F2}"
            );
        }

        Console.WriteLine(new string('-', 56));

        double averageSequential = report.SequentialResults
            .Average(result => result.Elapsed.TotalMilliseconds);

        double averageMultithreaded = report.MultithreadedResults
            .Average(result => result.Elapsed.TotalMilliseconds);

        double speedup = averageMultithreaded > 0
            ? averageSequential / averageMultithreaded
            : 0;

        Console.WriteLine();
        Console.WriteLine($"Среднее последовательное время: {averageSequential:F2} мс");
        Console.WriteLine($"Среднее многопоточное время:    {averageMultithreaded:F2} мс");
        Console.WriteLine($"Ускорение:                      {speedup:F2}x");

        Console.WriteLine();

        if (speedup > 1)
            Console.WriteLine("Многопоточная обработка оказалась быстрее.");
        else if (speedup < 1)
            Console.WriteLine("Последовательная обработка оказалась быстрее.");
        else
            Console.WriteLine("Обе реализации показали одинаковое время.");

        Console.WriteLine();

        BenchmarkResult sequential = report.SequentialResults[0];
        BenchmarkResult multithreaded = report.MultithreadedResults[0];

        Console.WriteLine("Проверка корректности:");

        Console.WriteLine(
            $"Обработано элементов: {sequential.Computation.ProcessedItems}"
        );

        Console.WriteLine(
            $"Контрольная сумма: {sequential.Computation.Checksum}"
        );

        Console.WriteLine(
            $"Контрольные суммы совпадают: " +
            $"{sequential.Computation.Checksum == multithreaded.Computation.Checksum}"
        );

        Console.WriteLine("==============================================");
    }
}