using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            ResponseDto responseDto = await _authService.RegisterUser(registerUserDto);

            if(!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }

        [HttpPost]
        [Route("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            ResponseDto responseDto = await _authService.LoginUser(loginUserDto);

            if (!responseDto.IsSuccess)
            {
                return BadRequest(responseDto);
            }

            return Ok(responseDto);
        }
    }
}
