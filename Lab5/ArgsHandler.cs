using System;
using System.IO;


namespace Lab5 {
  class ArgsHandler {
    public static string[] GetData(string[] args) {
      string[] result;

      if (args.Length == 0) {
        GetHelp();
        Environment.Exit(0);
      }

      if (args[^1] == "-h") 
        GetHelp();

      if (args[0] == "-f") {

        if (args.Length < 2) {
          throw new ArgumentException("No filename");
        }
        string path = $"../../../{args[1]}";

        try { 
          result = FileHandler.ReadWords(path);
        }
        catch (FileNotFoundException e) {
          throw;
        }
      }
      else {
        if (args.Length < 3) {
          throw new ArgumentException("Incorrect input");
        }
        result = new string[] {
          (string)args[0].Clone(),
          (string)args[1].Clone(),
          (args.Length == 3 ? args[2] : null)
        };
      }
      return result;
    }

    public static void GetHelp() {
      string help = 
        "=== Floating numbers converter ===\n" +
        "PARAMS:\n\t1. Number\n\t2. Base\n\t3. [Precision (default: 4)]\n" +
        "OR\n\t1. -f\n\t2. Filename";
      Console.WriteLine(help);
    }

  }
}
