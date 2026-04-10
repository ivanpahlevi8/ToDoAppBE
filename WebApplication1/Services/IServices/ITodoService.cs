using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface ITodoService
    {
        public Task<ResponseDto> CreateToDo(ToDoDto toDtoDto);

        public Task<ResponseDto> UpddateToDo(ToDoDto toDtoDto);
    }
}
