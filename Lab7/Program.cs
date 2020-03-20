using System;
using System.Linq;

namespace Lab7
{
  class Program {
    static void Main(string[] args) {
      if (args.Length == 0) 
        Environment.Exit(-1);
      var str = String.Concat(args[0].OrderBy(c => c));
      foreach (var item in str) {
        Console.WriteLine($"{item} {(int)Convert.ToChar(item)}");
      }
    }
  }
}