using System;

namespace Lab15Space {
  public class Logger {

    readonly object _locker = new object();

    public Logger() {}


    void Write(string msg, ConsoleColor consoleColor) {
      lock (_locker) {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine(msg);
      }
    }

    
    public void WriteLine(string msg) {
      Console.ResetColor();
      Write(msg, ConsoleColor.White);
    }


    public void WriteLineYellow(string msg) {
      Write(msg, ConsoleColor.Yellow);
    }


    public void WriteLineGreen(string msg) {
      Write(msg, ConsoleColor.Green);
    }


    public void WriteLineRed(string msg) {
      Write(msg, ConsoleColor.Red);
    }

    public void WriteLineCyan(string msg) {
      Write(msg, ConsoleColor.Cyan);
    }

  }
}
