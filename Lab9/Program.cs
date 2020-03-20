using System;

namespace Lab9
{
  class Program {
    public delegate double FuncValue(double x);
    static void Main(string[] args) {
      Console.WriteLine(Bisection(f, -2, -1, 10e-5));
      Console.WriteLine(Bisection(f, 3, 4, 10e-5));
    }

    static double f(double x) {
      return Math.Pow(x, 4) + Math.Pow(x, 3) - 6 * Math.Pow(x,2) - 20 * x - 16;
    }

    static double Bisection(
      FuncValue f,
      double a,
      double b,
      double EPSILON = 1e-10
    ) {
      var x1 = a;
      var x2 = b;
      var f_b = f(b);
      var iter = 0;

      while (Math.Abs(x2 - x1) > EPSILON) {
          iter++;
          var midPoint = 0.5 * (x1 + x2);
          if (f_b * f(midPoint) > 0)
            x2 = midPoint;
          else
            x1 = midPoint;
      }

      return x2 - (x2 - x1) * f(x2) / (f(x2) - f(x1));
    }
  }
}
