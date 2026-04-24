using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private ResponseDto _responseDto;

        public TeamService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> AssignUserToTeam(string userId, int teamId)
        {
            try
            {
                TeamUserJunction teamUserJunction = new TeamUserJunction
                {
                    UserId = userId,
                    TeamId = teamId,
                };

                await _dbContext.TeamUserJunction.AddAsync(teamUserJunction);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success assign user to team";
                _responseDto.Result = teamUserJunction;

                return _responseDto;
            } catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> CreateTeam(TeamDto teamDto)
        {
            try
            {
                teamDto.CreatedAt = DateTime.Now;

                TeamModel teamModel = _mapper.Map<TeamModel>(teamDto);

                await _dbContext.Teams.AddAsync(teamModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success create new team";
                _responseDto.Result = teamDto;

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

        public async Task<ResponseDto> DeleteTeam(int teamId)
        {
            try
            {
                TeamModel? getTeamModel = await _dbContext.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);

                if(getTeamModel == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Error happen: Team with id {teamId} is not exist";
                    _responseDto.Result = null;

                    return _responseDto;
                }

                _dbContext.Teams.Remove(getTeamModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success delete team";
                _responseDto.Result = "Success delete team";

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

        public async Task<ResponseDto> GetAllTeam(string userId)
        {
            try
            {
                // get all team when user be a leader
                List<TeamModel> teamWhereUserLeader = await _dbContext.Teams.Where(t => t.TeamLeader == userId).ToListAsync();

                // get all team when user is be a member
                IEnumerable<TeamUserJunction> getTeamUserJunction = await _dbContext.TeamUserJunction.Include(tuj => tuj.Team).Where(tuj => tuj.UserId == userId).ToListAsync();

                List<TeamModel> teamWhereUserMember = getTeamUserJunction.Select(tuj => tuj.Team).ToList();

                // combine
                List<TeamModel> combinedData = teamWhereUserLeader.Concat(teamWhereUserMember).ToList();

                IEnumerable<TeamDto> alLTeamDto = _mapper.Map<IEnumerable<TeamDto>>(combinedData);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all team";
                _responseDto.Result = alLTeamDto;

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

        public async Task<ResponseDto> GetAllTeamMember(int teamId)
        {
            try
            {
                TeamModel? getTeam = await _dbContext.Teams.Include(t => t.TeamUserJunction).FirstOrDefaultAsync(t => t.TeamId == teamId);

                if(getTeam == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Team with id {teamId} is not exist";

                    return _responseDto;
                }

                var allUserTeam = await Task.WhenAll(
                    getTeam.TeamUserJunction.Select(t =>
                        _dbContext.Users.FirstOrDefaultAsync(u => u.Id == t.UserId)
                    )
                );

                List<UserModel> result = allUserTeam.Where(u => u != null).ToList();

                TeamDto teamDto = _mapper.Map<TeamDto>(getTeam);
                teamDto.UserMember = _mapper.Map<List<UserDto>>(result);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success getting team with member";
                _responseDto.Result = teamDto;

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

        public async Task<ResponseDto> GetTeam(int teamId)
        {
            try
            {
                TeamModel? getTeamModel = await _dbContext.Teams.Include(t => t.TeamUserJunction).ThenInclude(tuj => tuj.User).FirstOrDefaultAsync(t => t.TeamId == teamId);

                if(getTeamModel == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Team with id : {teamId} is not exist";

                    return _responseDto;
                }

                List<UserModel> teamUserMember = getTeamModel.TeamUserJunction.Select(tuj => tuj.User).ToList();

                TeamDto teamDto = _mapper.Map<TeamDto>(getTeamModel);
                teamDto.UserMember = _mapper.Map<List<UserDto>>(teamUserMember);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get team by id";
                _responseDto.Result = teamDto;

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

        public async Task<ResponseDto> UnAssignedUserToTeam(string userId, int teamId)
        {
            try
            {
                TeamUserJunction? teamUserJunction = await _dbContext.TeamUserJunction.FirstOrDefaultAsync(tuj => tuj.TeamId == teamId && tuj.UserId == userId);

                if(teamUserJunction == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"User with id {userId} is not exist on team {teamId}";
                    
                    return _responseDto;
                }

                _responseDto.IsSuccess= true;
                _responseDto.Message = "Success unassigned user from team";
                _responseDto.Result = "Success unassigned user from team";

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

        public async Task<ResponseDto> UpdateTeam(TeamDto teamDto)
        {
            try
            {
                TeamModel teamModel = _mapper.Map<TeamModel>(teamDto);

                _dbContext.Teams.Update(teamModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success update team";
                _responseDto.Result = teamModel;

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
