using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lab8
{
  class Program {
    static void Main(string[] args) {
      if (args.Length == 0) {
        Console.Error.WriteLine("No arguments");
        Environment.Exit(-1);
      }

      else if (args[0] == "-a" && args.Length == 3) {
        Console.WriteLine(CountStringOccurrences(args[1], args[2]));
      }
      else if (args[0] == "-b" && args.Length == 3) {
        Console.WriteLine(ReplaceSecondToLastWord(args[1], args[2]));
      }
      else if (args[0] == "-c" && args.Length == 3) {

        if (!int.TryParse(args[2], out int k)) {
          Console.Error.WriteLine("Incorrect k-value");
          Environment.Exit(-3);
        }
        
        var match = FindKCapitalLetterWord(args[1], k);
        Console.WriteLine(string.IsNullOrEmpty(match) 
          ? "Failed" : $"Succeeded: {match}");
      }
      else {
        Console.WriteLine("Not enough arguments");
        Environment.Exit(-2);
      }
    }

    static int CountStringOccurrences(string text, string pattern) {
      return Regex.Matches(text, pattern).Count;
    }

    static string ReplaceSecondToLastWord(string text, string word) {
      var tmp = text.Split(' ');
      if (tmp.Length < 2)
        return "'Less than 2'";
      tmp[^2] = word;
      //return tmp.ToString();
      return String.Join(' ', tmp);
    }

    static string FindKCapitalLetterWord(string text, int k) {
      var words = text.Split(' ');
      var filtered = (from word in words where Char.IsUpper(word.First()) select word).ToArray();
      return filtered.Length >= k ? filtered[k - 1] : "None";
    }

  }
}
