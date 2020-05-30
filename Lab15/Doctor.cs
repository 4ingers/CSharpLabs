using System;
using System.Collections.Generic;
using System.Text;

namespace Lab15Space {

  class Doctor : IHuman {
    public string Name { get; }
    public bool Busy { get; set; }
    public bool Helps { get; set; }

    public Doctor(string name) {
      Name = name;
      Busy = false;
      Helps = false;
    }
    
  }
}
