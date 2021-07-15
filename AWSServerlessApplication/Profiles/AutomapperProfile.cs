using AutoMapper;
using AWSServerlessApplication.Dto;
using AWSServerlessApplication.Models;


namespace AWSServerlessApplication.Profiles
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            MapUser();
        }

        private void MapUser()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
