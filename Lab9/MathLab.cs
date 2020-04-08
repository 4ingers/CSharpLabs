using System;

namespace Space9 {
  class MathLab {
    public static (double, double) X1range = (-2, -1);
    public static (double, double) X2range = (3, 4);

    public static double F(double x) => Math.Pow(x, 4) + Math.Pow(x, 3) - 6 * Math.Pow(x, 2) - 20 * x - 16;

    public static double Bisection(Func<double, double> f, (double a, double b) range, double eps) {
      double x_1 = range.a;
      double x_2 = range.b;
      double fb = f(x_2);

      while (Math.Abs(x_2 - x_1) >= eps) {
        double x_3 = (x_1 + x_2) / 2.0;
        if (fb * f(x_3) > 0)
          x_2 = x_3;
        else
          x_1 = x_3;
      }
      return x_2 - f(x_2) * (x_2 - x_1) / (f(x_2) - f(x_1));
    }
  }
}
