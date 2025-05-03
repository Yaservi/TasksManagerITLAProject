using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AplicationLayer.Repository.ICommon;
using DomainLayer.Dto;
using DomainLayer.Models;

namespace AplicationLayer.Service
{
    public class TaskService
    {
        private ICommonProcess<Tarea> _commonProcess;

        public TaskService(ICommonProcess<Tarea> commonProcess)
        {
            _commonProcess = commonProcess;
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
               var result = await _commonProcess.AddAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

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
                var result = await _commonProcess.UpdateAsync(tarea);
                response.Message = result.Message;
                response.Successful = result.IsSuccess;

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
