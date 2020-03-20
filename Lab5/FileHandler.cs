using System;
using System.IO;

namespace Lab5 {
  class FileHandler {
    public static string[] ReadWords(string path) {
      if (!File.Exists(path))
        throw new FileNotFoundException();

      using StreamReader sr = File.OpenText(path);
      string s = sr.ReadLine();
      var data = s.Trim().Split(' ');
      string[] result = new string[] {
        (string) data[0].Clone(),
        (string) data[1].Clone(),
        (data.Length == 3 ? data[2] : null)
      };
      return result;
    }
  }
}
