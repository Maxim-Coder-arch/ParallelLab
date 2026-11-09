using ParallelLab.Models;
using ParallelLab.Services;
using ParallelLab.UI;

const int DataSize = 100_000;
const int Iterations = 50;
const int ThreadCount = 4;
const int Repetitions = 5;
const int IncrementsPerThread = 1_000_000;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==============================================");
Console.WriteLine("              PARALLEL LAB");
Console.WriteLine("       Лаборатория многопоточности C#");
Console.WriteLine("==============================================");

Console.WriteLine();
Console.WriteLine("Параметры эксперимента:");
Console.WriteLine($"Количество элементов: {DataSize:N0}");
Console.WriteLine($"Итераций на элемент:  {Iterations}");
Console.WriteLine($"Рабочих потоков:      {ThreadCount}");
Console.WriteLine($"Повторений:           {Repetitions}");

int[] data = Enumerable
    .Range(1, DataSize)
    .Select(value => value % 1000 + 1)
    .ToArray();

ComputationService computationService = new();

SequentialProcessor sequentialProcessor = new(computationService);

MultithreadedProcessor multithreadedProcessor = new(computationService);

BenchmarkService benchmarkService = new(
    sequentialProcessor,
    multithreadedProcessor
);

Console.WriteLine();
Console.WriteLine("Выполнение основного эксперимента...");

BenchmarkReport report = benchmarkService.Run(
    data,
    Iterations,
    ThreadCount,
    Repetitions
);

ConsoleRenderer renderer = new();

renderer.RenderReport(report);

Console.WriteLine();
Console.WriteLine("Выполнение эксперимента со счётчиком...");

CounterExperimentService counterService = new();

CounterExperimentResult counterResult = counterService.Run(
    ThreadCount,
    IncrementsPerThread
);

renderer.RenderCounterExperiment(counterResult);

Console.WriteLine();
Console.WriteLine("Проверка обработки исключений...");

try
{
    multithreadedProcessor.Process(
        data,
        iterations: 0,
        threadCount: ThreadCount
    );
}
catch (AggregateException exception)
{
    Console.WriteLine("Ошибки успешно перехвачены.");

    foreach (Exception error in exception.InnerExceptions)
    {
        Console.WriteLine(error.Message);
        Console.WriteLine($"Причина: {error.InnerException?.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("Все эксперименты завершены.");