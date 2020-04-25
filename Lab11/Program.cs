using System;
using System.Linq;

namespace Lab11Space {
  class Program {
    static void Main(string[] args) {
      Complex c1 = -8;
      var roots = Complex.NthRoots(c1, 3);
      foreach (var value in roots) {
        Console.WriteLine(value.ToString());
      }

      // Testing event
      Complex divident = 7;
      Complex.DivisionByZero += c_DivisionByZero;
      Console.WriteLine(divident / 0);
      Vector<Complex> vc = new Vector<Complex>(new Complex[] {
        3, 4, Complex.ImaginaryOne
      });
      Console.WriteLine(vc.ToString());
      Console.WriteLine(Vector<Complex>.DotProduct(vc, vc).ToString());
      Console.WriteLine(vc.GetNorm().ToString("F"));

      Vector<Complex> i = new Vector<Complex>(new Complex[] {
        1, 0, 0
      });
      Vector<Complex> j = new Vector<Complex>(new Complex[] {
        0, 1, 0
      });
      Console.WriteLine(Vector<Complex>.CrossProduct(i, j).ToString());
    }

    static void c_DivisionByZero(object sender, DivisionByZeroEventArgs e) {
      Console.WriteLine($"Division by {e.Divisor} == 0");
      Environment.Exit(-1);
    }
  }
}
