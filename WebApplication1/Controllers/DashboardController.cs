using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("project-summary")]
        public async Task<IActionResult> ProjectSummary(string userId)
        {
            ResponseDto responseDto = await _dashboardService.GetDashboardProjectSummary(userId);

            if(responseDto.IsSuccess)
            {
                return Ok(responseDto);
            } else
            {
                return BadRequest(responseDto);
            }
        }

        [HttpGet]
        [Route("todo-summary")]
        public async Task<IActionResult> ToDoSummary(string userId)
        {
            ResponseDto responseDto = await _dashboardService.GetDashboardToDoSummary(userId);

            if (responseDto.IsSuccess)
            {
                return Ok(responseDto);
            }
            else
            {
                return BadRequest(responseDto);
            }
        }

        [HttpGet]
        [Route("team-summary")]
        public async Task<IActionResult> TeamSummary(string userId)
        {
            ResponseDto responseDto = await _dashboardService.GetDashboardTeamSummary(userId);

            if (responseDto.IsSuccess)
            {
                return Ok(responseDto);
            }
            else
            {
                return BadRequest(responseDto);
            }
        }

        [HttpGet]
        [Route("connection-summary")]
        public async Task<IActionResult> ConnectionSummary(string userId)
        {
            ResponseDto responseDto = await _dashboardService.GetDashboardConnectionSummary(userId);

            if (responseDto.IsSuccess)
            {
                return Ok(responseDto);
            }
            else
            {
                return BadRequest(responseDto);
            }
        }
    }
}
