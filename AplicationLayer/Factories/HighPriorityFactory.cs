using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Models;

namespace AplicationLayer.Factories
{
    public abstract class HighPriorityFactory
    {

        public abstract Tarea CreateHighPriorityTask(string description);
      

    }
}
