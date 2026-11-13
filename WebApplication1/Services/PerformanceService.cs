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

        public async Task<ResponseDto> getConnectionPerformance(string userId, int year)
        {
            try
            {
                // get start and end date
                var start = new DateTime(year: year, month: 1, day: 1);
                var end = start.AddYears(1);

                // get connection connected
                var getConnectionConnected = await _dbContext.Connections
                    .Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId)
                    .Where(c => c.ConnectionStatus == SD.CONNECTION_CONNECT_STATUS)
                    .Where(c => c.CreatedAt >= start && c.CreatedAt <= end)
                    .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                    .Select(c => new ChartObjectDto { Year = c.Key.Year, Month = c.Key.Month, Count = c.Count() })
                    .ToListAsync();

                // get connection disconnected
                var getConnectionDisconnect = await _dbContext.Connections
                    .Where(c => c.UserConnectionId == userId || c.UserOwnerId == userId)
                    .Where(c => c.ConnectionStatus == SD.CONNECTION_DISCONNECT_STATUS)
                    .Where(c => c.CreatedAt >= start && c.CreatedAt <= end)
                    .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                    .Select(c => new ChartObjectDto { Year = c.Key.Year, Month = c.Key.Month, Count = c.Count() })
                    .ToListAsync();

                // create result for each
                var connectionConnectedResult = new List<ChartObjectDto>();
                var connectionDisconnectedResult = new List<ChartObjectDto>();

                for(int i = 1; i <= 12; i++)
                {
                    var isConnectionConnectedExist = getConnectionConnected.Any(c => c.Month == i);
                    var isConnectionDisconnectExist = getConnectionDisconnect.Any(c => c.Month == i);

                    if(!isConnectionConnectedExist || !isConnectionDisconnectExist)
                    {
                        var chartData = new ChartObjectDto
                        {
                            Year = year,
                            Month = i,
                            Count = 0,
                        };

                        if(!isConnectionConnectedExist)
                        {
                            connectionConnectedResult.Add(chartData);
                        }
                        
                        if(!isConnectionDisconnectExist)
                        {
                            connectionDisconnectedResult.Add(chartData);
                        }
                    } else
                    {
                        var getConnected = getConnectionConnected.Where(c => c.Month == i).FirstOrDefault()!;
                        var getDisconnect = getConnectionDisconnect.Where(c => c.Month == i).FirstOrDefault()!;

                        if(isConnectionConnectedExist)
                        {
                            connectionConnectedResult.Add(getConnected);
                        }
                        
                        if(isConnectionDisconnectExist)
                        {
                            connectionDisconnectedResult.Add(getDisconnect);
                        }
                    }
                }

                // create response
                ConnectionChartPeformanceDto connectionChart = new ConnectionChartPeformanceDto
                {
                    ConnectionConnectedChart = connectionConnectedResult,
                    ConnectionDisconnectedChart = connectionDisconnectedResult,
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get data";
                _responseDto.Result = connectionChart;

                return _responseDto;
            } catch(Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException?.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> GetProjectPerformanceChart(string userId, int year)
        {
            try
            {
                // create current year date
                var startDate = new DateTime(year, 1, 1);
                var endDate = startDate.AddYears(1);

                // get project as leader
                var getProjectAsLeader = await _dbContext.Projects
                    .Where(p => p.ProjectUserLeadId == userId)
                    .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                    .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                    .Select(grp => new ChartObjectDto { Year=grp.Key.Year, Month=grp.Key.Month, Count=grp.Count() })
                    .ToListAsync();

                // create project as a leader result
                var getProjectAsLeaderResult = new List<ChartObjectDto>();

                for(int i = 0; i <= 12; i++)
                {
                    // check is month exist on list
                    bool isExist = getProjectAsLeader.Any(p => p.Month == i);

                    if(!isExist)
                    {
                        var chartObject = new ChartObjectDto { Year = year, Month = i, Count = 0 };

                        getProjectAsLeaderResult.Add(chartObject);
                    } else
                    {
                        // get object
                        var getChart = getProjectAsLeader.Where(p => p.Month == i).FirstOrDefault()!;

                        getProjectAsLeaderResult.Add(getChart);
                    }
                }

                // get project as member
                var getProjectAsmember = await _dbContext.ProjectUserJunction
                    .Where(puj => puj.UserId == userId)
                    .Where(puj => puj.CreatedAt >= startDate && puj.CreatedAt <= endDate)
                    .GroupBy(puj => new { puj.CreatedAt.Year, puj.CreatedAt.Month })
                    .Select(puj => new ChartObjectDto { Year = puj.Key.Year, Month = puj.Key.Month, Count = puj.Count() })
                    .ToListAsync();

                // create project as member result
                var getProjectAsMemberResult = new List<ChartObjectDto>();

                for(int i = 1; i <= 12; i++)
                {
                    // check if data exist or not
                    bool isDataExist = getProjectAsmember.Any(d => d.Month == i);

                    if(!isDataExist)
                    {
                        var chartObject = new ChartObjectDto
                        {
                            Year = year,
                            Month = i,
                            Count = 0,
                        };

                        getProjectAsMemberResult.Add(chartObject);
                    } else
                    {
                        // get month data
                        var getData = getProjectAsmember.Where(d => d.Month == i).FirstOrDefault()!;

                        getProjectAsMemberResult.Add(getData);
                    }
                }

                // create project chart performance
                ProjectChartPerformanceDto projectChartPerformance = new ProjectChartPerformanceDto();
                projectChartPerformance.ProjectAsMember = getProjectAsMemberResult;
                projectChartPerformance.ProjectLeads = getProjectAsLeaderResult;

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Succes getting project chart";
                _responseDto.Result = projectChartPerformance;

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

        public async Task<ResponseDto> GetTeamPerformanceChart(string userId, int year)
        {
            try
            {
                // create start and end date
                var start = new DateTime(year: year, month: 1, day: 1);
                var end = start.AddYears(1);

                // get all team as leader
                var getAllTeamAsLeader = await _dbContext.Teams
                    .Where(t => t.TeamLeader == userId)
                    .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                    .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                    .Select(t => new ChartObjectDto { Year = t.Key.Year, Month = t.Key.Month, Count = t.Count() })
                    .ToListAsync();

                var getAllTeamAsLeaderResult = new List<ChartObjectDto>();


                // get all team as member
                var getAllTeamAsMember = await _dbContext.TeamUserJunction
                    .Where(tuj => tuj.UserId == userId)
                    .Where(tuj => tuj.CreatedAt >= start && tuj.CreatedAt <= end)
                    .GroupBy(tuj => new { tuj.CreatedAt.Year, tuj.CreatedAt.Month })
                    .Select(tuj => new ChartObjectDto { Year = tuj.Key.Year, Month = tuj.Key.Month, Count = tuj.Count() })
                    .ToListAsync();

                var getAllTeamAsMemberResult = new List<ChartObjectDto>();

                for (int i = 1; i <= 12; i++)
                {
                    bool isDataExist = getAllTeamAsLeader.Any(t => t.Month == i);

                    if (!isDataExist)
                    {
                        var chartData = new ChartObjectDto
                        {
                            Year = year,
                            Month = i,
                            Count = 0,
                        };

                        getAllTeamAsLeaderResult.Add(chartData);
                    }
                    else
                    {
                        var getDataLead = getAllTeamAsLeader.Where(t => t.Month == i).FirstOrDefault()!;

                        getAllTeamAsLeaderResult.Add(getDataLead);
                    }

                    isDataExist = getAllTeamAsMember.Any(t => t.Month == i);

                    if (!isDataExist)
                    {
                        var chartData = new ChartObjectDto
                        {
                            Year = year,
                            Month = i,
                            Count = 0,
                        };

                        getAllTeamAsMemberResult.Add(chartData);
                    }
                    else
                    {
                        var getDataMember = getAllTeamAsMember.Where(t => t.Month == i).FirstOrDefault()!;

                        getAllTeamAsMemberResult.Add(getDataMember);
                    }
                }

                // create result
                TeamChartPerformanceDto teamChart = new TeamChartPerformanceDto
                {
                    TeamNumberAsLead = getAllTeamAsLeaderResult,
                    TeamNumberAsMember = getAllTeamAsMemberResult,
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get result";
                _responseDto.Result = teamChart;

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

        public async Task<ResponseDto> GetToDoPerformanceChart(string userId, int year)
        {
            try
            {
                // create start and end date range
                var start = new DateTime(year, 1, 1);
                var end = start.AddYears(1);

                // get on created todo
                var onCreatedToDoData = await _dbContext.ToDo
                    .Where(t => t.CreatedById == userId)
                    .Where(t => t.ItemState == "CREATED_TO_DO") // CREATED_TO_DO  PROCESSED_TO_DO
                    .Where(t => t.CreatedAt >= start && t.CreatedAt < end)
                    .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                    .Select(t => new ChartObjectDto { Year=t.Key.Year, Month=t.Key.Month, Count=t.Count()})
                    .ToListAsync();

                var resultList = new List<ChartObjectDto>();

                // loop through 12 month
                for(int i = 1; i <= 12; i++)
                {
                    // create date time
                    var dateTime = new DateTime(year, i, 1);

                    // check if month exist on created to do data
                    bool isMonthExist = onCreatedToDoData.Any(dt => dt.Month == i);

                    if(!isMonthExist)
                    {
                        resultList.Add(new ChartObjectDto { Year = dateTime.Year, Month = dateTime.Month, Count = 0 });
                    } else
                    {
                        // get the data
                        var getData = onCreatedToDoData.Where(dt => dt.Month == i).FirstOrDefault()!;

                        resultList.Add(getData);
                    }
                }

                // get on processed to do
                var onProcessedToDoData = await _dbContext.ToDo
                    .Where(t => t.CreatedById == userId)
                    .Where(t => t.ItemState == "PROCESSED_TO_DO")
                    .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                    .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                    .Select(t => new ChartObjectDto { Year = t.Key.Year, Month = t.Key.Month, Count = t.Count() })
                    .ToListAsync();

                var onProcessedToDoDataResult = new List<ChartObjectDto>();

                for(int i = 1; i <= 12; i++)
                {
                    bool isDataExist = onProcessedToDoData.Any(d => d.Month == i);

                    if(!isDataExist)
                    {
                        var chartData = new ChartObjectDto
                        {
                            Year = year,
                            Month = i,
                            Count = 0,
                        };

                        onProcessedToDoDataResult.Add(chartData);
                    } else
                    {
                        var getData = onProcessedToDoData.Where(d => d.Month == i).FirstOrDefault()!;

                        onProcessedToDoDataResult.Add(getData);
                    }
                }

                // get on finished to do
                var onFinishedToDoData = await _dbContext.ToDo
                    .Where(t => t.CreatedById == userId)
                    .Where(t => t.ItemState == "FINISHED_TO_DO")
                    .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
                    .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
                    .Select(t => new ChartObjectDto { Year = t.Key.Year, Month = t.Key.Month, Count = t.Count() })
                    .ToListAsync();

                var onFinishedToDoDataResult = new List<ChartObjectDto>();

                for(int i = 1; i <= 12; i++)
                {
                    bool isDataExist = onFinishedToDoData.Any(d => d.Month == i);

                    if(!isDataExist)
                    {
                        var chartData = new ChartObjectDto
                        {
                            Year = year,
                            Month = i,
                            Count = 0,
                        };

                        onFinishedToDoDataResult.Add(chartData);
                    } else
                    {
                        var getData = onFinishedToDoData.Where(d => d.Month == i).FirstOrDefault()!;

                        onFinishedToDoDataResult.Add(getData);
                    }
                }

                // create result object
                ToDoChartPeformanceDto toDoChart = new ToDoChartPeformanceDto
                {
                    ToDoOnCreatedYear = resultList,
                    ToDoOnProgressedYear = onProcessedToDoDataResult,
                    ToDoOnFinishedYear = onFinishedToDoDataResult,
                };

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success";
                _responseDto.Result = toDoChart;

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
