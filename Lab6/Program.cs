using System;
using System.Linq;

namespace Lab6
{
  class Program {
    static void Main(string[] args) {

      if (args.Length == 0) {
        Console.Error.WriteLine("There're no words");
        Environment.Exit(-1);
      }

      Console.WriteLine($"Original sequence: {string.Join(' ', args)}");

      
      Console.Write("Last characters:");
      var sortedWords = (from w in args orderby w select w).ToList();
      
      sortedWords.ForEach(w => Console.Write($"{w.Last()} "));
      Console.WriteLine();
      Console.WriteLine();
      Console.WriteLine("Sorted list:");
      sortedWords.ForEach(w => Console.WriteLine(w));
    }
  }
}