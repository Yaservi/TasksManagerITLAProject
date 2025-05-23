using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AplicationLayer.Helper;
using AplicationLayer.Repository.ICommon;
using DomainLayer.Dto;
using DomainLayer.Models;

namespace AplicationLayer.Service
{
    public class TaskService
    {
        private ICommonProcess<Tarea> _commonProcess;
        private readonly TaskHelper _taskHelper;
        private readonly Queue<Tarea> _queue = new Queue<Tarea>();

        public TaskService(ICommonProcess<Tarea> commonProcess, TaskHelper taskHelper)
        {
            _commonProcess = commonProcess;
            _taskHelper = taskHelper;

        }



        public async Task<Response<string>>HighPriorityTask(TaskDescriptionDto dto)
        {
            var response = new Response<string>();
            try
            {
                var factory = new AplicationLayer.Factories.HighPriorityTask();
                var tarea = factory.CreateHighPriorityTask(dto.Description);
                if (!_taskHelper.Validate(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea de alta prioridad no es válida para crear";
                    return response;
                }
                _queue.Enqueue(tarea);

                _taskHelper.NotificationCreation(tarea);

                int daysLefts = _taskHelper.CalculateDaysLeft(tarea);
                Console.WriteLine($"Días restantes para culminar la tarea: {daysLefts}");

                var result = await _commonProcess.AddAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

                if (!result.IsSuccess)
                {
                    response.Errors.Add("No se pudo crear la tarea de alta prioridad");
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }
 

        public async Task<Response<Tarea>> GetAllTaskAsync()
        {
            var response = new Response<Tarea>();
            try
            {

                response.DataList = await _commonProcess.GetAllAsync();
                response.Successful = true;
                
            }
            catch (Exception e) 
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }
        public async Task<Response<Tarea>> GetByIdTaskAsync(int id)
        {
            var response = new Response<Tarea>();
            try
            {
                var result = await _commonProcess.GetIdAsync(id);
                if (result != null) {
                    response.SingleData = result;
                    response.Successful = true;
                }else
                {
                    response.Successful = false;
                    response.Message = "No se encontró información";
                }
                
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddAllTaskAsync(Tarea tarea)
        {
            var response = new Response<string>();

            try
            {


                if (!_taskHelper.Validate(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es valida para crear";
                    return response;
                }
                _queue.Enqueue(tarea);

                _taskHelper.NotificationCreation(tarea);

                int daysLefts = _taskHelper.CalculateDaysLeft(tarea);
                Console.WriteLine($"Dias restantes para culminar la tarea: {daysLefts}");


               var result = await _commonProcess.AddAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

                if (!result.IsSuccess)
                {
                   
                    response.Errors.Add("No se pudo crear la tarea");
                }
            }

            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> UpdateAllTaskAsync(Tarea tarea)
        {
            var response = new Response<string>();
            try
            {
                if (!_taskHelper.Validate(tarea))
                {
                    response.Successful = false;
                    response.Message = "La tarea no es valida para Actualizar";
                    return response;
                }
                _queue.Enqueue(tarea);

                _taskHelper.NotificationCreation(tarea);

                int daysLefts = _taskHelper.CalculateDaysLeft(tarea);
                Console.WriteLine($"Dias restantes para culminar la tarea: {daysLefts}");

                var result = await _commonProcess.UpdateAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

                if(!result.IsSuccess)
                {
                    response.Errors.Add("No se pudo guardar la tarea");
                }

            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

        public async Task<Response<string>> DeleteAllTaskAsync(int id)
        {
            var response = new Response<string>();
            try
            {
                var result = await _commonProcess.DeleteAsync(id);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }
            return response;
        }

    }
}
