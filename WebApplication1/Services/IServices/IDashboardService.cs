using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface IDashboardService
    {
        // function to get total todo user, total on create, on progress, and on done
        public Task<ResponseDto> GetDashboardToDoSummary(string userId);

        // function to get summary of project (total project, total project that on progress, and total project done)
        public Task<ResponseDto> GetDashboardProjectSummary(string userId);

        // function to get summary of team(number team involved, and number team leading)
        public Task<ResponseDto> GetDashboardTeamSummary(string userId);
    }
}
