using System;

namespace ExceptionSpace {
  public class OperationException : Exception {
    public string Operation { get; private set; }
    public OperationException(string operation, string message) : base(message) {
      Operation = operation;
    }
  }
}
