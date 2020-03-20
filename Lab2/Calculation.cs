using System;

namespace Lab2 {
  class Calculation {
    public static double E() {
      double current = 1, previous = 1;
      double item = 1;

      for (int i = 1; i < Constants.MaxIterations; ++i) {
        current += item /= i;
        if (Math.Abs(current - previous) < Constants.EPS)
          return current;
        previous = current;
      }
      return double.NaN;
    }


    public static string PI() {
      string piNumber = "3,";
      int dividedBy = Constants.Pi_DividedBy;
      int result;

      for (int i = 0; i < Constants.Digits; i++) {
        if (dividedBy < Constants.Pi_Divisor)
          dividedBy *= 10;

        result = dividedBy / Constants.Pi_Divisor;

        string resultString = result.ToString();
        piNumber += resultString;

        dividedBy -= Constants.Pi_Divisor * result;
      }

      return piNumber;
    }

    public static double Log2() {
      double result;
      double current = 0;
      double z = Constants.Log2_z;
      double z2 = z * z;

      for (double i = Constants.Log2_x; i > 1; i--)
        current = (Math.Pow((i - 1), 2) * z2) / ((2 * i - 1) - current);

      result = (2 * z) / (1 - current);
      return result;
    }
  }
}
