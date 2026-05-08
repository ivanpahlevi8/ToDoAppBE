using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamRoleController : ControllerBase
    {
        private readonly ITeamRoleService _teamRoleService;

        [HttpPost]
        [Route("create-teamrole")]
        public async Task<IActionResult> CreateTeamRole([FromBody] TeamRoleDto teamRoleDto)
        {
            ResponseDto response = await _teamRoleService.CreateTeamRole(teamRoleDto);

            if(!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete]
        [Route("delete-teamrole")]
        public async Task<IActionResult> DeleteTeamRole(int teamRoleId)
        {
            ResponseDto response = await _teamRoleService.DeleteTeamRole(teamRoleId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("get-teamroles")]
        public async Task<IActionResult> GetAllTeamRoles(int teamId)
        {
            ResponseDto response = await _teamRoleService.GetAllTeamRole(teamId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
