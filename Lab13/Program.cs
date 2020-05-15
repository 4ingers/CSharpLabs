using System;

namespace Lab13Space {
  class Program {
    static void Main(string[] args) {
      FileSearch fs = new FileSearch();

      if (args.Length == 1) {
        Console.WriteLine($"Search: {args[0]}");
        fs.FindAll(args[0]);
      }
      else if (args.Length == 2) {
        Console.WriteLine($"Search: {args[0]}{Environment.NewLine}Drive : {args[1]}");
        fs.FindAllAtDrive(args[0], args[1]);
      }
      else {
        Help();
        return;
      }
      Console.WriteLine();

      ///////////////////////////////////////    TIMING     /////////////////////////////////////////////
      var watch = System.Diagnostics.Stopwatch.StartNew();
      ///////////////////////////////////////////////////////////////////////////////////////////////////

      
      ///////////////////////////////////////////////////////////////////////////////////////////////////
      watch.Stop();
      Console.WriteLine($"Time elapsed: {watch.Elapsed.TotalSeconds}s{Environment.NewLine}");
      //////////////////////////////////////    /TIMING     /////////////////////////////////////////////

      fs.Print();
    }


    static public void Help() {
      Console.WriteLine(
        "-- HELP --" + Environment.NewLine +
        "Args:" + Environment.NewLine +
        "  1. File" + Environment.NewLine +
        "  2. Drive (not required) [A: or A:\\]" + Environment.NewLine
      );
    }

  }
}
