using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost]
        [Route("/create-project")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectDto)
        {
            ResponseDto responseDto = await _projectService.CreateProject(projectDto);

            if(!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpGet]
        [Route("/get-project")]
        public async Task<IActionResult> GetProject(int projectId)
        {
            ResponseDto responseDto = await _projectService.GetProjectById(projectId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpGet]
        [Route("/get-all-project")]
        public async Task<IActionResult> GetAllProject()
        {
            ResponseDto responseDto = await _projectService.GetAllProject();

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPut]
        [Route("/update-project")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDto project)
        {
            ResponseDto responseDto = await _projectService.UpdateProject(project);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpDelete]
        [Route("/delete-project")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            ResponseDto responseDto = await _projectService.DeleteProject(projectId);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }
    }
}
