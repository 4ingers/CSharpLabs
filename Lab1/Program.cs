using System;
using System.Numerics;
using System.Text;
using System.Linq;

namespace Lab1 {
  class Program {
    static void Main(string[] args) {

      if (args.Length < 4) {
        Console.Error.WriteLine("Program requires 4 parameters " +
          "but you've typed less");
        Environment.Exit(-1);
      }

      var numbers = (from str in args select ParseString(str)).ToArray();
      double a = numbers[0], 
        b = numbers[1], 
        c = numbers[2], 
        d = numbers[3];

      if (!int.TryParse(args[4], out int precision) || precision < 0)
        precision = 4;

      Console.WriteLine($">> Precision: {precision}\n");

      PrintEquation(a, b, c, d);

      int cRootsGot = Cardano(a, b, c, d, out (Complex, Complex, Complex) cRoots);
      PrintRoots(cRoots, cRootsGot, "Cardano", precision);

      int vRootsGot = Vieta(a, b, c, d, out (Complex, Complex, Complex) vRoots);
      PrintRoots(vRoots, vRootsGot, "Vieta", precision);
    }


    static double ParseString(string str) {
      str = str.Replace(",", ".");
      if (!Double.TryParse(str, out double result))
        SymbolNotAllowed(str);
      return result;
    }

    static void SymbolNotAllowed(string str) {
      Console.Error.WriteLine($"Symbol not allowed: {str}");
      Environment.Exit(-2);
    }


    static int Cardano(double a, double b, double c, double d, out (Complex, Complex, Complex) roots) {

      Complex x1 = new Complex();
      Complex x2 = new Complex();
      Complex x3 = new Complex();

      if (a < Double.Epsilon && b < Double.Epsilon && c < Double.Epsilon) {
        roots = (x1, x2, x3);
        return 0;
      }
      if (a < Double.Epsilon && b < Double.Epsilon) {
        x1 = -d / c;
        roots = (x1, x2, x3);
        return 1;
      }
      else if (a == 0) {
        double D = c * c - 4 * b * d;
        x1 = (-c - Complex.Sqrt(D)) / (2 * b);
        x2 = (-c + Complex.Sqrt(D)) / (2 * b);
        roots = (x1, x2, x3);
        return 2;
      }

      Double f = ((3 * c / a) - ((b * b) / (a * a))) / 3,
        g = (((2 * Math.Pow(b, 3)) 
          / (Math.Pow(a, 3))) - ((9 * b * c) / (a * a)) + (27 * d / a)) / 27,
        h = (g * g) / 4 + Math.Pow(f, 3) / 27;

      if (Math.Abs(f) < double.Epsilon && Math.Abs(g) < double.Epsilon
        && Math.Abs(h) < double.Epsilon) {
        x1 = x2 = x3 = (d / a) >= 0 ? -Math.Cbrt(d / a) : Math.Cbrt(-d / a);
      }
      else if (h <= 0) {
        double i = Math.Sqrt((g * g / 4) - h),
          j = Math.Cbrt(i),
          k = Math.Acos(-g / (2 * i)),
          M = Math.Cos(k / 3),
          N = Math.Sqrt(3) * Math.Sin(k / 3),
          P = -b / (3 * a);

        x1 = 2 * j * Math.Cos(k / 3) - (b / (3 * a));
        x2 = -j * (M + N) + P;
        x3 = -j * (M - N) + P;
      }
      else if (h > 0) {
        double R = -g / 2 + Math.Sqrt(h);
        double S = R >= 0 ? Math.Cbrt(R) : -Math.Cbrt(-R);
        double T = -g / 2 - Math.Sqrt(h);
        double U = T >= 0 ? Math.Cbrt(T) : -Math.Cbrt(-T);

        x1 = (S + U) - (b / (3 * a));
        x2 = -(S + U) / 2 - (b / (3 * a))
          - (S - U) * Math.Sqrt(3) * 0.5 * Complex.ImaginaryOne;
        x3 = -(S + U) / 2 - (b / (3 * a)) 
          + (S - U) * Math.Sqrt(3) * 0.5 * new Complex(0, 1);
      }
      roots = (x1, x2, x3);
      return 3;
    }


    static int Vieta(double a, double b, double c, double d, out (Complex, Complex, Complex) roots) {

      Complex x1 = new Complex();
      Complex x2 = new Complex();
      Complex x3 = new Complex();

      if (a < Double.Epsilon && b < Double.Epsilon && c < Double.Epsilon) {
        roots = (x1, x2, x3);
        return 0;
      }
      if (a < double.Epsilon && b < Double.Epsilon) {
        x1 = -d / c;
        roots = (x1, x2, x3);
        return 1;
      }
      else if (a == 0) {
        Double D = c * c - 4 * b * d;
        x1 = (-c - Complex.Sqrt(D)) / (2 * b);
        x2 = (-c + Complex.Sqrt(D)) / (2 * b);
        roots = (x1, x2, x3);
        return 2;
      }

      double a1 = b / a,
        b1 = c / a,
        c1 = d / a;

      Complex q = (3 * b1 - a1 * a1) / 9,
        r = (9 * a1 * b1 - 27 * c1 - 2 * Complex.Pow(a1, 3)) / 54,
        s = Complex.Pow(r + Complex.Sqrt(Complex.Pow(q, 3) + r * r), 
          1.0 / 3.0),
        t = Complex.Pow(r - Complex.Sqrt(Complex.Pow(q, 3) + r * r), 
          1.0 / 3.0);

      x1 = s + t - a1 / 3;
      x2 = -0.5 * (s + t) - (1.0 / 3.0) * a1 
        + 0.5 * Math.Sqrt(3) * (s - t) * new Complex(0, 1);
      x3 = -0.5 * (s + t) - (1.0 / 3.0) * a1 
        - 0.5 * Math.Sqrt(3) * (s - t) * new Complex(0, 1);

      roots = (x1, x2, x3);
      return 3;
    }


    static void PrintEquation(double a, double b, double c, double d) {
      if (!(a == 0 && b == 0 && c == 0 && d == 0)) {

        StringBuilder sb = new StringBuilder();
        if (a != 0)
          sb.Append($"{a}x3 ");
        if (b != 0)
          sb.Append($"{(b >= 0 ? "+ " : "- ")}{Math.Abs(b)}x2 ");
        if (c != 0)
          sb.Append($"{(c >= 0 ? "+ " : "- ")}{Math.Abs(c)}x ");
        if (d != 0)
          sb.Append($"{(d >= 0 ? "+ " : "- ")}{Math.Abs(d)} ");
        sb.Append("= 0\n");

        if (sb.ToString().StartsWith('+'))
          sb.Remove(0, 1);

        Console.WriteLine($">> Input: {sb}");
      }
    }

    static void PrintRoots((Complex, Complex, Complex) roots, int rootsNum, String methodName, int precision) {
      Console.WriteLine(">> " + methodName.ToUpper());
      if (rootsNum == 0) {
        Console.WriteLine("There aren't roots..\n");
      }
      else {
        String format = "{0:I" + precision + "}";
        Console.WriteLine("\tx1 = "
          + String.Format(new ComplexFormatter(), format, roots.Item1));
        if (rootsNum > 1)
          Console.WriteLine("\tx2 = "
            + String.Format(new ComplexFormatter(), format, roots.Item2));
        if (rootsNum > 2)
          Console.WriteLine("\tx3 = "
            + String.Format(new ComplexFormatter(), format, roots.Item3));
      }
      Console.WriteLine();
    }

  }
}