using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface IProjectService
    {
        // craete function to create project
        public Task<ResponseDto> CreateProject(ProjectDto projectDto);

        // function to read object by id
        public Task<ResponseDto> GetProjectById(int projectId);

        // function to read all project
        public Task<ResponseDto> GetAllProject();

        // function to update project
        public Task<ResponseDto> UpdateProject(ProjectDto projectDto);

        // function to delete project
        public Task<ResponseDto> DeleteProject(int projectId);
    }
}
