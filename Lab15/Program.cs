using System;
using System.Threading;

namespace Lab15Space {
  class Program {
    static void Main(string[] args) {

      int receptionCapacity,
          doctorsCount,
          sessionDuration;

      Hospital hospital = null;

      if (args.Length == 0) {
        hospital = new Hospital();
      }
      else if (args.Length > 2 &&
          int.TryParse(args[0], out receptionCapacity) &&
          int.TryParse(args[1], out doctorsCount) &&
          int.TryParse(args[2], out sessionDuration) &&
          receptionCapacity >= 0 && doctorsCount >= 0 && sessionDuration >= 0){

        hospital = new Hospital(receptionCapacity,doctorsCount,sessionDuration);
      }
      else {
        Help();
        Environment.Exit(-1);
      }

      hospital.Work();

    }

    static void Help() {
      Console.WriteLine(
          $"Входные параметры:{Environment.NewLine}" +
          $"\t1. Вместимость приёмной >= 0{Environment.NewLine}" +
          $"\t2. Количество докторов >= 0{Environment.NewLine}" +
          $"\t3. Максимальная длительность осмотра >= 1{Environment.NewLine}");
    }

  }
}
