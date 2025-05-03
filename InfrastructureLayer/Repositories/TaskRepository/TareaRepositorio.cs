using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicationLayer.Repository.ICommon;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repositories.TaskRepository
{
    public class TareaRepositorio : ICommonProcess<Tarea>
    {
        private readonly TaskDbContext _dbContext;

        public TareaRepositorio(TaskDbContext DbContext)
        {
            _dbContext = DbContext;
        }
        public async Task<IEnumerable<Tarea>> GetAllAsync() => await _dbContext.Tareas.ToListAsync();
        public async Task<Tarea> GetIdAsync(int id) => await _dbContext.Tareas.FindAsync(id);
        public async Task<(bool IsSuccess, string Message)> AddAsync(Tarea entry)
        {
            try
            {
                await _dbContext.Tareas.AddAsync(entry);
                await _dbContext.SaveChangesAsync();
                return (true, "La tarea ha sido guardada con exito");
            }
            catch(Exception)
            {
                return (false, "No se pudo guardar la tarea");
            }
        }
        public async Task<(bool IsSuccess, string Message)> UpdateAsync(Tarea entry)
        {
            try
            {
                _dbContext.Tareas.Update(entry);
                await _dbContext.SaveChangesAsync();
                return (true, "La tarea ha sido actualizada con exito");
            }
            catch (Exception)
            {
                return (false, "No se pudo actualizar la tarea");
            }
        }
        public async Task<(bool IsSuccess, string Message)> DeleteAsync(int id)
        {
            try
            {
                var tarea = await _dbContext.Tareas.FindAsync(id);
                if (tarea != null)
                {
                    _dbContext.Tareas.Remove(tarea);
                    await _dbContext.SaveChangesAsync();
                    return (true, "La tarea se eliminó correctamente");
                }
                else
                {
                    return (false, "No se encontró la tarea");
                }

            }
            catch (Exception)
            {
                return (false, "No se pudo actualizar la tarea");
            }
        }

        

      
    }
}
