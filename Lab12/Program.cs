using System;
using System.Linq;


namespace Lab12Space {
  class Program {
    static void Main(string[] args) {
      // Check for command line arguments
      if (args.Length == 0) {
        // Greet user with short usage manual
        Console.WriteLine("[Usage] <path> <word>");
        Environment.Exit(0);
      }

      string path = args[0];

      try {
        var result = FrequencyAnalysis.Analyse(path);
        // If search is required
        if (args.Length == 2) {
          var key = args[1].ToLower();
          Console.WriteLine(
            result.ContainsKey(key) ? $"{key}: {result[key]}" :
            "No such word in this file"
          );
        }
        else {
          foreach (var kvp in result.OrderByDescending(key => key.Value)) {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
          }
        }
      }
      // Path is not valid
      catch (ArgumentException e) {
        Console.WriteLine(e.Message);
      }

    }
  }
}
