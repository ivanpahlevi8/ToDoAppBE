using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        [Route("get-requested-connection")]
        public async Task<IActionResult> GetRequestedConnection(string requesterId)
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

        [HttpPut]
        [Route("unconnect-user")]
        public async Task<IActionResult> UnconnectUser(string connectionId)
        {
            ResponseDto response = await _connectionService.UnconnectUser(connectionId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("declined-connection")]
        public async Task<IActionResult> DeclinedConnection(string connectionId)
        {
            ResponseDto response = await _connectionService.DeclinedConnection(connectionId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("connection-rejected-byuser")]
        public async Task<IActionResult> ConnectionRejectedByUser(string userId)
        {
            ResponseDto response = await _connectionService.GetAllConnectRejectByToUser(userId, true);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("connection-reject-touser")]
        public async Task<IActionResult> ConnectionRejectToUser(string userId)
        {
            ResponseDto response = await _connectionService.GetAllConnectRejectByToUser(userId, false);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("connection-disconnected-byuser")]
        public async Task<IActionResult> ConnectionDisconnectedByUser(string userId)
        {
            ResponseDto response = await _connectionService.GetAllDisconnectByToUser(userId, true);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("connection-disconnect-touser")]
        public async Task<IActionResult> ConnectionDisconnectToUser(string userId)
        {
            ResponseDto response = await _connectionService.GetAllDisconnectByToUser(userId, false);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete]
        [Route("remove-connection")]
        public async Task<IActionResult> RemoveConnection(int connectionId)
        {
            ResponseDto response = await _connectionService.DeleteConnection(connectionId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("get-reques-connection")]
        public async Task<IActionResult> GetRequestConnection(string userId)
        {
            ResponseDto response = await _connectionService.GetAllRequestConnection(userId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("get-is-connected")]
        public async Task<IActionResult> GetIsConnected(string userId, string userConnectedId)
        {
            ResponseDto response = await _connectionService.IsConnectedWithUser(userId, userConnectedId);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
