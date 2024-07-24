using AutoMapper;
using QuizApi.Dtos.User;
using QuizApp.Models;

namespace QuizApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<RegisterUserDTO, User>();
               // .ForMember(dest => dest.Password, opt => opt.Ignore()); // Handle password hashing in the service
        }
    }
}
