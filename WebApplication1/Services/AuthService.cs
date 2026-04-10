using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<UserModel> _userManager;
        private ResponseDto _responseDto;

        public AuthService(AppDbContext dbContext, IMapper mapper, UserManager<UserModel> userManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> GetUserByUsername(string username)
        {
            try
            {
                UserModel? userModel = await _dbContext.User.FirstOrDefaultAsync(u => u.UserName == username);

                if(userModel == null)
                {
                    _responseDto.Message = $"User with id {username} is not exist";
                    _responseDto.IsSuccess = false;

                    return _responseDto;
                }

                _responseDto.Message = "Success get user by username";
                _responseDto.Result = userModel;
                _responseDto.IsSuccess = true;

                return _responseDto;
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> LoginUser(LoginUserDto loginUserDto)
        {
            try
            {
                UserModel? getUserModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginUserDto.Username);

                if (getUserModel == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Error when getting user with username " + loginUserDto.Username;
                    
                    return _responseDto;
                }

                bool isPasswordCorrect = await _userManager.CheckPasswordAsync(getUserModel, loginUserDto.Password);

                if(isPasswordCorrect)
                {
                    _responseDto.IsSuccess = true;
                    _responseDto.Message = "Login Success";
                    _responseDto.Result = getUserModel;

                    return _responseDto;
                } else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = "Wrong credential input";

                    return _responseDto;
                }
            }catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> RegisterUser(RegisterUserDto registerUserDto)
        {
            try
            {
                UserModel userModel = new UserModel
                {
                    Email = registerUserDto.Email,
                    FirstName = registerUserDto.FirstName,
                    LastName = registerUserDto.LastName,
                    PhoneNumber = registerUserDto.PhoneNumber,
                    UserName = registerUserDto.UserName,
                    CreatedAt = DateTime.Now,
                };


                var response = await _userManager.CreateAsync(userModel, registerUserDto.Password);

                if(response.Succeeded)
                {
                    // return registered user as response
                    UserModel? getUserModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == registerUserDto.UserName);

                    if(getUserModel == null) 
                    {
                        _responseDto.IsSuccess = false;
                        _responseDto.Message = "Error when registered as a new user";
                        _responseDto.Result = null;

                        return _responseDto;
                    }

                    _responseDto.IsSuccess = true;
                    _responseDto.Message = "Success register as a new user";
                    _responseDto.Result = getUserModel;

                    return _responseDto;
                } else
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Error when register : {response.Errors.First().Description}";
                    _responseDto.Result = null;

                    return _responseDto;

                }
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }
    }
}
