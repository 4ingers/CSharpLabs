using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

//TODO Проверка количества файлов, 
//TODO Убрать статичность
//TODO Сжатие/разжатие как отдельные методы

namespace Lab13Space {
  class FileSearch {

    ArrayList filesInfo;

    public ArrayList Files { get => filesInfo; private set => filesInfo = value; }


    public FileSearch() {
      Files = new ArrayList();
    }


    public FileSearch(string fileName) {
      Files = new ArrayList();

      foreach (DriveInfo drive in DriveInfo.GetDrives()) {
        DirectoryInfo directories = new DirectoryInfo(drive.Name);
        foreach (var dir in directories.GetDirectories()) {
          try {
            foreach (var file in dir.GetFiles(fileName, SearchOption.AllDirectories))
              Files.Add(file);
          }
          catch (UnauthorizedAccessException) {
            //Console.WriteLine(e.Message);
          }
        }
      }
    }


    public void FindAll(string fileName) {
      foreach (DriveInfo drive in DriveInfo.GetDrives()) {
        DirectoryInfo directories = new DirectoryInfo(drive.Name);
        foreach (var dir in directories.GetDirectories()) {
          try {
            foreach (var file in dir.GetFiles(fileName, SearchOption.AllDirectories)) 
              filesInfo.Add(file);
          }
          catch (UnauthorizedAccessException) {
            //Console.WriteLine(e.Message);
          }
        }
      }
    }

    public void FileOpener() {
      if (Files.Count == 0)
        return;

      while (true) {
        Console.WriteLine("Type number of file you want to open (\"exit\" -- to Exit): ");
        var c = Console.ReadLine();

        if (c == "exit")
          break;

        if (int.TryParse(c, out int num) && Files.Count > 0 && Files.Count <= num) {
          FileInfo fileInfo = Files[num] as FileInfo;
          Process proc = new Process();
          proc.StartInfo.FileName = fileInfo.FullName;
          proc.StartInfo.UseShellExecute = true;
          proc.Start();            
        }
      }  
    }


    public void FileZipper() {
      if (Files.Count != 0) {
        while (true) {
          Console.WriteLine("Type number of file you want to compress/decompress (\"exit\" -- to Exit): ");
          var c = Console.ReadLine();

          if (c == "exit")
            break;

          if (int.TryParse(c, out int i) && Files.Count > 0 && Files.Count <= i) {
            FileInfo fileInfo = Files[i] as FileInfo;
              
            if (fileInfo.Extension == ".cmprsd") {
              Console.WriteLine("You are going to decompress the file. Enter the full name: ");
              string targetFile = Console.ReadLine();
              Decompressor(fileInfo.FullName, targetFile);
            }
            else {
              Console.WriteLine("You are going to compress the file. Enter the full name without extension: ");
              string compressedFile = Console.ReadLine();
              compressedFile += ".cmprsd";
              Compressor(fileInfo.FullName, compressedFile);
            }
              
          }
        }
      }
    }


    public static void Compressor(string sourceFile, string compressedFile) {
      // source file stream
      using FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate);
      // target file stream
      using FileStream targetStream = File.Create(compressedFile);
      // compression stream
      using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);

      sourceStream.CopyTo(compressionStream);
      Console.WriteLine($"File {sourceFile} compressed. Size before: {sourceStream.Length}  size after: {targetStream.Length}.");
      Console.WriteLine($"Saved to {targetStream.Name}");
    }


    public static void Decompressor(string compressedFile, string targetFile) {
      // source file stream
      using FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate);
      // target file stream
      using FileStream targetStream = File.Create(targetFile);
      // decompression stream
      using GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);

      decompressionStream.CopyTo(targetStream);
      Console.WriteLine($"File decompressed: {targetFile}");
    }


    public string Print() {
      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < filesInfo.Count; ++i)
        sb.Append($"{i}. {(Files[i] as FileInfo).FullName}{Environment.NewLine}");

      return sb.ToString();
    }
  }
}