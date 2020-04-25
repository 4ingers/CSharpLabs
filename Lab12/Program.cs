using System;


namespace Lab12Space {
  class Program {
    static void Main(string[] args) {
      if (args.Length == 0) {
        PrintUsageManual();
        Environment.Exit(0);
      }

      string path = args[0];

      try {
        var analyser = new FrequencyAnalyser(path);

        if (args.Length == 1) {
          foreach (var item in analyser.Analyse())
            Console.WriteLine($"{item.Key} : {item.Value}");
        }
        // If max counts are needed
        else if (args.Length == 2) {
          if (args[1].Equals("max")) {
            foreach (var kvp in analyser.Max())
              Console.WriteLine($"{kvp.Key} : {kvp.Value}");
          }
          else {
            PrintUsageManual();
          }
        }
        // If search is required
        else if (args.Length == 3) {
          if (args[1].Equals("search")) {
            var token = args[2].ToLower();
            int count = analyser.Search(token);
            Console.WriteLine(count == -1 ? "No such word in this file" : $"{token} : {count}");
          }
          else {
            PrintUsageManual();
          }
        }
      }
      // Invalid path
      catch (ArgumentException) {
        Console.Error.WriteLine("Invalid path");
      }
    }

    private static void PrintUsageManual() {
      Console.WriteLine(
        "*path*\n" +
        "*path* max\n" +
        "*path* search *token*\n"
      );
    }
  }
}
