using System;

public static class RandomExtensions {
    #region PublicMethods

    public static double NextDouble(this Random random, double minValue, double maxValue) {
        if (minValue > maxValue) {
            throw new ArgumentOutOfRangeException(nameof(minValue), "minValue is greater than maxValue");
        }

        double range = maxValue - minValue;
        double randomDouble = random.NextDouble() * range + minValue;

        return randomDouble;
    }

    #endregion
}
