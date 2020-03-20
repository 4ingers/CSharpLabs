using System;
using System.Text;

namespace Lab4
{
  class Program {
    static void Main(string[] args) {
      HandleInput(args);

      int number = 0, radix = 0;
      if (!(int.TryParse(args[0], out number)
        && (int.TryParse(args[1], out radix)))) {
        Console.Error.WriteLine("Incorrect argument");
        Environment.Exit(-1);
      }

      var result = ConvertDecimalToAny(number, radix);

      Console.WriteLine(result == null ? 
         "Radix must be in [2; 36]" : $"Result: {result}");
    }


    static void HandleInput(string[] input) {
      if (input.Length < 2) {
        Console.Error.WriteLine("No arguments");
        Environment.Exit(-1);
      }
    }


    static string ConvertDecimalToAny(int num, int radix) {
      const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

      if (radix < 2 || radix > digits.Length)
        return null;
      
      StringBuilder sb = new StringBuilder();

      bool minus = num < 0;
      num = Math.Abs(num);

      while (num != 0) {
        sb.Insert(0, digits[num % radix]);
        num /= radix;
      }

      if (minus)
        sb.Insert(0, '-');

      return sb.ToString();
    }

  }
}
