using ParallelLab.Models;
using ParallelLab.Services;
using ParallelLab.UI;

const int DataSize = 100_000;
const int Iterations = 50;
const int ThreadCount = 4;
const int Repetitions = 5;

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

Console.WriteLine();
Console.WriteLine("Подготовка данных...");

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

Console.WriteLine("Данные подготовлены.");

Console.WriteLine();
Console.WriteLine("Выполнение эксперимента...");
Console.WriteLine("Пожалуйста, подождите.");

BenchmarkReport report = benchmarkService.Run(
    data,
    Iterations,
    ThreadCount,
    Repetitions
);

ConsoleRenderer renderer = new();

renderer.RenderReport(report);

Console.WriteLine();
Console.WriteLine("Эксперимент завершён.");