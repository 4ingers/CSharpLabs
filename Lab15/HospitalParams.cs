using System;
using System.Collections.Generic;
using System.Text;

namespace Lab15Space {
  public class HospitalParams {

    public const int DefaultReceptionCapacity = 3;
    public const int DefaultDoctorsCount = 5;
    public const int DefaultMaxSessionDuration = 15;

    public const int MaxNewPatientsCount = 3;

    public const int MaxUniqueCaseProbability = 40;
    public const int DespairTimeout = 10;

    public const int MaxImmunityLevel = 40;

    public const int MinGenerationInterval = 1;
    public const int MaxGenerationInterval = 4;

    public const int MinInfectionInterval = 1;
    public const int MaxInfectionInterval = 8;

    public const int MinRelaxInterval = 2;
    public const int MaxRelaxInterval = 4;

    public const double ReceptionEntranceCheckInterval = 0.2;
    public const double SurgeryEntranceCheckInterval = 0.2;
  }
}
