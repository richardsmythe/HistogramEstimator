
/// <summary>
///  A dynamic equi-depth histogram creation, with the rebalancing happening on every addition.
/// </summary>
public class HistogramEstimator
{
    private readonly int[] _counts;
    private readonly double[] _boundaries;
    private int _totalCount;
    private readonly int _maxBins;

    public HistogramEstimator(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Number of bins must be non-negative.");
        _counts = new int[n];
        _boundaries = new double[n + 1];
        _totalCount = 0;
        _maxBins = n;
    }

    public int[] Counts => _counts;
    public double[] Boundaries => _boundaries;

    /// <summary>
    /// Finds the bin that a given value falls into based on the current bin boundaries.
    /// It loops through the boundaries array and returns the index of the bin that contains the value.
    /// If the value doesn't fall between any two boundaries, it returns the last bin (this happens if the value is higher than the last boundary).
    /// </summary>
    private int FindBin(double value)
    {
        for (int i = 0; i < _boundaries.Length - 1; i++)
        {
            if (value >= _boundaries[i] && value <= _boundaries[i + 1])
            {
                return i;
            }
        }
        return _boundaries.Length - 2;
    }

    /// <summary>
    /// Adds a new sample to the histogram.
    /// </summary>
    /// <param name="s"></param>
    public void Add(double s)
    {
        if (_totalCount == 0)
        {
            _boundaries[0] = s;
            _totalCount++;
            return;
        }

        if (_totalCount == 1)
        {
            _boundaries[_boundaries.Length - 1] = s;
        }

        if (s < _boundaries[0]) _boundaries[0] = s;
        if (s > _boundaries[_boundaries.Length - 1]) _boundaries[_boundaries.Length - 1] = s;

        int i = FindBin(s);
        _counts[i]++;
        _totalCount++;

        RebalanceBins();
    }

    /// <summary>
    /// This method redistributes the samples into a smaller number of bins with more even distribution.
    /// It recalculates the boundaries for each bin based on the running total distribution of the data points.
    /// The bin counts are reset, and the data points are redistributed into the new bins.
    /// Rebalancing is triggered after each new data point is added, ensuring that the histogram adapts to the growing dataset.
    /// </summary>
    private void RebalanceBins()
    {
        if (_totalCount <= _maxBins) return;
        int targetCountPerBin = _totalCount / _maxBins;

        int[] runningTotal = new int[_maxBins];
        runningTotal[0] = _counts[0];
        for (int i = 1; i < _maxBins; i++)
        {
            runningTotal[i] = runningTotal[i - 1] + _counts[i];
        }

        double minVal = _boundaries[0];
        double maxVal = _boundaries[_boundaries.Length - 1];

        double[] newBoundaries = new double[_maxBins + 1];
        newBoundaries[0] = minVal;
        newBoundaries[_maxBins] = maxVal;

        int boundaryIndex = 1;

        for (int i = 0; i < _maxBins - 1; i++)
        {
            int targetRunningTotal = (i + 1) * targetCountPerBin;
            int closestIndex = Array.BinarySearch(runningTotal, targetRunningTotal);
            if (closestIndex < 0) closestIndex = ~closestIndex;

            double lowerBoundary = closestIndex > 0 ? _boundaries[closestIndex] : minVal;
            double upperBoundary = closestIndex < _boundaries.Length - 1 ? _boundaries[closestIndex + 1] : maxVal;

            int lowerBoundIndex = closestIndex > 0 ? closestIndex : 0;
            int upperBoundIndex = closestIndex < _boundaries.Length - 1 ? closestIndex + 1 : _boundaries.Length - 1;

            int previousCount = lowerBoundIndex > 0 ? runningTotal[lowerBoundIndex - 1] : 0;
            int currentBinCount = runningTotal[upperBoundIndex - 1] - previousCount;

            double proportion = currentBinCount > 0 ? (double)(targetRunningTotal - previousCount) / currentBinCount : 0;

            newBoundaries[boundaryIndex++] = lowerBoundary + proportion * (upperBoundary - lowerBoundary);
        }

        Array.Copy(newBoundaries, _boundaries, newBoundaries.Length);
        Array.Clear(_counts, 0, _counts.Length);

        // Redistribute the samples now across the new bins now that boundries are adjusted.
        double difference = (maxVal - minVal) / (_totalCount - 1);
        for (int j = 0; j < _totalCount; j++) 
        {
            double value = minVal + j * difference;
            int binIndex = FindBin(value);
            if (binIndex >= 0 && binIndex < _counts.Length)
            {
                _counts[binIndex]++;
            }
        }
    }
}



