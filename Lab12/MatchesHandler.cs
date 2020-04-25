using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace Lab12Space {
  class MatchesHandler { 
    private readonly Dictionary<string, int> dict;
    private readonly string path;

    public MatchesHandler(string path) {
      dict = new Dictionary<string, int>();
      this.path = path;
    }


    public void Initialize() {
      foreach (var line in File.ReadLines(path)) {
        var simplified = Regex.Replace(line.ToLower(), @"[^\w\d ]", " ");
        var words = simplified.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words) {
          if (!dict.TryAdd(word, 1))
            dict[word]++;
        }
      }
    }


    public string All() {
      if (dict.Count() == 0)
        Initialize();

      StringBuilder sb = new StringBuilder();

      foreach (var item in dict.OrderByDescending(item => item.Value))
        sb.Append($"{item.Key} : {item.Value}\n");

      return sb.ToString();
    }

    public string Top() {
      if (dict.Count() == 0)
        Initialize();
      var sorted = dict.OrderByDescending(item => item.Value);
      var max = sorted.First().Value;

      StringBuilder sb = new StringBuilder();

      foreach (var item in sorted.Where(item => item.Value == max))
        sb.Append($"{item.Key} : {item.Value}\n");

      return sb.ToString();
    }


    public string Find(string token) {
      if (dict.Count() == 0)
        Initialize();
      return $"{token} : {(dict.ContainsKey(token) ? dict[token] : 0)}";
    }
  }
}