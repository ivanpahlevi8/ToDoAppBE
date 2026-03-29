using WebApplication1.Models;
using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface ITeamService
    {
        public Task<ResponseDto> CreateTeam(TeamDto teamDto);

        public Task<ResponseDto> GetTeam(int teamId);

        public Task<ResponseDto> GetAllTeam();

        public Task<ResponseDto> UpdateTeam(TeamDto teamDto);

        public Task<ResponseDto> DeleteTeam(int teamId);

        public Task<ResponseDto> AssignUserToTeam(string userId, int teamId);

        public Task<ResponseDto> UnAssignedUserToTeam(string userId, int teamId);

        public Task<ResponseDto> GetAllTeamMember(int teamId);
    }
}
