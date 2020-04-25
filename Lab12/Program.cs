using System;
using System.IO;


namespace Lab12Space {
  class Program {
    static void Main(string[] args) {

      if (args.Length == 0) {
        Help();
        Environment.Exit(0);
      }

      string path = args[0];

      if (!File.Exists(path)) {
        Console.Error.WriteLine($"File at {path} not found");
        Environment.Exit(-1);
      }

      var handler = new MatchesHandler(path);

      // --- All the words ---
      if (args.Length == 1) 
        Console.WriteLine(handler.All());

      // --- Max ---
      else if (args.Length == 2) {
        if (args[1].Equals("max")) 
          Console.WriteLine(handler.Top());
        else
          Help();
      }

      // --- Find ---
      else if (args.Length == 3) {
        if (args[1].Equals("search")) {
          var token = args[2].ToLower();
          Console.WriteLine(handler.Find(token));
        }
        else 
          Help();
      }
    }

    private static void Help() {
      Console.WriteLine(
        "*path*\n" +
        "*path* max\n" +
        "*path* search *token*\n"
      );
    }
  }
}
