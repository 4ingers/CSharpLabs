using System;

namespace _9
{
  class Program {
    public delegate double Solver(double x);
    static void Main(string[] args) {
      double x1 = Bisection(Equation, -2, -1, Constants.Epsilon);
      double x2 = Bisection(Equation, 3, 4, Constants.Epsilon);
      Console.WriteLine(x1);
      Console.WriteLine(x2);
    }

    static double Equation(double x) {
      return Math.Pow(x, 4) + Math.Pow(x, 3) - 6 * Math.Pow(x,2) - 20 * x - 16;
    }

    static double Bisection(Solver f, double a, double b, double eps) {
      var x1 = a;
      var x2 = b;
      var f_b = f(b);
      
      while (Math.Abs(x2 - x1) >= eps) {
          var x3 = (x1 + x2) / 2.0;
          if (f_b * f(x3) > 0)
            x2 = x3;
          else
            x1 = x3;
      }
      return x2 - (x2 - x1) * f(x2) / (f(x2) - f(x1));
    }
  }
}
