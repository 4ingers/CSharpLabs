using System;

namespace Lab13Space {
  class Program {
    static void Main(string[] args) {
      FileSearch fileSearch = new FileSearch();

      if (args.Length == 0) {
        Help();
        Environment.Exit(0);
      }

      Console.WriteLine($"Search: {args[0]}{Environment.NewLine}");

      if (args.Length == 1) {
        fileSearch.FindAll(args[0]);
        fileSearch.Print();
        Environment.Exit(0);
      }

      else if (args.Length == 2) {
        if (args[1] == "-o") {
          fileSearch.FindAll(args[0]);
          fileSearch.OpenMenu();
          Environment.Exit(0);
        }
        else if (args[1] == "-z") {
          fileSearch.FindAll(args[0]);
          fileSearch.GZipMenu();
          Environment.Exit(0);
        }
        else {
          Console.WriteLine($"Start path : {args[1]}{Environment.NewLine}");
          fileSearch.FindAllFrom(args[0], args[1]);
          fileSearch.Print();
          Environment.Exit(0);
        }
      }

      else if (args.Length == 3) {
        Console.WriteLine($"Start path : {args[1]}{Environment.NewLine}");

        if (args[2] == "-o") {
          fileSearch.FindAllFrom(args[0], args[1]);
          fileSearch.OpenMenu();
          Environment.Exit(0);
        }
        else if (args[2] == "-z") {
          fileSearch.FindAllFrom(args[0], args[1]);
          fileSearch.GZipMenu();
          Environment.Exit(0);
        }
      }
      
      Help();
      Environment.Exit(-1);
    }


    static public void Help() {
      Console.WriteLine(
        "-- HELP --" + Environment.NewLine +
        "Args:" + Environment.NewLine +
        "  1. File" + Environment.NewLine +
        "  2. Start path (not required)" + Environment.NewLine
      );
    }



    /////////////////////////////////////////    TIMING     /////////////////////////////////////////////
    //var watch = System.Diagnostics.Stopwatch.StartNew();
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    //watch.Stop();
    //Console.WriteLine($"Time elapsed: {watch.Elapsed.TotalSeconds}s{Environment.NewLine}");
    ////////////////////////////////////////    /TIMING     /////////////////////////////////////////////
  }
}
