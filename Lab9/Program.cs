using System;

namespace Space9 {
  class Program {
    static void Main(string[] args) {
      double x1 = MathLab.Bisection(MathLab.F, MathLab.X1range, Constants.Epsilon);
      double x2 = MathLab.Bisection(MathLab.F, MathLab.X2range, Constants.Epsilon);
      Console.WriteLine(x1);
      Console.WriteLine(x2);
    }
  }
}
