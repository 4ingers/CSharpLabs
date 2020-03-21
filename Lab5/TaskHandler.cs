using System;

namespace Lab5 {
  class TaskHandler {
    public static (decimal, int, int) GetValuesFromArguments(string[] args) {
      string[] toConvert;
      try {
        toConvert = ArgsHandler.GetData(args);
      }
      catch {
        throw;
      }
      string numberString = toConvert[0];
      string baseString = toConvert[1];


      if (!decimal.TryParse(numberString.Replace(',', '.'), out decimal num))
        throw new ArgumentException("floating conversion failed");
      if (!int.TryParse(baseString, out int radix))
        throw new ArgumentException("base conversion failed");

      int precision = Constants.Precision;

      if (toConvert.Length == 3) {
        string precisionString = toConvert[2];
        if (!int.TryParse(precisionString, out precision) || (precision < 1)
          || precision > 128)
          precision = Constants.Precision;
      }
      return (num, radix, precision);
    }

  }
}
