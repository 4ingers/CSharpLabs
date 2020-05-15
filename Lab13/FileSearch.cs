using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;


namespace Lab13Space {
  class FileSearch {

    List<string> files;
    public List<string> FoundFiles { get => files; }


    public FileSearch() {
      files = new List<string>();
    }

    public FileSearch(string fileName) : this() {
      object locker = new object();
      Parallel.ForEach(DriveInfo.GetDrives(), drive => {
        List<string> filesInDrive = new List<string>();
        
        ApplyAllFiles(new DirectoryInfo(drive.Name), fileName, file => filesInDrive.Add(file));

        lock (locker) {
          files.AddRange(filesInDrive);
        }  
      });
    }

    public FileSearch(string fileName, string driveName) : this() {
      ApplyAllFiles(new DirectoryInfo(driveName), fileName, file => files.Add(file));
    }
    


    public void FindAllAtDrive(string fileName, string driveName) {
      files.Clear();
      ApplyAllFiles(new DirectoryInfo(driveName), fileName, file => files.Add(file));
    }


    public void FindAll(string fileName) {
      files.Clear();

      object locker = new object();
      Parallel.ForEach(DriveInfo.GetDrives(), drive => {
        List<string> filesInDrive = new List<string>();

        ApplyAllFiles(new DirectoryInfo(drive.Name), fileName, file => filesInDrive.Add(file));

        lock (locker) {
          files.AddRange(filesInDrive);
        }
      });
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

        if (c == "q")
          break;

        if (int.TryParse(c, out int i) && i > 0 && i <= files.Count) {
          --i;
          string file = files[i];

          Process proc = new Process();
          proc.StartInfo.FileName = file;
          proc.StartInfo.UseShellExecute = true;
          proc.Start();            
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

        if (inputFileNum == "q")
          break;

        if (int.TryParse(inputFileNum, out int fileNum) && files.Count > 0 && fileNum <= files.Count) {
          --fileNum;
          string file = files[fileNum];

          Console.Write($"1 -- compress{Environment.NewLine}" +
                      $"2 -- decompress{Environment.NewLine}" +
                      $"q -- quit{Environment.NewLine}" +
                      $"Your choice: ");
          var actionNum = Console.ReadLine();

          if (actionNum == "1") {
            Compress(file, file+ ".zzz");
          }
          else if (actionNum == "2" && file.EndsWith(".zzz")) {
            Decompress(file, file.SkipLast(4).ToString());
          }       
        }
      }
    }


    public static bool Compress(string sourceFile, string targetFile) {
      if (!File.Exists(sourceFile) || sourceFile == targetFile)
        return false;

      using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open);
      using FileStream targetStream = File.Create(targetFile);

      using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);

      sourceStream.CopyTo(compressionStream);

      Console.WriteLine($"File {sourceStream.Name} compressed to {targetStream.Name}.");

      return true;
    }


    public static bool Decompress(string sourceFile, string targetFile) {
      if (!File.Exists(sourceFile) || sourceFile == targetFile)
        return false;

      using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open);
      using FileStream targetStream = File.Create(targetFile);

      using GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);

      decompressionStream.CopyTo(targetStream);

      Console.WriteLine($"File {sourceStream.Name} decompressed to {targetStream.Name}");
      return true;
    }


    public void Print() {
      int i = 0;
      foreach (string file in files) 
        Console.WriteLine($"{++i}. {file}");
      Console.WriteLine();
    }
  }
}