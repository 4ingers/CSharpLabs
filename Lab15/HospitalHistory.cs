using System;

namespace Lab15Space {
  public static class HospitalHistory {

    public static void Write(string msg, ConsoleColor consoleColor) {
      Console.ForegroundColor = consoleColor;
      Console.Write(msg);
      Console.ResetColor();
    }
    public static void Writeln(string msg, ConsoleColor consoleColor) {
      Console.ForegroundColor = consoleColor;
      Console.WriteLine(msg);
      Console.ResetColor();
    }

    public static void WriteWhite(string msg) => Write(msg, ConsoleColor.White);
    public static void WritelnWhite(string msg) => Writeln(msg, ConsoleColor.White);

    public static void WriteYellow(string msg) => Write(msg, ConsoleColor.Yellow);
    public static void WritelnYellow(string msg) => Writeln(msg, ConsoleColor.Yellow);

    public static void WriteGreen(string msg) => Write(msg, ConsoleColor.Green);
    public static void WritelnGreen(string msg) => Writeln(msg, ConsoleColor.Green);

    public static void WriteRed(string msg) => Write(msg, ConsoleColor.Red);
    public static void WritelnRed(string msg) => Writeln(msg, ConsoleColor.Red);

    public static void WriteCyan(string msg) => Write(msg, ConsoleColor.Cyan);
    public static void WritelnCyan(string msg) => Writeln(msg, ConsoleColor.Cyan);

  }
}
