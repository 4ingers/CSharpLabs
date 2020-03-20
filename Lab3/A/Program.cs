using System;
using System.Linq;

namespace Lab3A
{
  class Program {
    static void Main(string[] args) {

      string input = GetSymbols();

      var average = CharsAvg(input);

      Console.WriteLine(average == double.NaN ? 
        "Failed" : $"Result: {average}");
    }

    static string GetSymbols() {
      Console.WriteLine("Type symbols dividing them by space " +
        "(non 1-symbols consequences will be ignored) :");
      string input = Console.ReadLine();

      if (input.Length == 0) {
        Console.Error.WriteLine("No input");
        Environment.Exit(-1);
      }

      return input;
    }

    static double CharsAvg(string inputChars) {
      var chars = inputChars.Trim().Split(' ').ToList();
      chars.RemoveAll(ch => ch.Length != 1);

      var codes = (from i in chars select (int)Convert.ToChar(i)).ToArray();

      if (codes.Length == 0) {
        return double.NaN;
      }
      var average = (double)codes.Sum() / codes.Length;

      return average;
    }
  }
}
