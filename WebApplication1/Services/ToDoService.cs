using AutoMapper;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class ToDoService : ITodoService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private ResponseDto _responseDto;

        public ToDoService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> CreateToDo(ToDoDto toDtoDto)
        {
            try
            {
                ToDoModel toDoModel = _mapper.Map<ToDoModel>(toDtoDto);

                await _dbContext.ToDo.AddAsync(toDoModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success create To Do";

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

        public async Task<ResponseDto> UpddateToDo(ToDoDto toDoDto)
        {
            try
            {
                ToDoModel toDoModel = _mapper.Map<ToDoModel>(toDoDto);

                _dbContext.Update(toDoModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success update todo";
                _responseDto.Result = "Success update todo";

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
    }
}
