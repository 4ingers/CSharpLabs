using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Lab12Space {
  class FrequencyAnalysis {
    public static ConcurrentDictionary<string, int> Analyse(string path) {
      var result = new ConcurrentDictionary<string, int>();

      // Check if file actually exists
      if (!File.Exists(path)) {
        throw new ArgumentException(
          $"There is no such file at path <{path}>", nameof(path)
        );
      }

      Parallel.ForEach(File.ReadLines(path), (line) => {
        // line = Regex.Replace(line.ToLower(), @"(?<= ) |[^\w\d ]", " ");
        line = Regex.Replace(line.ToLower(), @"[^\w\d ]", " ");
        var words = line.Split(' ');

        foreach (var word in words) 
          if (!string.IsNullOrWhiteSpace(word)) 
            result.AddOrUpdate(word, 1, (key, oldValue) => oldValue + 1);
      });

      return result;
    }
  }
}