using ParallelLab.Models;

namespace ParallelLab.Services;

public class ComputationService
{
    public ComputationResult ComputeRange(
        int[] data,
        int startIndex,
        int endIndex,
        int iterations)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (startIndex < 0 || endIndex > data.Length || startIndex > endIndex)
            throw new ArgumentOutOfRangeException(
                nameof(startIndex),
                "Некорректный диапазон обработки."
            );

        if (iterations < 1)
            throw new ArgumentOutOfRangeException(
                nameof(iterations),
                "Количество итераций должно быть больше нуля."
            );

        long checksum = 0;
        long processedItems = 0;

        for (int i = startIndex; i < endIndex; i++)
        {
            checksum = checked(checksum + ComputeItem(data[i], iterations));
            processedItems++;
        }

        return new ComputationResult(processedItems, checksum);
    }

    private static long ComputeItem(int number, int iterations)
    {
        double value = number;

        for (int i = 0; i < iterations; i++)
        {
            value = Math.Sqrt(value + i + 1) * 1.000001
                    + Math.Sin(value);
        }

        return (long)Math.Round(value * 1_000_000);
    }
}