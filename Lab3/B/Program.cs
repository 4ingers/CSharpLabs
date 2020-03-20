using System;
using System.Linq;

namespace Lab3B
{
  class Program {
    static void Main(string[] args) {
      string input = GetInput();

      var product = GAvg(input);
      
      Console.WriteLine($"Result: {product}");
    }

    static string GetInput() {
      Console.WriteLine("Type doubles dividing them by space " +
        "(empties will be ignored) :");
      string input = Console.ReadLine();

      if (input.Length == 0) {
        Console.WriteLine("No input");
        Environment.Exit(-1);
      }

      return input;
    }

    static double GAvg(string input) {
      var strings = input.Trim().Split(' ').ToList();
      var numbers = (from str in strings select ParseString(str)).ToArray();
      var product = numbers.Aggregate(1, (double p, double d) => p * d);
      var result = Math.Pow(product, 1.0 / numbers.Length);
      return result;
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

  }
}
