using AutoMapper;
using QuizApi.Dtos.Profile;
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
            CreateMap<Quiz, ViewUpdateQuizDTO>().ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<ViewUpdateQuizDTO,Quiz>().ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<Quiz, ViewUpdateQuizDTO>().ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<Question, QuestionDTO>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<Question, QuestionProfileDTO>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<QuestionProfileDTO, Question>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<Option, OptionDTO>();
            CreateMap<Option, OptionWithAnswerDTO>();
            CreateMap<OptionWithAnswerDTO, Option>();
            CreateMap<User, ViewProfileDTO>()            
            .ForMember(dest => dest.Attempts, opt => opt.MapFrom(src => src.Attempts));
            CreateMap<Attempt, AttemptProfileDTO>()
                .ForMember(dest => dest.Certificate, opt => opt.MapFrom(src => src.Certificate));
            CreateMap<Certificate, CertificateProfileDTO>();
            CreateMap<QuizDTO, Quiz>()
         
            .ForMember(dest => dest.Questions, opt => opt.Ignore())
            .ForMember(dest => dest.AllowedUsers, opt => opt.Ignore())
            .ForMember(dest => dest.Attempts, opt => opt.Ignore())
            .ForMember(dest => dest.Certificates, opt => opt.Ignore())
            .ForMember(dest => dest.Creator, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
