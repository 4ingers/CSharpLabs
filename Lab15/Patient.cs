using System;
using System.Collections.Generic;
using System.Text;

namespace Lab15Space {
  class Patient : IHuman {

    public bool Ill { get; set; }
    public int Immunity { get;  }
    public string Name { get; }


    public Patient(string name, int probabilityIll) {
      Name = name;
      Ill = new Random().Next(100) < probabilityIll;
      Immunity = new Random().Next(100);
    }


    public void Cure() => Ill = false;
    public void Infect() => Ill = true;
  }
}
