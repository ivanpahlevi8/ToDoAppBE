using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ITodoService _toDoService;

        public ToDoController(ITodoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpPost]
        [Route("create-todo")]
        public async Task<IActionResult> CreateToDo([FromBody] ToDoDto toDoDto)
        {
            ResponseDto responseDto = await _toDoService.CreateToDo(toDoDto);

            if(!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPut]
        [Route("update-todo")]
        public async Task<IActionResult> UpdateToDo([FromBody] ToDoDto toDoDto)
        {
            ResponseDto responseDto = await _toDoService.UpddateToDo(toDoDto);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }
    }
}
