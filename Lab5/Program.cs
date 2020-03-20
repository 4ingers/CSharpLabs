using System;
using System.IO;

namespace Lab5 {
  class Program {
    static void Main(string[] args) {
      decimal num = 0;
      int radix = 0;
      int precision = 0;

      try {
        var data = TaskHandler.GetValuesFromArguments(args);
        num = data.Item1;
        radix = data.Item2;
        precision = data.Item3;
      }
      catch (ArgumentException e) {
        Console.Error.WriteLine(e.Message);
        Environment.Exit(-1);
      }
      catch (FileNotFoundException) {
        Console.Error.WriteLine("No such file");
      }
      string result = Converter.FormatFloating(num, radix, precision);
      Console.WriteLine($"{num}[10] = {result}[{radix}]");
    }  

  }
}
