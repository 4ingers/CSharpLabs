using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Lab12Space {
  class FrequencyAnalyser {
    // Fields
    private readonly string path;
    private ConcurrentDictionary<string, int> dict;

    public FrequencyAnalyser(string path) {
      this.path = path;
      dict = new ConcurrentDictionary<string, int>();
    }

    // Get all frequency data
    public List<KeyValuePair<string, int>> Analyse() {
      AnalyseData();
      return dict.OrderByDescending(key => key.Value).ToList();
    }

    // Get list of most frequent words
    public List<KeyValuePair<string, int>> Top() {
      if (dict.IsEmpty)
        AnalyseData();

      var tmp = dict.OrderByDescending(key => key.Value);
      return tmp.Where((kvp) => kvp.Value == tmp.First().Value).ToList();
    }

    // Check if query exists in file. If so - returns frequency for that query,
    // If not - returns -1
    public int Search(string query) {
      AnalyseData();
      return dict.ContainsKey(query) ? dict[query] : -1;
    }

    private void AnalyseData() {
      // Check if file actually exists
      if (!File.Exists(path)) {
        throw new ArgumentException(
          $"There is no such file at path <{path}>", nameof(path)
        );
      }

      foreach (var line in File.ReadLines(path)) {
        // Matches all single characters except those in brackets
        //   \w   --   words
        //   \d   --   digits
        //        --   actual space character
        var simplified = Regex.Replace(line.ToLower(), @"[^\w\d ]", " ");
        var words = simplified.Split(' ');

        foreach (var word in words) {
          if (!string.IsNullOrWhiteSpace(word)) {
            // If key is present in the dictionary - updates its value,
            // if not - adds one with value 1
            dict.AddOrUpdate(word, 1, (key, oldValue) => oldValue + 1);
          }
        }
      }
    }
  }
}