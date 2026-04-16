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

        public Task<ResponseDto> UnconnectUser(string connectionId);
        public Task<ResponseDto> DeclinedConnection(string connectionId);
        public Task<ResponseDto> GetAllConnectRejectByToUser(string userId, bool isByUser);
        public Task<ResponseDto> GetAllDisconnectByToUser(string userId, bool isByUser);
    }
}
