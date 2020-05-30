using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Lab15Space {

  class Hospital {

    // Вместимость смотровой
    readonly int _receptionCapacity;
    
    // Количество докторов
    readonly int _doctorsCount;

    // Максимальное время приема
    readonly int _maxSessionDuration;

    // Время работы 
    readonly TimeSpan _hospitalWorkTimeout;


    Logger logger = new Logger();
    
    // Очередь
    LinkedList<Patient> queue = new LinkedList<Patient>();

    // Рецепция
    ConcurrentQueue<Patient> reception = new ConcurrentQueue<Patient>();

    // Свободные врачи
    ConcurrentQueue<Doctor> surgeries = new ConcurrentQueue<Doctor>();


    // Блок очереди
    private object queueLocker = new object();



    public Hospital() : this(HospitalParams.DefaultReceptionCapacity,
                             HospitalParams.DefaultDoctorsCount,
                             HospitalParams.DefaultMaxSessionDuration) { }


    public Hospital(int hospitalWorkTimeout) : this() =>
      _hospitalWorkTimeout = TimeSpan.FromSeconds(hospitalWorkTimeout);


    public Hospital(int receptionCapacity,
                    int doctorsCount,
                    int maxAppointmentDuration) {
      _receptionCapacity = receptionCapacity;
      _doctorsCount = doctorsCount;
      _maxSessionDuration = maxAppointmentDuration;
      _hospitalWorkTimeout = Timeout.InfiniteTimeSpan;
    }

    public Hospital(int receptionCapacity,
                    int doctorsCount, 
                    int maxAppointmentDuration,
                    int hospitalWorkTimeout) {
      _receptionCapacity = receptionCapacity;
      _doctorsCount = doctorsCount;
      _maxSessionDuration = maxAppointmentDuration;
      _hospitalWorkTimeout = TimeSpan.FromSeconds(hospitalWorkTimeout);
    }



    public void Work() {

      Console.WriteLine(
          $"Вместимость приёмной : {_receptionCapacity}{Environment.NewLine}" +
          $"Количество докторов  : {_doctorsCount}{Environment.NewLine}" +
          $"Максимальная длительность осмотра : {_maxSessionDuration}{Environment.NewLine}");

      DoctorsPrepatarion();

      PatientsGenerator();

      Pandemic();

      HospitalDoor();

      DistributePatients();

      Thread.Sleep(_hospitalWorkTimeout);

    }


    private void DoctorsPrepatarion() {
      for (int i = 0; i < _doctorsCount; i++) {
        string name = NameGetter.Get();
        Doctor doctor = new Doctor(name);
        surgeries.Enqueue(doctor);
      }
    }


    private void DistributePatients() {
      Thread distributor = new Thread(SessionsOrganizer);
      distributor.Name = nameof(distributor);
      distributor.IsBackground = true;
      distributor.Start();
    }


    private void SessionsOrganizer() {
      while (true) {
        Thread.Sleep(TimeSpan.FromSeconds(
          HospitalParams.SurgeryEntranceCheckInterval));

        if (reception.Count != 0 && surgeries.Count != 0)
          ThreadPool.QueueUserWorkItem(Session);
      }
    }


    private void Session(object state) {

      if (!reception.TryDequeue(out Patient patient) ||
          !surgeries.TryDequeue(out Doctor doctor))
        return;

      logger.WriteLineYellow($"Доктор {doctor.Name} " +
        $"{(patient.Ill ? "лечит" : "консультирует")} {patient.Name}");

      int sessionDuration = new Random().Next(1, _maxSessionDuration);
      Thread.Sleep(TimeSpan.FromSeconds(sessionDuration));


      // Помощь врача
      int uniqueCaseProbability = new Random().Next(100);
      int helpingDuration = 0;
      if (uniqueCaseProbability < HospitalParams.MaxUniqueCaseProbability)
        helpingDuration = Helping(doctor);


      logger.WriteLineGreen($"Доктор {doctor.Name} " +
        $"{(patient.Ill ? "лечил" : "консультировал")} {patient.Name} " +
        $"{sessionDuration + helpingDuration}с");

      int relaxDuration = new Random().Next(HospitalParams.MinRelaxInterval,
                                            HospitalParams.MaxRelaxInterval);
      Thread.Sleep(TimeSpan.FromSeconds(relaxDuration));

      logger.WriteLineYellow($"Доктор {doctor.Name} отдохнул {relaxDuration}с " +
        $"и готов работать дальше");

      surgeries.Enqueue(doctor);
    }


    private int Helping(Doctor requester) {
      Doctor assistant = null;

      logger.WriteLineCyan($"Доктору {requester.Name} нужна помощь!");

      var someoneCame = SpinWait
        .SpinUntil(() => surgeries.TryDequeue(out assistant),
                   (int)TimeSpan.FromSeconds(HospitalParams.DespairTimeout)
                     .TotalMilliseconds);

      if (!someoneCame) {
        logger.WriteLineCyan($"Доктору {requester.Name} никто не помог..");
        return 0;
      }

      logger.WriteLineCyan($"Доктор {assistant.Name} " +
        $"помогает доктору {requester.Name}");

      int helpingDuration = new Random().Next(1, _maxSessionDuration);
      Thread.Sleep(TimeSpan.FromSeconds(helpingDuration));

      logger.WriteLineCyan($"Доктор {assistant.Name} " +
        $" помогал доктору {requester.Name} {helpingDuration}с");

      surgeries.Prepend(assistant);

      return helpingDuration;
    }


    private void PatientsGenerator() {
      Thread patientAdder = new Thread(PatientsComing);
      patientAdder.Name = nameof(patientAdder);
      patientAdder.IsBackground = true;
      patientAdder.Start();
    }



    private Patient AddPatient() {
      string name = NameGetter.Get();

      int illnessProbability = new Random().Next(100);

      Patient patient = new Patient(name, illnessProbability);


      if (reception.Count ==  _receptionCapacity ||
          queue.Count != 0) {
        lock (queueLocker) {
          queue.AddLast(patient);

          if (patient.Ill)
            logger.WriteLineRed($"{name} теперь в очереди, нужно лечение");
          else 
            logger.WriteLine($"{name} теперь в очереди, нужна консультация");
        }
        return patient;
      }

      logger.WriteLine($"{patient.Name} вошёл в " +
        $"{(reception.IsEmpty ? "пустую " : "")}смотровую");

      reception.Enqueue(patient);

      return patient;
    }
    

    private void PatientsComing() {
      while (true) {
        Thread.Sleep(TimeSpan.FromSeconds(
          new Random().Next(HospitalParams.MinGenerationInterval,
                            HospitalParams.MaxGenerationInterval)));

        int newPatientsCount = new Random().Next(
          1, HospitalParams.MaxNewPatientsCount);

        for (int i = 0; i < newPatientsCount; i++)
          AddPatient();

      }
    }

    

    private void Pandemic() {
      Thread infectious = new Thread(InfectAll);
      infectious.Name = nameof(infectious);
      infectious.IsBackground = true;
      infectious.Start();
    }


    private void InfectAll() {
      while (true) {
        Thread.Sleep(TimeSpan.FromSeconds(
          new Random().Next(HospitalParams.MinInfectionInterval,
                            HospitalParams.MaxInfectionInterval)));

        lock (queueLocker) {
          // Будем итерировать очередь вручную
          for (var node = queue.First; node != null; node = node.Next) {

            // Нашли больного
            if (node.Value.Ill) {
              lock (queueLocker) {

                // Попытаемся заразить одного соседа (если не уперлись в начало)..
                if (node.Previous != null && !node.Previous.Value.Ill &&
                  new Random().Next(100) > node.Previous.Value.Immunity) {
                  
                  node.Previous.Value.Infect();

                  logger.WriteLineRed($"{node.Previous.Value.Name} " +
                    $"заразился от {node.Value.Name}");
                }
                // ..потом другого (если не уперлись в конец)
                if (node.Next != null && !node.Next.Value.Ill &&
                  new Random().Next(100) > node.Next.Value.Immunity) {
                  
                  node.Next.Value.Infect();

                  logger.WriteLineRed($"{node.Next.Value.Name} " +
                    $"заразился от {node.Value.Name}");
                }
              }
              if (node.Next != null) {
                node = node.Next;
              }
            }
          }
        }
      }
    }



    private void HospitalDoor() {
      Thread entranceSecurity = new Thread(ReceptionEntrance);
      entranceSecurity.Name = nameof(entranceSecurity);
      entranceSecurity.IsBackground = true;
      entranceSecurity.Start();
    }


    private Patient MovePatientToReception() {
      Patient patient = queue.First.Value;
      queue.RemoveFirst(); 
      reception.Enqueue(patient);
      return patient;
    }


    private void ReceptionEntrance() {
      while (true) {
        Thread.Sleep(TimeSpan.FromSeconds(
          HospitalParams.ReceptionEntranceCheckInterval));

        lock (queueLocker) {
          if (queue.Count == 0)
            continue;

          if (reception.Count == _receptionCapacity)
            continue;

          if (reception.IsEmpty) {
            var patient = MovePatientToReception();

            logger.WriteLineGreen($"{patient.Name} вошёл в пустую смотровую");
            continue;
          }

          var entrant = queue.First.Value;
          if (reception.TryPeek(out var oneFromReception) &&
              entrant.Ill == oneFromReception.Ill) {
            var patient = MovePatientToReception();

            logger.WriteLineGreen($"{patient.Name} вошёл в смотровую. ");
          }
        }
      }
    }

  }
}
