using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

                // map to do model to dto
                ToDoDto toDoDto = _mapper.Map<ToDoDto>(toDoModel);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success create To Do";
                _responseDto.Result = toDoDto;

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

        public async Task<ResponseDto> DeleteToDo(int toDoId)
        {
            try
            {
                ToDoModel? getToDo = await _dbContext.ToDo.FirstOrDefaultAsync(t => t.ToDoId == toDoId);

                if(getToDo == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"To Do with id {toDoId} is not exist";
                    _responseDto.Result = null;

                    return _responseDto;
                }

                _dbContext.ToDo.Remove(getToDo);
                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success delete item";
                _responseDto.Result = $"Success delete to do with id {toDoId}";

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

        public async Task<ResponseDto> GetToDoWithinProject(int projectId)
        {
            try
            {
                List<ToDoModel> getAllToDoModel = await _dbContext.ToDo.Where(td => td.ProjectId == projectId).ToListAsync();

                List<ToDoDto> getToDoDto = _mapper.Map<List<ToDoDto>>(getAllToDoModel);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all to within project";
                _responseDto.Result = getToDoDto;

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
