using System;
using System.IO;


namespace Lab14Space {

  class Program {

    static void Main(string[] args) {

      if (args.Length == 0 || args.Length == 1) {
        Console.WriteLine("Usage: <path> <path>");
        Environment.Exit(0);
      }

      using var input = new StreamReader(args[0]);
      using var output = new StreamWriter(args[1]);

      while (!input.EndOfStream) {
        var line = input.ReadLine();
        var calc = new PerfectFormCalculator(line);

        try { 
          calc.Process();
        }
        catch (Exception e) when (e is NotSupportedException || e is FormatException) {
          Console.WriteLine($"{e.Message}{Environment.NewLine}" +
            $"Program will now continue from the next formula{Environment.NewLine}");
          continue;
        }

        Console.WriteLine(calc.TruthTable);

        output.WriteLine($"Формула: {line}");
        output.WriteLine($"СДНФ: {calc.PDNF}");
        output.WriteLine($"СКНФ: {calc.PCNF}{Environment.NewLine}{Environment.NewLine}");
      }
    }
  }
}
