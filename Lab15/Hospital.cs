using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

    
    // Очередь
    LinkedList<Patient> queue = new LinkedList<Patient>();

    // Рецепция
    ConcurrentQueue<Patient> reception = new ConcurrentQueue<Patient>();

    // Свободные врачи
    ConcurrentQueue<Doctor> surgeries = new ConcurrentQueue<Doctor>();


    // Фоновые задачи
    TaskFactory backgroundWork = new TaskFactory(
        TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

    // Работа врачей
    TaskFactory doctorsWork = new TaskFactory();

    // Блок очереди
    object queueLocker = new object();



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
      ShowInitinalParams();
      DoctorsPrepatarion();
      
      backgroundWork.StartNew(PatientsComing);
      backgroundWork.StartNew(InfectAll);
      backgroundWork.StartNew(SessionsOrganizer);
      backgroundWork.StartNew(ReceptionEntrance);

      Thread.Sleep(_hospitalWorkTimeout);
    }


    private void ShowInitinalParams() {
      HospitalHistory.WritelnWhite(
        $" Вместимость приёмной\t: {_receptionCapacity}{Environment.NewLine}" +
        $" Количество докторов\t: {_doctorsCount}{Environment.NewLine}" +
        $" Макс. время осмотра\t: {_maxSessionDuration}{Environment.NewLine}");
    }


    private void DoctorsPrepatarion() {
      for (int i = 0; i < _doctorsCount; i++) {
        string name = NameGetter.Get();
        Doctor doctor = new Doctor(name);
        surgeries.Enqueue(doctor);
      }
    }


    private void SessionsOrganizer() {
      while (true) {
        Thread.Sleep(TimeSpan.FromSeconds(HospitalParams
          .SurgeryEntranceCheckInterval));

        if (reception.Count != 0 && surgeries.Count != 0)
          doctorsWork.StartNew(Session);
      }
    }


    private void Session() {

      if (!reception.TryDequeue(out Patient patient) ||
          !surgeries.TryDequeue(out Doctor doctor))
        return;

      Thread.CurrentThread.Name = $"Доктор {doctor.Name} " +
        $"{(patient.Ill ? "лечит" : "консультирует")} {patient.Name}";
      HospitalHistory.WritelnYellow($"Доктор {doctor.Name} " +
        $"{(patient.Ill ? "лечит" : "консультирует")} {patient.Name}");

      int sessionDuration = new Random().Next(1, _maxSessionDuration);
      Thread.Sleep(TimeSpan.FromSeconds(sessionDuration));

      int helpingDuration = UniqueCase(doctor);

      HospitalHistory.WritelnGreen($"Доктор {doctor.Name} " +
        $"{(patient.Ill ? "лечил" : "консультировал")} {patient.Name} " +
        $"{sessionDuration + helpingDuration}с. Сейчас он отдохнёт и продолжит приём..");

      int relaxDuration = new Random().Next(HospitalParams.MinRelaxInterval,
                                            HospitalParams.MaxRelaxInterval);
      Thread.Sleep(TimeSpan.FromSeconds(relaxDuration));

      HospitalHistory.WritelnYellow($"Доктор {doctor.Name} отдохнул {relaxDuration}с " +
        $"и готов работать дальше");

      surgeries.Enqueue(doctor);
    }


    private int UniqueCase(Doctor requester) {
      int uniqueCaseProbability = new Random().Next(100);
      if (uniqueCaseProbability > HospitalParams.MaxUniqueCaseProbability)
        return 0;

      HospitalHistory.WritelnCyan($"Доктору {requester.Name} нужна помощь!");
      
      Doctor assistant = null;

      // Ждем помощи, пока не отчаимся. Дождались -- убираем ассистента из очереди
      var someoneCame = SpinWait
        .SpinUntil(() => surgeries.TryDequeue(out assistant),
                   (int)TimeSpan.FromSeconds(HospitalParams.DespairTimeout)
                     .TotalMilliseconds);

      // Никто не пришёл
      if (!someoneCame) {
        HospitalHistory.WritelnCyan($"Доктору {requester.Name} никто не помог..");
        return 0;
      }

      HospitalHistory.WritelnCyan($"Доктор {assistant.Name} " +
        $"помогает доктору {requester.Name}");

      // Помощь..
      int helpingDuration = new Random().Next(1, _maxSessionDuration);

      doctorsWork.StartNew(() => {
        Thread.Sleep(TimeSpan.FromSeconds(helpingDuration));
      });
      Thread.Sleep(TimeSpan.FromSeconds(helpingDuration));

      HospitalHistory.WritelnCyan($"Доктор {assistant.Name} " +
        $" помогал доктору {requester.Name} {helpingDuration}с");

      // Вернули ассистента обратно вперед очереди
      surgeries.AsParallel().Prepend(assistant);

      return helpingDuration;
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
            HospitalHistory.WritelnRed($"{name} теперь в очереди, нужно лечение");
          else 
            HospitalHistory.WritelnWhite($"{name} теперь в очереди, нужна консультация");
        }
        return patient;
      }

      if (patient.Ill)
        HospitalHistory.WritelnRed($"{patient.Name} вошёл в " +
          $"{(reception.IsEmpty ? "пустую " : "")}смотровую");
      else
        HospitalHistory.WritelnWhite($"{patient.Name} вошёл в " +
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

                  HospitalHistory.WritelnRed($"{node.Previous.Value.Name} " +
                    $"заразился от {node.Value.Name}");
                }
                // ..потом другого (если не уперлись в конец)
                if (node.Next != null && !node.Next.Value.Ill &&
                  new Random().Next(100) > node.Next.Value.Immunity) {
                  
                  node.Next.Value.Infect();

                  HospitalHistory.WritelnRed($"{node.Next.Value.Name} " +
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

            HospitalHistory.WritelnGreen($"{patient.Name} вошёл в пустую смотровую");
            continue;
          }

          var entrant = queue.First.Value;
          if (reception.TryPeek(out var oneFromReception) &&
              entrant.Ill == oneFromReception.Ill) {
            var patient = MovePatientToReception();

            HospitalHistory.WritelnGreen($"{patient.Name} вошёл в смотровую. ");
          }
        }
      }
    }

  }
}
