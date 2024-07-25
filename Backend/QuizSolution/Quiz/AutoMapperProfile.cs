using AutoMapper;
using QuizApi.Dtos.Quiz;
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
            CreateMap<Quiz, QuizDTO>();
            CreateMap<CreateQuizDTO, Quiz>();
            CreateMap<CreateQuestionDTO, Question>();
            CreateMap<Question, QuestionDTO>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<Option, OptionDTO>();
        }
    }
}
