using System;
namespace Lab2 {
  class Program {
    static void Main() {
      Console.WriteLine($"PI:           {Calculation.PI()}");
      Console.WriteLine($"Math.PI:      {Math.PI}");
      Console.WriteLine();
      Console.WriteLine($"E:            {Calculation.E()}");
      Console.WriteLine($"Math.E:       {Math.E}");
      Console.WriteLine();
      Console.WriteLine($"Log2:         {Calculation.Log2()}");
      Console.WriteLine($"Math.Log(2):  {Math.Log(2)}");
    }
  }
}
