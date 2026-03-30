using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private ResponseDto _responseDto;

        public ConnectionService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> AcceptConnection(int connectionId)
        {
            try
            {
                ConnectionModel? connectionModel = await _dbContext.Connections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if (connectionModel == null)
                {
                    _responseDto.Message = $"Connection with id {connectionId} is not exist";
                    _responseDto.IsSuccess = false;

                    return _responseDto;
                }

                connectionModel.ConnectionStatus = SD.CONNECTION_CONNECT_STATUS;

                _dbContext.Connections.Update(connectionModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success accept the connection";
                _responseDto.Result = _mapper.Map<ConnectionDto>(connectionModel);

                return _responseDto;
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> GetAllConnectionUser(string userId)
        {
            try
            {
                IEnumerable<ConnectionModel> allConnection = await _dbContext.Connections.Where(c => c.UserOwnerId == userId || c.UserConnectionId == userId).ToListAsync();

                IEnumerable<ConnectionDto> allConnectionDto = _mapper.Map<IEnumerable<ConnectionDto>>(allConnection);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all connection user";
                _responseDto.Result = allConnectionDto;

                return _responseDto;
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> GetRequestedConnection(string requesterId)
        {
            try
            {
                IEnumerable<ConnectionModel> allRequestedConnection = await _dbContext.Connections.Where(c => c.UserConnectionId == requesterId).Where(c => c.ConnectionStatus == SD.CONNECTION_FOLLOW_STATUS).ToListAsync();

                IEnumerable<ConnectionDto> connectionDtos = _mapper.Map<IEnumerable<ConnectionDto>>(allRequestedConnection);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get requested connection";
                _responseDto.Result = connectionDtos;

                return _responseDto;
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> SendConnection(ConnectionDto connectionDto)
        {
            try
            {
                ConnectionModel connectionModel = _mapper.Map<ConnectionModel>(connectionDto);

                // initial status of connection is following
                connectionModel.ConnectionStatus = SD.CONNECTION_FOLLOW_STATUS;

                await _dbContext.Connections.AddAsync(connectionModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success send connection";
                _responseDto.Result = "Success send connection";

                return _responseDto;
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }
    }
}
