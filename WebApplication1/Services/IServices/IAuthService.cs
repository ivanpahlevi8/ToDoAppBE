using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface IAuthService
    {
        public Task<ResponseDto> RegisterUser(RegisterUserDto registerUserDto);
        public Task<ResponseDto> LoginUser(LoginUserDto loginUserDto);
        public Task<ResponseDto> GetUserByUsername(string username);
    }
}
