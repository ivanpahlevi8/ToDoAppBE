using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class ProjectService : IProjectService
    {
        // instance of db context
        private readonly AppDbContext _dbContext;
        private ResponseDto _responseDto;
        private readonly IMapper _mapper;

        public ProjectService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        public async Task<ResponseDto> CreateProject(ProjectDto projectDto)
        {
            try
            {
                // create map from dto into project
                ProjectModel project = _mapper.Map<ProjectModel>(projectDto);

                project.CreatedAt = DateTime.Now;

                await _dbContext.Projects.AddAsync(project);

                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success insert project";
                _responseDto.Result = "Success insert project";

                return _responseDto;
            } catch(Exception ex)
            {
                string errMsg = "Error Happen : " + ex.Message + ", " + ex.InnerException.Message;
                _responseDto.IsSuccess = false;
                _responseDto.Message = errMsg;
                _responseDto.Result = null;

                return _responseDto;
            }
        }

        public async Task<ResponseDto> DeleteProject(int projectId)
        {
            try
            {
                ProjectModel? getProject = await _dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

                if(getProject == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Project with id : {projectId} is not exist";

                    return _responseDto;
                }

                _dbContext.Projects.Remove(getProject);
                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess = true;
                _responseDto.Message = $"Success delete project with name {getProject.ProjectName}";
                _responseDto.Result = $"Success delete project with name {getProject.ProjectName}";

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

        public async Task<ResponseDto> GetAllProject()
        {
            try
            {
                List<ProjectModel> allProject = await _dbContext.Projects.ToListAsync();

                List<ProjectDto> allProjectDto = _mapper.Map<List<ProjectDto>>(allProject);

                _responseDto.IsSuccess= true;
                _responseDto.Message = "Success get all project";
                _responseDto.Result = allProjectDto;

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

        public async Task<ResponseDto> GetProjectById(int projectId)
        {
            try
            {
                ProjectModel? getProject = await _dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

                if(getProject == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Project with id : {projectId} is not exist";

                    return _responseDto;
                }

                ProjectDto project = _mapper.Map<ProjectDto>(getProject);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get project by id";
                _responseDto.Result = project;

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

        public async Task<ResponseDto> UpdateProject(ProjectDto projectDto)
        {
            try
            {
                ProjectModel? getProject = await _dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectDto.ProjectId);

                if (getProject == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Project with id : {projectDto.ProjectId} is not exist";

                    return _responseDto;
                }

                _dbContext.Projects.Update(getProject);
                await _dbContext.SaveChangesAsync();

                _responseDto.IsSuccess= true;
                _responseDto.Message = "Success update project";
                _responseDto.Result = projectDto;

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

        public async Task<ResponseDto> GetProjectToDos(int projectId)
        {
            try
            {
                ProjectModel? getProject = await _dbContext.Projects.Include(p => p.ToDos).FirstOrDefaultAsync(p => p.ProjectId == projectId);

                if(getProject == null)
                {
                    _responseDto.IsSuccess = false;
                    _responseDto.Message = $"Project with id {projectId} is not exist";
                    _responseDto.Result = getProject;

                    return _responseDto;
                }

                ProjectDto projectDto = _mapper.Map<ProjectDto>(getProject);
                projectDto.ToDos = getProject.ToDos;

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get project todo";
                _responseDto.Result = projectDto;

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

        public async Task<ResponseDto> GetProjectByUserLead(string userId)
        {
            try
            {
                IEnumerable<ProjectModel> allProject = await _dbContext.Projects.Where(p => p.ProjectUserLeadId == userId).ToListAsync();

                IEnumerable<ProjectDto> projectDto = _mapper.Map<IEnumerable<ProjectDto>>(allProject);

                _responseDto.IsSuccess = true;
                _responseDto.Message = "Success get project by user lead";
                _responseDto.Result = projectDto;

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

        public async Task<ResponseDto> GetProjectByUserMember(string userId)
        {
            try
            {
                // get all junction project user by user id
                List<ProjectUserJunction> allUserJunction = await _dbContext.ProjectUserJunction.Include(puj => puj.Project).Where(puj => puj.UserId == userId).ToListAsync();

                // get all project from junction
                List<ProjectModel> allProject = allUserJunction.Select(puj => puj.Project).ToList();

                List<ProjectDto> allProjectDto = _mapper.Map<List<ProjectDto>>(allProject);

                _responseDto.IsSuccess=true;
                _responseDto.Message = "Success getting project based on user id";
                _responseDto.Result = allProjectDto;

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
