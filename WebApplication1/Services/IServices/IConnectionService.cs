using AutoMapper;
using WebApplication1.Data;
using WebApplication1.Models.Dtos;

namespace WebApplication1.Services.IServices
{
    public interface IConnectionService
    {
        public Task<ResponseDto> SendConnection(ConnectionDto connectionDto);

        public Task<ResponseDto> AcceptConnection(int connectionId);

        public Task<ResponseDto> GetRequestedConnection(string requesterId);

        public Task<ResponseDto> GetAllConnectionUser(string userId);
    }
}
