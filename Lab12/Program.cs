using System;


namespace Lab12Space {
  class Program {
    static void Main(string[] args) {
      if (args.Length == 0) {
        PrintUsageManual();
        Environment.Exit(0);
      }

      string path = args[0];
      var analyser = new FrequencyAnalyser(path);

      try {
        // If no optioal argumets passed - print the frequecy data
        if (args.Length == 1) {
          foreach (var kvp in analyser.Analyse()) {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
          }
        }

        // If top frequent are needed
        else if (args.Length == 2) {
          // Check if arguments are right, if not the greet user with manual
          if (string.Compare("top", args[1]) == 0) {
            foreach (var kvp in analyser.Top()) {
              Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
          }
          else {
            PrintUsageManual();
          }
        }

        // If search is required
        if (args.Length == 3) {
          if (string.Compare("search", args[1]) == 0) {
            var query = args[2].ToLower();
            int frequency = analyser.Search(query);
            Console.WriteLine(
              frequency == -1 ?
                "No such word in this file" :
                $"{query}: {frequency}"
            );
          }
          else {
            PrintUsageManual();
          }
        }
      }
      // Path is not valid
      catch (ArgumentException e) {
        Console.WriteLine(e.Message);
      }

    }

    private static void PrintUsageManual() {
      Console.WriteLine("<path>");
      Console.WriteLine("<path> top");
      Console.WriteLine("<path> search <query>");
    }
  }
}
