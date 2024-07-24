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
        public DbSet<CorrectAnswer> CorrectAnswers { get; set; }
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
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CorrectAnswer>()
                .HasOne(ca => ca.Question)
                .WithMany(q => q.CorrectAnswers)
                .HasForeignKey(ca => ca.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CorrectAnswer>()
                .HasOne(ca => ca.Option)
                .WithMany(o => o.CorrectAnswers)
                .HasForeignKey(ca => ca.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AllowedUser>()
                .HasOne(au => au.Quiz)
                .WithMany(q => q.AllowedUsers)
                .HasForeignKey(au => au.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AllowedUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.AllowedQuizzes)
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.User)
                .WithMany(u => u.Attempts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attempt>()
                .HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Attempt)
                .WithMany(at => at.Answers)
                .HasForeignKey(a => a.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Option)
                .WithMany(o => o.Answers)
                .HasForeignKey(a => a.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Attempt)
                .WithOne(at => at.Certificate)
                .HasForeignKey<Certificate>(c => c.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Quiz)
                .WithMany(q => q.Certificates)
                .HasForeignKey(c => c.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
