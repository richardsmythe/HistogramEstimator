public static class DataGenerator
{
    public static double[] GenerateUniformData(int numSamples, double minVal, double maxVal)
    {
        Random random = new Random();
        return Enumerable.Range(0, numSamples).Select(i => minVal + random.NextDouble() * (maxVal - minVal)).ToArray();
    }

    public static double[] GenerateNormalData(int numSamples, double mean, double stdDev)
    {
        Random random = new Random();
        return Enumerable.Range(0, numSamples).Select(i =>
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }).ToArray();
    }

    public static double[] GenerateExponentialData(int numSamples, double lambda)
    {
        Random random = new Random();
        return Enumerable.Range(0, numSamples).Select(i => -Math.Log(1 - random.NextDouble()) / lambda).ToArray();
    }

    public static double[] GenerateDiverseData(int numSamples, double minVal, double maxVal)
    {
        int third = numSamples / 3;
        double[] data1 = GenerateUniformData(third, minVal, maxVal);
        double[] data2 = GenerateNormalData(third, (maxVal + minVal) / 2, (maxVal - minVal) / 4);
        double[] data3 = GenerateExponentialData(third, 2.0 / (maxVal - minVal));
        return data1.Concat(data2).Concat(data3).OrderBy(x => Guid.NewGuid()).ToArray();
    }
}

