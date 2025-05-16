using AplicationLayer.Service;
using DomainLayer.Dto;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _service;

        public TasksController(TaskService Service)
        {
            _service = Service;
        }


        [HttpGet]
        public async Task<ActionResult<Response<Tarea>>> GetAllTaskAsync() => await _service.GetAllTaskAsync();

        [HttpGet("id")]
        public async Task<ActionResult<Response<Tarea>>> GetByIdAllTaskAsync(int id) => await _service.GetByIdTaskAsync(id);

        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddAllTaskAsync(Tarea tarea) => await _service.AddAllTaskAsync(tarea);

        [HttpPut]
        public async Task<ActionResult<Response<string>>> UpdateAllTaskAsync(Tarea tarea) => await _service.UpdateAllTaskAsync(tarea);

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<string>>> DeleteAllTaskAsync(int id) => await _service.DeleteAllTaskAsync(id);

        [HttpPost("High priority")]
        public async Task<IActionResult> CrearTareaAltaPrioridad([FromBody] TaskDescriptionDto Dto)
        {
            var result = await _service.HighPriorityTask(Dto);
            if (result.Successful)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
