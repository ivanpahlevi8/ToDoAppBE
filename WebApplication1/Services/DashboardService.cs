using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;
        private ResponseDto _responseDto;
        private readonly IMapper _mapper;

        public DashboardService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        public async Task<ResponseDto> GetDashboardProjectSummary(string userId)
        {
            try
            {
                // get total lead project within user
                int totalLeadProject = await _dbContext.Projects.CountAsync(p => p.ProjectUserLeadId == userId);

                // get total member of project from user
                int totalProjectAsMember = await _dbContext.ProjectUserJunction.CountAsync(puj => puj.UserId == userId);

                // get top three project as leader
                List<ProjectModel> topThreeProjectAsLeader = await _dbContext.Projects.Where(p => p.ProjectUserLeadId == userId).OrderByDescending(p => p.CreatedAt).Take(3).ToListAsync();

                // get topp three project as member
                List<ProjectUserJunction> topThreeProjectUserAsMember = await _dbContext.ProjectUserJunction.Where(puj => puj.UserId == userId).OrderByDescending(puj => puj.CreatedAt).Take(3).ToListAsync();

                List<ProjectModel> topThreeProjectAsMember = new List<ProjectModel>();

                foreach(var puj in topThreeProjectUserAsMember)
                {
                    // get project model
                    var getProjectModel = await _dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectId == puj.ProjectId);

                    if (getProjectModel == null)
                    {
                        _responseDto.Message = $"Error when getting project with id {puj.ProjectId}";
                        _responseDto.IsSuccess = false;

                        return _responseDto;
                    }

                    topThreeProjectAsMember.Add(getProjectModel);
                }

                // create project summary dto
                ProjectSummaryDto projectSummary = new ProjectSummaryDto
                {
                    ProjectInvolvedNumber = totalLeadProject + totalProjectAsMember,
                    ProjectLeadNumber = totalLeadProject,
                    ProjectMemberNumber = totalProjectAsMember,
                    TopThreeProjectAsMember = _mapper.Map<List<ProjectDto>>(topThreeProjectAsMember),
                    TopThreeProjectAsLeader = _mapper.Map<List<ProjectDto>>(topThreeProjectAsLeader)
                };

                _responseDto.Message = "Success get project summary";
                _responseDto.Result = projectSummary;
                _responseDto.IsSuccess = true;

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

        public async Task<ResponseDto> GetDashboardTeamSummary(string userId)
        {
            try
            {
                // get team number as user be team leader
                int getTeamNumberAsLeader = await _dbContext.Teams.CountAsync(t => t.TeamLeader == userId);

                // get team number as member
                int getTeamNumberAsMember = await _dbContext.TeamUserJunction.CountAsync(tuj => tuj.UserId ==  userId);

                // get top three team where user as a leader
                List<TeamModel> getTeamAsLeader = await _dbContext.Teams.Where(t => t.TeamLeader == userId).Take(3).ToListAsync();

                // get top three team user as member
                List<TeamUserJunction> getTeamUserJunction = await _dbContext.TeamUserJunction.Where(tuj => tuj.UserId == userId).OrderByDescending(tuj => tuj.CreatedAt).Take(3).ToListAsync();

                List<TeamModel> getTeamAsMember = new List<TeamModel>();

                foreach(var tuj in getTeamUserJunction)
                {
                    TeamModel? getTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.TeamId == tuj.TeamId);

                    if(getTeam == null)
                    {
                        _responseDto.Message = $"Error when getting team with id {tuj.TeamId}";
                        _responseDto.IsSuccess = false;

                        return _responseDto;
                    }

                    getTeamAsMember.Add(getTeam);
                }

                // create team summary dto
                TeamSummaryDto teamSummary = new TeamSummaryDto
                {
                    TeamNumberInvolved = getTeamNumberAsMember,
                    TeamNumberLead = getTeamNumberAsLeader,
                    TopThreeTeamAsLeader = _mapper.Map<List<TeamDto>>(getTeamAsLeader),
                    TopThreeTeamAsMember = _mapper.Map<List<TeamDto>>(getTeamAsMember),
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get team sumary";
                _responseDto.Result = teamSummary;

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

        public async Task<ResponseDto> GetDashboardToDoSummary(string userId)
        {
            try
            {
                // get all to do number
                int toDoTotal = await _dbContext.ToDo.CountAsync(t => t.CreatedById == userId);

                // get all to do with state created
                int toDoOnCreated = await _dbContext.ToDo.CountAsync(t => t.CreatedById == userId && t.ItemState == "CREATED_TO_DO");

                // get all to do with state processed
                int toDoOnProcessed = await _dbContext.ToDo.CountAsync(t => t.CreatedById == userId && t.ItemState == "PROCESSED_TO_DO");

                // get all to do with state finished
                int toDoOnFinished = await _dbContext.ToDo.CountAsync(t => t.CreatedById == userId && t.ItemState == "FINISHED_TO_DO");

                // get top three todo
                List<ToDoModel> topThreeToDo = await _dbContext.ToDo.Where(td => td.CreatedById == userId).OrderByDescending(td => td.CreatedAt).Take(3).ToListAsync();

                // create to do summary dto
                ToDoSummaryDto toDoSummary = new ToDoSummaryDto
                {
                    ToDoNumber = toDoTotal,
                    OnCreateToDoNumber = toDoOnCreated,
                    OnProgressToDoNumber = toDoOnProcessed,
                    OnFinishedToDoNumber = toDoOnFinished,
                    TopThreeToDo = _mapper.Map<List<ToDoDto>>(topThreeToDo),
                };

                _responseDto.Message = "Success get to do summary";
                _responseDto.Result = toDoSummary;
                _responseDto.IsSuccess = true;

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

        public async Task<ResponseDto> GetDashboardConnectionSummary(string userId)
        {
            try
            {
                // get number of requested connection to user
                int numberOfRequestedConnection = await _dbContext.Connections.CountAsync(c => c.UserConnectionId == userId);

                // get number of request connection from user
                int numerOfRequestConnection = await _dbContext.Connections.CountAsync(c => c.UserOwnerId == userId);

                // get number of connection with status user follow
                int numberConnectionUserFollowStatus = await _dbContext.Connections.Where(c => c.UserOwnerId == userId).CountAsync(c => c.ConnectionStatus == SD.CONNECTION_FOLLOW_STATUS);

                // get number to connection with status user is followed
                int numerConnectionUserFollowedStatus = await _dbContext.Connections.Where(c => c.UserConnectionId == userId).CountAsync(c => c.ConnectionStatus == SD.CONNECTION_FOLLOW_STATUS);

                // get number of connection with status connect
                int numberConnectionConnectedStatus = await _dbContext.Connections.Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId).CountAsync(c => c.ConnectionStatus == SD.CONNECTION_CONNECT_STATUS);

                // get number of connection with status user reject
                int numberConnectionUserRejectStatus = await _dbContext.Connections.Where(c => c.UserConnectionId == userId).CountAsync(c => c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS);

                // get number of connection with status rejected by user
                int numberConnectionUserRejectedStatus = await _dbContext.Connections.Where(c => c.UserOwnerId == userId).CountAsync(c => c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS);

                // get number of connection with status disconnected
                int numberConnectionDisconnectedStatus = await _dbContext.Connections.Where(c => c.UserOwnerId == userId || c.UserConnectionId == userId).CountAsync(c => c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS);

                // get top three user connection
                List<ConnectionModel> topThreeConenction = await _dbContext.Connections.Where(c => c.UserOwnerId == userId || c.UserConnectionId == userId).Where(c => c.ConnectionStatus == SD.CONNECTION_CONNECT_STATUS).OrderByDescending(c => c.CreatedAt).Take(3).ToListAsync();

                // create connection summary 
                ConnectionSummaryDto connectionSummary = new ConnectionSummaryDto
                {
                    NumberOfRequestedConnection = numberOfRequestedConnection,
                    NumberOfRequestConnection = numerOfRequestConnection,
                    NumberConnectionStatusUserFollow = numberConnectionUserFollowStatus,
                    NumberConnectionStatusUserFollowed = numerConnectionUserFollowedStatus,
                    NumberConnectionStatusConnect = numberConnectionConnectedStatus,
                    NumberConnectionStatusUserReject = numberConnectionUserRejectStatus,
                    NumberConnectionStatusUserRejected = numberConnectionUserRejectedStatus,
                    NumberConnectionStatusDisconnect = numberConnectionDisconnectedStatus,
                    TopThreeConnection = _mapper.Map<List<ConnectionDto>>(topThreeConenction)
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success getting connection summary";
                _responseDto.Result = connectionSummary;

                return _responseDto;
            } catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
            
            
            throw new NotImplementedException();
        }
    }
}
