using Microsoft.EntityFrameworkCore;
using QuizApp.Models;

namespace QuizApp.Context
{
    public class DBQuizContext : DbContext
    {
        public DBQuizContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<QuizTag> QuizTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AllowedUser> AllowedUsers { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Certificate> Certificates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define composite keys
            modelBuilder.Entity<QuizTag>().HasKey(qt => new { qt.QuizId, qt.Tag });

            // Define relationships
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Creator)
                .WithMany(u => u.Quizzes)
                .HasForeignKey(q => q.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);            

            modelBuilder.Entity<AllowedUser>()
                .HasOne(au => au.Quiz)
                .WithMany(q => q.AllowedUsers)
                .HasForeignKey(au => au.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AllowedUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.AllowedQuizzes)
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.User)
                .WithMany(u => u.Attempts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Attempt)
                .WithMany(at => at.Answers)
                .HasForeignKey(a => a.AttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Option)
                .WithMany(o => o.Answers)
                .HasForeignKey(a => a.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
      
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            var tag1 = new Tag { TagName = "Devops" };
            var tag2 = new Tag { TagName = "App Development" };
            var tag3 = new Tag { TagName = "System Admin" };
            var tag4 = new Tag { TagName = "Network Engineer" };
            modelBuilder.Entity<Tag>().HasData(tag1);
            modelBuilder.Entity<Tag>().HasData(tag2);
            modelBuilder.Entity<Tag>().HasData(tag3);
            modelBuilder.Entity<Tag>().HasData(tag4);
        }
    }
}
