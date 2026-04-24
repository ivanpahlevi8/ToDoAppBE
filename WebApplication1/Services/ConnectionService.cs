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
                IEnumerable<ConnectionModel> allConnection = await _dbContext.Connections.Where(c => (c.UserOwnerId == userId || c.UserConnectionId == userId) && c.ConnectionStatus == SD.CONNECTION_CONNECT_STATUS).ToListAsync();

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
                IEnumerable<ConnectionModel> allRequestedConnection = await _dbContext.Connections.Where(c => c.UserConnectionId == requesterId && c.ConnectionStatus == SD.CONNECTION_FOLLOW_STATUS).ToListAsync();

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

        public async Task<ResponseDto> UnconnectUser(string connectionId)
        {
            try
            {
                ConnectionModel? getConnection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.ConnectionId == Int32.Parse(connectionId));

                if(getConnection == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Connection with id : {connectionId} is not exist";

                    return _responseDto;
                }

                getConnection.ConnectionStatus = SD.CONNECTION_DISCONNECT_STATUS;
                
                _dbContext.Update(getConnection);

                await _dbContext.SaveChangesAsync();

                UserModel? getUser = _dbContext.User.FirstOrDefault(u => u.Id == getConnection.UserConnectionId);

                if (getUser == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"User with id : {getConnection.UserConnectionId} is not exist";

                    return _responseDto;
                }

                ConnectionDto connectionDto = _mapper.Map<ConnectionDto>(getConnection);

                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Disconnect with user : {getUser.UserName}";
                _responseDto.Result = connectionDto;

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

        public async Task<ResponseDto> DeclinedConnection(string connectionId)
        {
            try
            {
                ConnectionModel? getConnection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.ConnectionId == Int32.Parse(connectionId));

                if (getConnection == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Connection with id : {connectionId} is not exist";

                    return _responseDto;
                }

                getConnection.ConnectionStatus = SD.CONNECTION_REJECT_STATUS;

                _dbContext.Update(getConnection);

                await _dbContext.SaveChangesAsync();

                UserModel? getUser = _dbContext.User.FirstOrDefault(u => u.Id == getConnection.UserConnectionId);

                if (getUser == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"User with id : {getConnection.UserConnectionId} is not exist";

                    return _responseDto;
                }

                ConnectionDto connectionDto = _mapper.Map<ConnectionDto>(getConnection);

                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Declined user : {getUser.UserName} connection request";
                _responseDto.Result = connectionDto;

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

        public async Task<ResponseDto> GetAllConnectRejectByToUser(string userId, bool isByUser)
        {
            try
            {
                List<ConnectionModel> connections;

                if(isByUser)
                {
                    connections = await _dbContext.Connections.Where(c => c.UserOwnerId == userId && c.ConnectionStatus == SD.CONNECTION_REJECT_STATUS).ToListAsync();
                } else
                {
                    connections = await _dbContext.Connections.Where(c => c.UserConnectionId == userId && c.ConnectionStatus == SD.CONNECTION_REJECT_STATUS).ToListAsync();
                }

                List<ConnectionDto> connectionDtoList = _mapper.Map<List<ConnectionDto>>(connections);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all connection rejected";
                _responseDto.Result = connectionDtoList;

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

        public async Task<ResponseDto> GetAllDisconnectByToUser(string userId, bool isByUser)
        {
            try
            {
                List<ConnectionModel> connections;

                if (isByUser)
                {
                    connections = await _dbContext.Connections.Where(c => c.UserOwnerId == userId && c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS).ToListAsync();
                }
                else
                {
                    connections = await _dbContext.Connections.Where(c => c.UserConnectionId == userId && c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS).ToListAsync();
                }

                List<ConnectionDto> connectionDtoList = _mapper.Map<List<ConnectionDto>>(connections);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all connection disconnect";
                _responseDto.Result = connectionDtoList;

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

        public async Task<ResponseDto> DeleteConnection(int connectionId)
        {
            try
            {
                ConnectionModel? getConnection = await _dbContext.Connections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

                if(getConnection == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"connection with id {connectionId} is not exist";
                    _responseDto.Result = $"connection with id {connectionId} is not exist";

                    return _responseDto;
                }

                var resulConnection = getConnection;

                _dbContext.Connections.Remove(getConnection);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success remove data";
                _responseDto.Result = resulConnection;

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

        public async Task<ResponseDto> GetAllRequestConnection(string userId)
        {
            try
            {
                IEnumerable<ConnectionModel> getAllConnection = await _dbContext.Connections.Where(c => c.UserOwnerId == userId && c.ConnectionStatus == SD.CONNECTION_FOLLOW_STATUS).ToListAsync();

                IEnumerable<ConnectionDto> connectionDtos = _mapper.Map<IEnumerable<ConnectionDto>>(getAllConnection);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all connection request";
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

        public async Task<ResponseDto> IsConnectedWithUser(string userId, string connectedUserId)
        {
            try
            {
                ConnectionModel? connection = await _dbContext.Connections.FirstOrDefaultAsync(c => (c.UserOwnerId == userId && c.UserConnectionId == connectedUserId) || (c.UserOwnerId == connectedUserId && c.UserConnectionId == userId));

                if (connection == null)
                {
                    _responseDto.Message = "No Connection";
                    _responseDto.Result = false;
                } else
                {
                    _responseDto.Message = "Has Connection";
                    _responseDto.Result = true;
                }

                _responseDto.IsSuccess = true;

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
