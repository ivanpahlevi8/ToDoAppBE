using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class TeamRoleService : ITeamRoleService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private ResponseDto _responseDto;

        public TeamRoleService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        public async Task<ResponseDto> CreateTeamRole(TeamRoleDto teamRoleDto)
        {
            try
            {
                // update created at
                teamRoleDto.CreatedAt = DateTime.Now;

                // map into model
                TeamRoleModel teamRoleModel = _mapper.Map<TeamRoleModel>(teamRoleDto);

                // add to db
                await _dbContext.AddAsync(teamRoleModel);

                // save db change
                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success inserting team role";
                _responseDto.Result = teamRoleDto;

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

        public async Task<ResponseDto> DeleteTeamRole(int teamRoleId)
        {
            try
            {
                // get team role by id
                TeamRoleModel? teamRoleModel = await _dbContext.TeamRoles.FirstOrDefaultAsync(tr => tr.TeamRoleId == teamRoleId);

                if(teamRoleModel == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"team role with id {teamRoleId} is not exist";

                    return _responseDto;
                }

                _dbContext.Remove(teamRoleModel);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success deleting item";
                _responseDto.Result = $"Success delete team role with id : {teamRoleId}";

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

        public async Task<ResponseDto> GetAllTeamRole(int teamId)
        {
            try
            {
                IEnumerable<TeamRoleModel> teamRoleModel = await _dbContext.TeamRoles.Where(tr => tr.TeamId == teamId).ToListAsync();

                IEnumerable<TeamRoleDto> teamRoleDtos = _mapper.Map<IEnumerable<TeamRoleDto>>(teamRoleModel);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get all team role";
                _responseDto.Result = teamRoleDtos;

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
