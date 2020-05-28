using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;


namespace Lab13Space {
  class FileSearch {
    readonly List<string> files;

    public List<string> FoundFiles { get => files; }
    public double TimeElapsed { get; private set; }

    public FileSearch() {
      files = new List<string>();
      TimeElapsed = 0;
    }


    public void FindAllFrom(string fileName, string startPath) {
      files.Clear();
      
      var watch = Stopwatch.StartNew();
      
      if (Directory.Exists(startPath))
        ApplyAllFiles(new DirectoryInfo(startPath), fileName, file => files.Add(file));
      
      watch.Stop();
      TimeElapsed = watch.Elapsed.TotalSeconds;
    }


    public void FindAll(string fileName) {
      files.Clear();

      object locker = new object();
      var watch = Stopwatch.StartNew();

      Parallel.ForEach(DriveInfo.GetDrives(), drive => {

        if (drive.DriveType != DriveType.Fixed && drive.DriveType != DriveType.Removable)
          return;

        List<string> filesInDrive = new List<string>();

        ApplyAllFiles(new DirectoryInfo(drive.Name), fileName, file => filesInDrive.Add(file));

        lock (locker) {
          files.AddRange(filesInDrive);
        }
      });

      watch.Stop();
      TimeElapsed = watch.Elapsed.TotalSeconds;
    }


    static void ApplyAllFiles(DirectoryInfo directory, string pattern, Action<string> fileAction) {
      try {
        foreach (FileInfo file in directory.GetFiles(pattern)) {
          fileAction(file.FullName);
        }
      }
      catch (UnauthorizedAccessException) {
        return;
      }
      foreach (DirectoryInfo deeperDirectory in directory.GetDirectories()) {
        ApplyAllFiles(deeperDirectory, pattern, fileAction);
      }
    }


    public void OpenMenu() {
      if (files.Count == 0)
        return;

      Print();

      while (true) {
        Console.Write("Type number of file you want to open (\"q\" -- to quit): ");
        var c = Console.ReadLine();
        Console.WriteLine();

        if (c == "q")
          break;

        if (int.TryParse(c, out int i) && i > 0 && i <= files.Count) {
          --i;
          string file = files[i];

          Process process = new Process();
          process.StartInfo.FileName = file;
          process.StartInfo.UseShellExecute = true;
          process.Start();
        }
      }  
    }


    public void GZipMenu() {
      if (files.Count == 0)
        return;

      Print();

      while (true) {
        Console.Write("Type number of file to compress/decompress (\"q\" -- to quit): ");
        var inputFileNum = Console.ReadLine();
        Console.WriteLine();

        if (inputFileNum == "q")
          break;

        if (int.TryParse(inputFileNum, out int i) && files.Count > 0 && i <= files.Count) {
          --i;
          string file = files[i];

          Console.WriteLine($"1 -- compress{Environment.NewLine}" +
                        $"2 -- decompress{Environment.NewLine}" +
                        $"q -- quit{Environment.NewLine}");
          Console.Write("Your choice: ");
          var actionNum = Console.ReadLine();
          Console.WriteLine();

          bool success = true;
          if (actionNum == "1") 
            success = GZip(file, CompressionMode.Compress);
          else if (actionNum == "2") 
            success = GZip(file, CompressionMode.Decompress);

          if (!success)
            Console.WriteLine("Failed");
        }
      }
    }


    static bool Compress(string sourceFile) {
      if (!File.Exists(sourceFile))
        return false;

      using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open);
      using FileStream targetStream = File.Create(sourceFile + ".zzz");

      using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);

      sourceStream.CopyTo(compressionStream);

      Console.WriteLine($"File {sourceStream.Name} compressed to {targetStream.Name}{Environment.NewLine}");

      return true;
    }


    static bool Decompress(string sourceFile) {
      if (!File.Exists(sourceFile) || !sourceFile.EndsWith(".zzz"))
        return false;

      using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open);
      string newFile = sourceFile.Remove(sourceFile.Length - 4);
      using FileStream targetStream = File.Create(newFile);

      using GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);

      decompressionStream.CopyTo(targetStream);

      Console.WriteLine($"File {sourceStream.Name} decompressed to {targetStream.Name}{Environment.NewLine}");
      return true;
    }


    static bool GZip(string sourceFile, CompressionMode compressionMode) {
      if (!File.Exists(sourceFile))
        return false;

      if (compressionMode == CompressionMode.Decompress && !sourceFile.EndsWith(".gz"))
        return false;

      using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open);

      string targetFile;
      if (compressionMode == CompressionMode.Compress)
        targetFile = sourceFile + ".gz";
      else 
        targetFile = sourceFile.Remove(sourceFile.Length - 3);

      using FileStream targetStream = new FileStream(targetFile, FileMode.Create);

      GZipStream compressionStream;

      if (compressionMode == CompressionMode.Compress) {
        compressionStream = new GZipStream(targetStream, compressionMode);
        sourceStream.CopyTo(compressionStream);
      }
      else {
        compressionStream = new GZipStream(sourceStream, compressionMode);
        compressionStream.CopyTo(targetStream);
      }

      compressionStream.Dispose();

      Console.WriteLine($"Succeded.");

      return true;
    }


    public void Print() {
      int i = 0;
      Console.WriteLine($"> Time elapsed: {TimeElapsed}s");
      string result = files.Select(file => $"  {++i}. {file}{Environment.NewLine}").Aggregate((i, j) => i + j);
      Console.WriteLine(result);
    }

  }
}