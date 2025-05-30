using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;

namespace AplicationLayer.Helper
{
    public class TaskHelper
    {
        private readonly Dictionary<string, double> _completionRateCache = new();
        private readonly Dictionary<string, List<Tarea>> _filterByStatusCache = new();
        delegate bool ValidateTask(Tarea tarea);

        public bool Validate(Tarea tarea)
        {
            if (tarea == null) 
                return false;

            ValidateTask validate = task => !string.IsNullOrWhiteSpace(tarea.Description);

            return validate(tarea);

        }
        public void NotificationCreation(Tarea message)
        {

            Action<Tarea> notify = task => Console.WriteLine($"Task created{task.Description}");
            notify(message);


        }

        public int CalculateDaysLeft(Tarea tarea)
        {

            Func<Tarea, int> daysleft = task => (task.DueDate.Date - DateTime.Now.Date).Days;
            return daysleft(tarea);

        }

        public void NotifyCompletion(Tarea tarea)
        {
            Action<Tarea> notify = task => Console.WriteLine($"Task completed: {task.Description}");
            notify(tarea);
        }

        public double CalculateTaskCompletionRateMemoized(IEnumerable<Tarea> tareas)
        {
            // Clave única basada en los IDs y estados de las tareas
            var key = string.Join(",", tareas.Select(t => $"{t.Id}:{t.Status}"));

            if (_completionRateCache.TryGetValue(key, out var rate))
                return rate;

            int total = tareas.Count();
            if (total == 0) return 0;
            int completadas = tareas.Count(t => t.Status == "completed");
            rate = (double)completadas / total * 100;
            _completionRateCache[key] = rate;
            return rate;
        }

        public List<Tarea> FilterTasksByStatusMemoized(IEnumerable<Tarea> tareas, string status)
        {
            var key = $"{status}:{string.Join(",", tareas.Select(t => t.Id))}";

            if (_filterByStatusCache.TryGetValue(key, out var result))
                return result;

            result = tareas.Where(t => t.Status == status).ToList();
            _filterByStatusCache[key] = result;
            return result;
        }
    




    }
}

