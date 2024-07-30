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
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["TokenKey:JWT"]))
                   };

               });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
            #endregion
            // Add services to the container.1


            #region Contexts
            var DBHOST = Environment.GetEnvironmentVariable("DB_HOST");
            var DBPASS = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            var DBNAME = Environment.GetEnvironmentVariable("DB_NAME");
            //var connectionString = $"Server={DBHOST};Database={DBNAME};User ID=sa;Password={DBPASS};TrustServerCertificate=True;Integrated Security=True;MultipleActiveResultSets=true";
            //var connectionString = "Data Source = GRMCBX3; Integrated Security = true; Initial Catalog = dbQuiz";
            var connectionString = "Server=tcp:sql-server-raghav.database.windows.net,1433;Initial Catalog=raghav;Persist Security Info=False;User ID=raghav;Password=pass@123;MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;";
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
             /*using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DBQuizContext>();
                dbContext.Database.Migrate();
            }*/

            app.Run();
        }
    }
}
