using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;


namespace Lab13Space {
  class FileSearch {

    ArrayList files;
    public ArrayList FoundFiles { get => (ArrayList)files.Clone(); }


    public FileSearch() {
      files = new ArrayList();
    }

    public FileSearch(string fileName) : this() {
      Parallel.ForEach(DriveInfo.GetDrives(), drive => {
        ApplyAllFiles(new DirectoryInfo(drive.Name), fileName, file => files.Add(file));
      });
    }

    public FileSearch(string fileName, string driveName) : this() {
      files = new ArrayList();
      ApplyAllFiles(new DirectoryInfo(driveName), fileName, file => files.Add(file));
    }
    


    public void FindAllAtDrive(string fileName, string driveName) {
      ApplyAllFiles(new DirectoryInfo(driveName), fileName, file => files.Add(file));
    }

    public void FindAll(string fileName) {
      files.Clear();
      Parallel.ForEach(DriveInfo.GetDrives(), drive => {
        FindAllAtDrive(fileName, drive.Name);
      });
    }


    void ApplyAllFiles(DirectoryInfo directory, string pattern, Action<FileInfo> fileAction) {
      try {
        foreach (FileInfo file in directory.GetFiles(pattern)) {
          fileAction(file);
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
          FileInfo file = files[i] as FileInfo;
          Process proc = new Process();
          proc.StartInfo.FileName = file.FullName;
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
          FileInfo file = files[fileNum] as FileInfo;

          Console.Write($"1 -- compress{Environment.NewLine}" +
                      $"2 -- decompress{Environment.NewLine}" +
                      $"q -- quit{Environment.NewLine}" +
                      $"Your choice: ");
          var actionNum = Console.ReadLine();

          if (actionNum == "1") {
            Compress(file.FullName, file.FullName + ".zzz");
          }
          else if (actionNum == "2" && file.Extension == ".zzz") {
            Decompress(file.FullName, file.FullName.SkipLast(4).ToString());
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
      foreach (FileInfo file in files) 
        Console.WriteLine($"{++i}. {file.FullName}");
      Console.WriteLine();
    }
  }
}