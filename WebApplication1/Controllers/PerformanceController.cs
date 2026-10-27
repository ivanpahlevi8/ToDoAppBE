using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _service;

        public PerformanceController(IPerformanceService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("get-todo-performance")]
        public async Task<IActionResult> GetToDoPerformance(string userId)
        {
            ResponseDto response = await _service.ToDoPerformance(userId);

            if(response.IsSuccess)
            {
                return Ok(response);
            } else
            {
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("get-project-performance")]
        public async Task<IActionResult> GetProjectPerformance(string userId)
        {
            ResponseDto response = await _service.ProjectPerformance(userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("get-team-performance")]
        public async Task<IActionResult> GetTeamPerformance(string userId)
        {
            ResponseDto response = await _service.TeamPerformance(userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
