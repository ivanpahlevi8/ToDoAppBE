using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        [Route("/create-team")]
        public async Task<IActionResult> CreateTeam([FromBody] TeamDto teamDto)
        {
            ResponseDto responseDto = await _teamService.CreateTeam(teamDto);

            if(!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpGet]
        [Route("/get-team")]
        public async Task<IActionResult> GetTeam(int teamId)
        {
            ResponseDto responseDto = await _teamService.GetTeam(teamId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpGet]
        [Route("/get-all-team")]
        public async Task<IActionResult> GetAllTeam()
        {
            ResponseDto responseDto = await _teamService.GetAllTeam();

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpGet]
        [Route("/get-team-member")]
        public async Task<IActionResult> GetTeamMember(int teamId)
        {
            ResponseDto responseDto = await _teamService.GetAllTeamMember(teamId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPut]
        [Route("/update-team")]
        public async Task<IActionResult> UpdateTeam([FromBody] TeamDto teamDto)
        {
            ResponseDto responseDto = await _teamService.UpdateTeam(teamDto);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpDelete]
        [Route("/delete-team")]
        public async Task<IActionResult> DeleteTeam(int teamId)
        {
            ResponseDto responseDto = await _teamService.DeleteTeam(teamId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPost]
        [Route("/assign-user-team")]
        public async Task<IActionResult> AssignUserTeam(string userId, int teamId)
        {
            ResponseDto responseDto = await _teamService.AssignUserToTeam(userId, teamId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPost]
        [Route("/unassign-user-team")]
        public async Task<IActionResult> UnassignUserTeam(string userId, int teamId)
        {
            ResponseDto responseDto = await _teamService.UnAssignedUserToTeam(userId, teamId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }
    }
}
