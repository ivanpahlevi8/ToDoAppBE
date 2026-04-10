using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ConnectionController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        [HttpPost]
        [Route("send-connection")]
        public async Task<IActionResult> SendConnection([FromBody] ConnectionDto connectionDto)
        {
            ResponseDto response = await _connectionService.SendConnection(connectionDto);

            if(!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("accept-connection")]
        public async Task<IActionResult> AcceptConnection(int connectionId)
        {
            ResponseDto response = await _connectionService.AcceptConnection(connectionId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("get-request-connection")]
        public async Task<IActionResult> GetRequestConnection(string requesterId)
        {
            ResponseDto response = await _connectionService.GetRequestedConnection(requesterId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("get-all-connection")]
        public async Task<IActionResult> GetAllConnection(string userId)
        {
            ResponseDto response = await _connectionService.GetAllConnectionUser(userId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
