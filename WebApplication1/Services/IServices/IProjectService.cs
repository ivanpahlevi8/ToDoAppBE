using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface IProjectService
    {
        // craete function to create project
        public Task<ResponseDto> CreateProject(ProjectDto projectDto);

        // function to read object by id
        public Task<ResponseDto> GetProjectById(int projectId);

        // function to get all project based on user lead
        public Task<ResponseDto> GetProjectByUserLead(string userId);

        // function to get all project based on user member
        public Task<ResponseDto> GetProjectByUserMember(string userId);

        // function to read all project
        public Task<ResponseDto> GetAllProject();

        // function to update project
        public Task<ResponseDto> UpdateProject(ProjectDto projectDto);

        // function to delete project
        public Task<ResponseDto> DeleteProject(int projectId);

        // function to get all todo within project
        public Task<ResponseDto> GetProjectToDos(int projecyId);
    }
}
