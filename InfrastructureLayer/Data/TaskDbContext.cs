using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Data
{
    public class TaskDbContext: DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) 
        {
            
        }

        public DbSet<DomainLayer.Models.Tarea> Tareas { get; set; }

    }
}
