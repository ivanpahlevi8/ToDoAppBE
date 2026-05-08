using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface ITeamRoleService
    {
        // function to create team role
        public Task<ResponseDto> CreateTeamRole(TeamRoleDto teamRoleDto);

        // function to get all team role
        public Task<ResponseDto> GetAllTeamRole(int teamId);

        // function to delete team role
        public Task<ResponseDto> DeleteTeamRole(int teamRoleId);
    }
}
