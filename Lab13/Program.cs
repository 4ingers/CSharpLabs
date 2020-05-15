using System;

namespace Lab13Space {
  class Program {
    static void Main(string[] args) {
      FileSearch.FileOpener(FileSearch.GetFiles("1.txt"));
      //FileFinder.FileZipper(FileFinder.FileGetter("1.txt", true));
      //FileFinder.FileZipper(FileFinder.FileGetter("compressed.cmprsd", true));
    }
  }
}
