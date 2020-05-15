using System;
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
      if (Directory.Exists(driveName))
        ApplyAllFiles(new DirectoryInfo(driveName), fileName, file => files.Add(file));
    }
    


    public void FindAllAtDrive(string fileName, string driveName) {
      files.Clear();
      if (Directory.Exists(driveName))
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
        Console.WriteLine();

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
        Console.WriteLine();

        if (inputFileNum == "q")
          break;

        if (int.TryParse(inputFileNum, out int fileNum) && files.Count > 0 && fileNum <= files.Count) {
          --fileNum;
          string file = files[fileNum];

          Console.WriteLine($"1 -- compress{Environment.NewLine}" +
                        $"2 -- decompress{Environment.NewLine}" +
                        $"q -- quit{Environment.NewLine}");
          Console.Write("Your choice: ");
          var actionNum = Console.ReadLine();
          Console.WriteLine();

          bool success = true;
          if (actionNum == "1") 
            success = Compress(file);
          else if (actionNum == "2") 
            success = Decompress(file);

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


    public void Print() {
      int i = 0;
      foreach (string file in files) 
        Console.WriteLine($"{++i}. {file}");
      Console.WriteLine();
    }

  }
}