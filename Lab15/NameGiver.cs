using System;
using System.IO;
using System.Collections;
using System.Text;

namespace Lab15Space {
  class NameGetter {
    static ArrayList names;

    static NameGetter() {

      string fileName = "names.txt";
      if (!File.Exists(fileName))
        throw new FileNotFoundException("Cannot load names initializer");

      names = new ArrayList();

      using (var fileStream = File.OpenRead(fileName)) {
        using var streamReader = new StreamReader(fileName);
        string line;
        while ((line = streamReader.ReadLine()) != null) {
          names.Add(line);
        }
      }
    }

    static public string Get() {
      return names[new Random().Next(names.Count - 1)] as string;
    }
  }
}
