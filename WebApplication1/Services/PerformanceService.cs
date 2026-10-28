using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Data;
using WebApplication1.Models.Dtos;
using WebApplication1.Models.Dtos.Performance;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;

        public PerformanceService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

            _responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> ConnectionPerformance(string userId)
        {
            try
            {
                // get current month
                var currentDateTime = DateTime.Now;
                var currentMonthStartDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
                var currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

                // get last month
                var lastMonthStartDate = currentMonthStartDate.AddMonths(-1);
                var lastMonthEndDate = currentMonthEndDate.AddMonths(-1);

                // get connection connected data
                int getConnectionConnectedCurrMonth = await _dbContext.Connections.Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId).Where(c => c.ConnectionStatus == SD.CONNECTION_CONNECT_STATUS).CountAsync(c => c.CreatedAt >= currentMonthStartDate && c.CreatedAt <= currentMonthEndDate);
                int getConnectionConnectedLastMonth = await _dbContext.Connections.Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId).Where(c => c.ConnectionStatus == SD.CONNECTION_CONNECT_STATUS).CountAsync(c => c.CreatedAt >= lastMonthStartDate && c.CreatedAt <= lastMonthEndDate);

                // connection connected percentage
                int connectionConnectedPercentage = 0;
                if(getConnectionConnectedLastMonth < 1)
                {
                    connectionConnectedPercentage = 100;
                } else
                {
                    connectionConnectedPercentage = (getConnectionConnectedCurrMonth = getConnectionConnectedLastMonth) / getConnectionConnectedLastMonth * 100;
                }

                // get connection disconnected data
                int getConnectionDisconnectedCurrMonth = await _dbContext.Connections.Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId).Where(c => c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS).CountAsync(c => c.CreatedAt >= currentMonthStartDate && c.CreatedAt <= currentMonthEndDate);
                int getConnectionDisconnectedLastMonth = await _dbContext.Connections.Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId).Where(c => c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS).CountAsync(c => c.CreatedAt >= lastMonthStartDate && c.CreatedAt <= lastMonthEndDate);

                int connectionDisconnectedPercentage = 0;
                if(getConnectionDisconnectedLastMonth < 1)
                {
                    connectionDisconnectedPercentage = 100;
                } else
                {
                    connectionDisconnectedPercentage = (getConnectionDisconnectedCurrMonth - getConnectionDisconnectedLastMonth) / getConnectionDisconnectedLastMonth * 100;
                }

                // create connection performance
                ConnectionPerformanceDto connectionPerformance = new ConnectionPerformanceDto
                {
                    ConnectionConnectCurrMonth = getConnectionConnectedCurrMonth,
                    ConnectionConnectLastMonth = getConnectionConnectedLastMonth,
                    ConnectionConnectPercentage = connectionConnectedPercentage,
                    ConnectionDisconnectCurrMonth = getConnectionDisconnectedCurrMonth,
                    ConnectionDisconnectLastMonth = getConnectionDisconnectedLastMonth,
                    ConnectionDisconnectPercentage = connectionDisconnectedPercentage,
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success getting connection performance";
                _responseDto.Result = connectionPerformance;

                return _responseDto;
            } catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException?.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> ProjectPerformance(string userId)
        {
            try
            {
                // get current month
                var currentDateTime = DateTime.Now;
                var currentMonthStartDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
                var currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

                // get last month
                var lastMonthStartDate = currentMonthStartDate.AddMonths(-1);
                var lastMonthEndDate = currentMonthEndDate.AddMonths(-1);

                // get number project lead 
                int numberProjectLeadCurrMonth = await _dbContext.Projects.Where(p => p.ProjectUserLeadId == userId).CountAsync(p => p.CreatedAt >= currentMonthStartDate && p.CreatedAt <= currentMonthEndDate);
                int numberProjectLeadLastMonth = await _dbContext.Projects.Where(p => p.ProjectUserLeadId == userId).CountAsync(p => p.CreatedAt >= lastMonthStartDate && p.CreatedAt <= lastMonthEndDate);
                int numberProjectLeadPercentage = 0;
                if(numberProjectLeadLastMonth < 1)
                {
                    numberProjectLeadPercentage = 100;
                } else
                {
                    numberProjectLeadPercentage = (numberProjectLeadCurrMonth - numberProjectLeadLastMonth) / numberProjectLeadLastMonth * 100;
                }

                // get number project involved
                int numberProjectInvolvedCurrMonth = await _dbContext.ProjectUserJunction.Where(puj => puj.UserId  == userId).CountAsync(puj => puj.CreatedAt >= currentMonthStartDate && puj.CreatedAt <= currentMonthEndDate);
                int numberProjectInvolvedLastMonth = await _dbContext.ProjectUserJunction.Where(puj => puj.UserId == userId).CountAsync(puj => puj.CreatedAt >= lastMonthStartDate && puj.CreatedAt <= lastMonthEndDate);
                int numberProjectInvolvedPercentage = 0;
                if(numberProjectInvolvedLastMonth < 1)
                {
                    numberProjectInvolvedPercentage = 100;
                } else
                {
                    numberProjectInvolvedPercentage = (numberProjectInvolvedCurrMonth - numberProjectInvolvedLastMonth) / numberProjectInvolvedLastMonth * 100;
                }


                // create project performance dto
                ProjectPerformanceDto projectPerformance = new ProjectPerformanceDto
                {
                    NumberProjectLeadsCurrMonth = numberProjectLeadCurrMonth,
                    NumberProjectLeadsLastMonth = numberProjectLeadLastMonth,
                    NumberProjectLeadsPercentage = numberProjectLeadPercentage,
                    NumberProjectInvolvedCurrMonth = numberProjectInvolvedCurrMonth,
                    NumberProjectInvolvedLastMonth = numberProjectInvolvedLastMonth,
                    NumberProjectInvolvedPercentage = numberProjectInvolvedPercentage,
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get project performance";
                _responseDto.Result = projectPerformance;

                return _responseDto;
            }
            catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException?.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> TeamPerformance(string userId)
        {
            try
            {
                // get current month
                var currentDateTime = DateTime.Now;
                var currentMonthStartDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
                var currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

                // get last month
                var lastMonthStartDate = currentMonthStartDate.AddMonths(-1);
                var lastMonthEndDate = currentMonthEndDate.AddMonths(-1);

                // get number team lead
                int numberTeamLeadsCurrMonth = await _dbContext.Teams.Where(t => t.TeamLeader == userId).CountAsync(t => t.CreatedAt >= currentMonthStartDate && t.CreatedAt <= currentMonthEndDate);
                int numberTeamLeadsLastMonth = await _dbContext.Teams.Where(t => t.TeamLeader == userId).CountAsync(t => t.CreatedAt >= lastMonthStartDate && t.CreatedAt <= lastMonthEndDate);

                // calculate percentage number team leads
                int numberTeamLeadsPercentage = 0;
                if(numberTeamLeadsLastMonth < 1)
                {
                    numberTeamLeadsPercentage = 100;
                } else
                {
                    numberTeamLeadsPercentage = (numberTeamLeadsCurrMonth - numberTeamLeadsLastMonth) / numberTeamLeadsLastMonth * 100;
                }

                // get number team involved
                int numberTeamInvolvedCurrMonth = await _dbContext.TeamUserJunction.Where(tuj => tuj.UserId == userId).CountAsync(tuj => tuj.CreatedAt >= currentMonthStartDate && tuj.CreatedAt <= currentMonthEndDate);
                int numberTeamInvolvedLastMonth = await _dbContext.TeamUserJunction.Where(tuj => tuj.UserId == userId).CountAsync(tuj => tuj.CreatedAt >= lastMonthStartDate && tuj.CreatedAt <= lastMonthEndDate);

                // calculate percentage number team involved
                int numberTeamInvolvedPercentage = 0;
                if(numberTeamInvolvedLastMonth < 1)
                {
                    numberTeamInvolvedPercentage = 100;
                } else
                {
                    numberTeamInvolvedPercentage = (numberTeamInvolvedCurrMonth - numberTeamInvolvedLastMonth) / numberTeamInvolvedLastMonth * 100;
                }

                // create team performance
                TeamPerformanceDto teamPerformance = new TeamPerformanceDto
                {
                    NumberTeamLeadsCurrMonth = numberTeamLeadsCurrMonth,
                    NumberTeamLeadsLastMonth = numberTeamLeadsLastMonth,
                    NumberTeamLeadsPercentage = numberTeamLeadsPercentage,
                    NumberTeamInvolvedCurrMonth = numberTeamInvolvedCurrMonth,
                    NumberTeamInvolvedLastMonth = numberTeamInvolvedLastMonth,
                    NumberTeamInvolvedPercentage = numberTeamInvolvedPercentage
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success getting team performance";
                _responseDto.Result = teamPerformance;

                return _responseDto;
            } catch (Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException?.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> ToDoPerformance(string userId)
        {
            try
            {
                // get current month
                var currentDateTime = DateTime.Now;
                var currentMonthStartDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
                var currentMonthEndDate = currentMonthStartDate.AddMonths(1).AddDays(-1);

                // get last month
                var lastMonthStartDate = currentMonthStartDate.AddMonths(-1);
                var lastMonthEndDate = currentMonthEndDate.AddMonths(-1);

                // get number of to do on created for current month and last month
                int numberToDoOnCreatedCurrMonth = await _dbContext.ToDo.Where(td => td.ItemState == "CREATED_TO_DO").CountAsync(td => td.CreatedAt >= currentMonthStartDate && td.CreatedAt <= currentMonthEndDate);
                int numberToDoOnCreatedLastMonth = await _dbContext.ToDo.Where(td => td.ItemState == "CREATED_TO_DO").CountAsync(td => td.CreatedAt >= lastMonthStartDate && td.CreatedAt <= lastMonthEndDate);
                int numberToDoCreatedPercentage = 0;
                if (numberToDoOnCreatedLastMonth < 1)
                {
                    numberToDoCreatedPercentage = 100;
                } else
                {
                    numberToDoCreatedPercentage = (numberToDoOnCreatedCurrMonth - numberToDoOnCreatedLastMonth) / numberToDoOnCreatedLastMonth * 100;
                }

                // get number of to do on created for current month and last month
                int numberToDoOnProcessedCurrMonth = await _dbContext.ToDo.Where(td => td.ItemState == "PROCESSED_TO_DO").CountAsync(td => td.CreatedAt >= currentMonthStartDate && td.CreatedAt <= currentMonthEndDate);
                int numberToDoOnProcessedLastMonth = await _dbContext.ToDo.Where(td => td.ItemState == "PROCESSED_TO_DO").CountAsync(td => td.CreatedAt >= lastMonthStartDate && td.CreatedAt <= lastMonthEndDate);
                int numberToDoProcessedPercentage = 0;
                if(numberToDoOnProcessedLastMonth < 1)
                {
                    numberToDoProcessedPercentage = 100;
                } else
                {
                    numberToDoProcessedPercentage = (numberToDoOnProcessedCurrMonth - numberToDoOnProcessedLastMonth) / numberToDoOnProcessedLastMonth * 100;
                }

                // get number of to do on created for current month and last month
                int numberToDoFinishedCurrMonth = await _dbContext.ToDo.Where(td => td.ItemState == "FINISHED_TO_DO").CountAsync(td => td.CreatedAt >= currentMonthStartDate && td.CreatedAt <= currentMonthEndDate);
                int numberToDoFinisheddLastMonth = await _dbContext.ToDo.Where(td => td.ItemState == "FINISHED_TO_DO").CountAsync(td => td.CreatedAt >= lastMonthStartDate && td.CreatedAt <= lastMonthEndDate);
                int numberToDoFinishedPercentage = 0;
                if (numberToDoFinisheddLastMonth < 1)
                {
                    numberToDoFinishedPercentage = 100;
                } else
                {
                    numberToDoFinishedPercentage = (numberToDoFinishedCurrMonth - numberToDoFinisheddLastMonth) / numberToDoFinisheddLastMonth * 100;
                }

                // create todo performance dto
                ToDoPerformanceDto toDoPerformance = new ToDoPerformanceDto
                {
                    NumberToDoCreatedThisMonth = numberToDoOnCreatedCurrMonth,
                    NumberToDoCreatedLastMonth = numberToDoOnCreatedLastMonth,
                    PercentageToDoCreated = numberToDoCreatedPercentage,
                    NumberToDoOnProgressThisMonth = numberToDoOnProcessedCurrMonth,
                    NumberToDoOnProgressLastMonth = numberToDoOnProcessedLastMonth,
                    PercentageToDoOnProgress = numberToDoProcessedPercentage,
                    NumberToDoDoneThisMonth = numberToDoFinishedCurrMonth,
                    NumberToDoDoneLastMonth = numberToDoFinisheddLastMonth,
                    PercentageToDoDone = numberToDoFinishedPercentage
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get todo performance";
                _responseDto.Result = toDoPerformance;

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
    }
}
