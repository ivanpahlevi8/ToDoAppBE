using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface ITodoService
    {
        public Task<ResponseDto> CreateToDo(ToDoDto toDtoDto);

        public Task<ResponseDto> UpddateToDo(ToDoDto toDtoDto);

        // get all to do within project
        public Task<ResponseDto> GetToDoWithinProject(int projectId);

        // delete to do
        public Task<ResponseDto> DeleteToDo(int toDoId);
    }
}
