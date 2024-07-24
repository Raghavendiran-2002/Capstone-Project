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
        }
    }
}
