using AutoMapper;
using WebApplication1.Models;
using WebApplication1.Models.Dtos;


namespace WebApplication1
{
    public class Mappingconfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(config =>
            {
                // create mapping from product to product dto
                config.CreateMap<ProjectModel, ProjectDto>().ReverseMap();

                // create mapping from product dto to product
                config.CreateMap<TeamDto, TeamModel>().ReverseMap();

                // create mappging from category to category dto
                config.CreateMap<ToDoModel, ToDoDto>().ReverseMap();

                config.CreateMap<UserModel, UserDto>().ReverseMap();

                config.CreateMap<ConnectionDto, ConnectionModel>().ReverseMap();
            });

            return mapperConfiguration;
        }
    }
}
