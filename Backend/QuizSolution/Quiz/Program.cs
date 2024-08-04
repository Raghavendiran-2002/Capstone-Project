using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizApi;
using QuizApi.Interfaces.Repository;
using QuizApi.Interfaces.Service;
using QuizApi.Repositories;
using QuizApi.Services;
using QuizApp.Context;
using QuizApp.Models;
using QuizApi.Exceptions;
using System.Text.Json.Serialization;
namespace QuizApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddLogging(l => l.AddLog4Net());

            #region Auth
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            var userSecret = Environment.GetEnvironmentVariable("JWT_USER_SECRET") ?? "This is the dummy key which has to be a bit long for the 512. which should be even more longer for the passing";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(userSecret))
                   };

               });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
            #endregion

            #region Redis
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            #endregion


            #region Contexts

            var connectionString = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new EnvironmentVariableUndefinedException("SQL_SERVER_CONNECTION_STRING");
            }
            builder.Services.AddDbContext<DBQuizContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });



            builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            #endregion

            #region repositories
            builder.Services.AddScoped<IUserRepository<int, User>, UserRepository>();
            builder.Services.AddScoped<IQuizRepository<int, Quiz>, QuizRepository>();
            builder.Services.AddScoped<ITagRepository<int, Tag>, TagRepository>();
            builder.Services.AddScoped<IQuizTagRepository<string, QuizTag>, QuizTagRepository>();
            builder.Services.AddScoped<IAllowedUserRepository<int, AllowedUser>, AllowedUserRepository>();
            builder.Services.AddScoped<IQuestionRepository<int, Question>, QuestionRepository>();
            builder.Services.AddScoped<IAttemptRepository<int, Attempt>, AttemptRepository>();
            builder.Services.AddScoped<ICertificateRepository<int, Certificate>, CertificateRepository>();

            #endregion

            #region services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();

            #endregion



            #region CORS
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("Cors", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            #endregion

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("Cors");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            // using (var scope = app.Services.CreateScope())
            // {
            //     var dbContext = scope.ServiceProvider.GetRequiredService<DBQuizContext>();
            //     dbContext.Database.Migrate();
            // }

            app.Run();
        }
    }
}
