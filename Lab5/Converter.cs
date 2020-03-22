using System;
using System.Text;
using System.Collections.Generic;

namespace Lab5 {
  class Converter {

    public static string FormatFloating(Decimal num, int radix, int precision = 8) {
      bool minus = false;

      if (num < 0) {
        minus = true;
        num *= -1;
      }
      int integer = (int)Math.Truncate(num);
      decimal dec = num - integer;

      StringBuilder converted = new StringBuilder(
        FormatInteger(integer, radix));

      if (dec > (decimal)Constants.Eps) {
        string decPart = FormatDecimal(dec, radix, precision);
        converted.Append(decPart);
      }
      return (minus ? "-" : "") + converted.ToString();
    }

    public static string FormatInteger(int num, int radix) {
      if (num == 0)
        return "0";

      StringBuilder sb = new StringBuilder();

      while (num != 0) {
        int mod = num % (int)radix;
        var symbol = GetSymbol(mod);
        sb.Insert(0, symbol);
        num /= (int)radix;
      }
      return sb.ToString();
    }


    public static string FormatDecimal(decimal num, int radix,
      int precision) {

      List<decimal> decs = new List<decimal>();

      StringBuilder sb = new StringBuilder(".");

      while (precision-- != 0 && num > (decimal)Constants.Eps) {

        if (decs.Contains(num))
          return ReplaceByPeriod(num, decs, sb.ToString());
        decs.Add(num);

        num *= radix;
        int trunc = (int)Math.Truncate(num);
        num -= Math.Truncate(num);

        var symbol = GetSymbol(trunc);

        sb.Append(symbol);
      }
      var result = sb.ToString();
      return result;
    }

    public static string ReplaceByPeriod(decimal num,
                                      List<decimal> decs,
                                      string str) {
      int index = decs.IndexOf(num);
      string substring = str.Substring(index + 1);
      return str.Replace(substring, $"({substring})");
    }

    public static string GetSymbol(int num) {
      if (num <= 9)
        return ((char)(num + '0')).ToString();
      else if (num <= 36)
        return ((char)(num + 'A' - 10)).ToString();
      else
        return $"<{num}>";
    }
  }

}
