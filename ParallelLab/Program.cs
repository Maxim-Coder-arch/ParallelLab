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





/*
1. Почему результаты измерений могут отличаться при каждом запуске?

На время выполнения влияют загрузка процессора, 
фоновые процессы операционной системы, 
планирование потоков и другие факторы. 
Поэтому даже при одинаковых входных данных 
результаты могут немного отличаться.

2. Всегда ли многопоточная обработка быстрее последовательной?

Нет. Многопоточность не гарантирует ускорение. 
Результат зависит от объёма вычислений, количества потоков, 
числа ядер процессора и накладных расходов на создание и 
синхронизацию потоков. В нашем эксперименте многопоточная 
обработка оказалась быстрее в среднем в 2,59 раза.

3. Что изменится, если использовать реальные вычисления вместо Thread.Sleep(100)?

При использовании Thread.Sleep() потоки преимущественно ожидают, не 
выполняя полезных вычислений. При реальных CPU-bound 
операциях потоки действительно конкурируют за процессорное 
время и могут параллельно использовать несколько ядер. Поэтому 
такой эксперимент лучше демонстрирует преимущества многопоточности 
для вычислительных задач.

4. Почему создание потоков невыгодно для очень маленьких задач?

Создание и запуск потоков требуют дополнительных ресурсов и времени. 
Если сама задача выполняется очень быстро, накладные расходы 
на управление потоками могут оказаться больше времени, 
которое удалось бы сэкономить за счёт параллельного выполнения.

Дополнительное задание
Как обрабатывать исключения в рабочих потоках?

Для этого в каждом рабочем потоке используется try-catch. 
Возникшие исключения помещаются в потокобезопасную очередь 
ConcurrentQueue<Exception>. После завершения всех потоков главный 
поток проверяет очередь и при наличии ошибок выбрасывает AggregateException.
 */