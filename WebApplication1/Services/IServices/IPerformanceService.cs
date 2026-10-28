using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface IPerformanceService
    {
        // function to get to do performance
        public Task<ResponseDto> ToDoPerformance(string userId);

        // function to get project performance
        public Task<ResponseDto> ProjectPerformance(string userId);

        // function to get team performance
        public Task<ResponseDto> TeamPerformance(string userId);

        // function to get connection performance
        public Task<ResponseDto> ConnectionPerformance(string userId);
    }
}
