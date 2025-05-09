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

        


    }
}

