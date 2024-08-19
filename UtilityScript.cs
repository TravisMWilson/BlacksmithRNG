using System;

public class UtilityScript {
    #region PrivateVariables

    private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qdt", "Qnt", "Sxt", "Spt", "Oct", "Nnt", "Dct" };

    #endregion
    #region PublicMethods

    public static string FormatNumber(double number) {
        if (number == 0) return "0";

        int suffixIndex = 0;
        double tempNumber = number;

        while (Math.Abs(tempNumber) >= 1000 && suffixIndex < Suffixes.Length - 1) {
            tempNumber /= 1000;
            suffixIndex++;
        }

        return tempNumber.ToString("0.#") + Suffixes[suffixIndex];
    }

    #endregion
}
