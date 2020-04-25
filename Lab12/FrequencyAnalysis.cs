using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;


namespace Lab12Space {
  class FrequencyAnalyser { 
    private readonly Dictionary<string, int> dict;
    private readonly string path;

    public FrequencyAnalyser(string path) {
      dict = new Dictionary<string, int>();
      this.path = path;
    }


    public void Initialize() {
      if (!File.Exists(path))
        throw new ArgumentException($"There is no such file at path <{path}>");

      foreach (var line in File.ReadLines(path)) {
        var simplified = Regex.Replace(line.ToLower(), @"[^\w\d ]", " ");
        var words = simplified.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words) {
          if (!dict.TryAdd(word, 1))
            dict[word]++;
        }
      }
    }


    public IEnumerable<KeyValuePair<string, int>> Analyse() {
      if (dict.Count() == 0)
        Initialize();
      return dict.OrderByDescending(item => item.Value);
    }

    public IEnumerable<KeyValuePair<string, int>> Max() {
      if (dict.Count() == 0)
        Initialize();
      var sorted = dict.OrderByDescending(item => item.Value);
      var max = sorted.First().Value;
      return sorted.Where(item => item.Value == max);
    }


    public int Search(string token) {
      if (dict.Count() == 0)
        Initialize();
      return dict.ContainsKey(token) ? dict[token] : -1;
    }
  }
}