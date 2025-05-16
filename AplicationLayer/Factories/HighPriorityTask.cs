using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;

namespace AplicationLayer.Factories
{
    public class HighPriorityTask: HighPriorityFactory
    {
        public override Tarea CreateHighPriorityTask(string description)
        {
            return new Tarea()
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(1),
                Status = "pending",
                AdditionalData = 3,
            };
        }
    }
   
}
