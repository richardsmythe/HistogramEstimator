using System.ComponentModel;
using System.Net;
using System.Reflection.Metadata.Ecma335;

internal class Program
{
    public static void Main()
    {

        var histogramEstimator = new HistogramEstimator(10);
        double[] sampleData = DataGenerator.GenerateDiverseData(100, 0, 100);

        foreach (double dataPoint in sampleData)
        {
            histogramEstimator.Add(dataPoint);
            PrintHistogram(histogramEstimator);
        }
    }

    public static void PrintHistogram(HistogramEstimator histogramEstimator)
    {
        Console.WriteLine("Current histogram state:");

        for (int i = 0; i < histogramEstimator.Counts.Length; i++)
        {
            double lowerBound = histogramEstimator.Boundaries[i];
            double upperBound = histogramEstimator.Boundaries[i + 1];
            int count = histogramEstimator.Counts[i];
            Console.WriteLine($"Bin {i + 1}: {lowerBound:F2} - {upperBound:F2} | Count: {count}");
        }

        Console.WriteLine();
    }

}



