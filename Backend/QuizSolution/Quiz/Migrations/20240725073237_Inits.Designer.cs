﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizApp.Context;

#nullable disable

namespace QuizApi.Migrations
{
    [DbContext(typeof(DBQuizContext))]
    [Migration("20240725073237_Inits")]
    partial class Inits
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("QuizApp.Models.AllowedUser", b =>
                {
                    b.Property<int>("AllowedUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AllowedUserId"), 1L, 1);

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("AllowedUserId");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserId");

                    b.ToTable("AllowedUsers");
                });

            modelBuilder.Entity("QuizApp.Models.Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AnswerId"), 1L, 1);

                    b.Property<int>("AttemptId")
                        .HasColumnType("int");

                    b.Property<int>("OptionId")
                        .HasColumnType("int");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("AnswerId");

                    b.HasIndex("AttemptId");

                    b.HasIndex("OptionId");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("QuizApp.Models.Attempt", b =>
                {
                    b.Property<int>("AttemptId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttemptId"), 1L, 1);

                    b.Property<DateTime>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("AttemptId");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserId");

                    b.ToTable("Attempts");
                });

            modelBuilder.Entity("QuizApp.Models.Certificate", b =>
                {
                    b.Property<int>("CertificateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CertificateId"), 1L, 1);

                    b.Property<int>("AttemptId")
                        .HasColumnType("int");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CertificateId");

                    b.HasIndex("AttemptId")
                        .IsUnique();

                    b.HasIndex("QuizId");

                    b.HasIndex("UserId");

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("QuizApp.Models.Option", b =>
                {
                    b.Property<int>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OptionId"), 1L, 1);

                    b.Property<bool>("IsAnswer")
                        .HasColumnType("bit");

                    b.Property<string>("OptionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionId")
                        .HasColumnType("int");

                    b.HasKey("OptionId");

                    b.HasIndex("QuestionId");

                    b.ToTable("Options");
                });

            modelBuilder.Entity("QuizApp.Models.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuestionId"), 1L, 1);

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.HasKey("QuestionId");

                    b.HasIndex("QuizId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("QuizApp.Models.Quiz", b =>
                {
                    b.Property<int>("QuizId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuizId"), 1L, 1);

                    b.Property<string>("Background")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Music")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QuizId");

                    b.HasIndex("CreatorId");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("QuizApp.Models.QuizTag", b =>
                {
                    b.Property<int>("QuizId")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TagEntityTagName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("QuizId", "Tag");

                    b.HasIndex("TagEntityTagName");

                    b.ToTable("QuizTags");
                });

            modelBuilder.Entity("QuizApp.Models.Tag", b =>
                {
                    b.Property<string>("TagName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TagName");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            TagName = "Devops"
                        },
                        new
                        {
                            TagName = "App Development"
                        },
                        new
                        {
                            TagName = "System Admin"
                        },
                        new
                        {
                            TagName = "Network Engineer"
                        });
                });

            modelBuilder.Entity("QuizApp.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("QuizApp.Models.AllowedUser", b =>
                {
                    b.HasOne("QuizApp.Models.Quiz", "Quiz")
                        .WithMany("AllowedUsers")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.User", "User")
                        .WithMany("AllowedQuizzes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Quiz");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuizApp.Models.Answer", b =>
                {
                    b.HasOne("QuizApp.Models.Attempt", "Attempt")
                        .WithMany("Answers")
                        .HasForeignKey("AttemptId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.Option", "Option")
                        .WithMany("Answers")
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Attempt");

                    b.Navigation("Option");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuizApp.Models.Attempt", b =>
                {
                    b.HasOne("QuizApp.Models.Quiz", "Quiz")
                        .WithMany("Attempts")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.User", "User")
                        .WithMany("Attempts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Quiz");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuizApp.Models.Certificate", b =>
                {
                    b.HasOne("QuizApp.Models.Attempt", "Attempt")
                        .WithOne("Certificate")
                        .HasForeignKey("QuizApp.Models.Certificate", "AttemptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.Quiz", "Quiz")
                        .WithMany("Certificates")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.User", "User")
                        .WithMany("Certificates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Attempt");

                    b.Navigation("Quiz");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuizApp.Models.Option", b =>
                {
                    b.HasOne("QuizApp.Models.Question", "Question")
                        .WithMany("Options")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuizApp.Models.Question", b =>
                {
                    b.HasOne("QuizApp.Models.Quiz", "Quiz")
                        .WithMany("Questions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Quiz");
                });

            modelBuilder.Entity("QuizApp.Models.Quiz", b =>
                {
                    b.HasOne("QuizApp.Models.User", "Creator")
                        .WithMany("Quizzes")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("QuizApp.Models.QuizTag", b =>
                {
                    b.HasOne("QuizApp.Models.Quiz", "Quiz")
                        .WithMany("QuizTags")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuizApp.Models.Tag", "TagEntity")
                        .WithMany("QuizTags")
                        .HasForeignKey("TagEntityTagName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Quiz");

                    b.Navigation("TagEntity");
                });

            modelBuilder.Entity("QuizApp.Models.Attempt", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("Certificate")
                        .IsRequired();
                });

            modelBuilder.Entity("QuizApp.Models.Option", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("QuizApp.Models.Question", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("Options");
                });

            modelBuilder.Entity("QuizApp.Models.Quiz", b =>
                {
                    b.Navigation("AllowedUsers");

                    b.Navigation("Attempts");

                    b.Navigation("Certificates");

                    b.Navigation("Questions");

                    b.Navigation("QuizTags");
                });

            modelBuilder.Entity("QuizApp.Models.Tag", b =>
                {
                    b.Navigation("QuizTags");
                });

            modelBuilder.Entity("QuizApp.Models.User", b =>
                {
                    b.Navigation("AllowedQuizzes");

                    b.Navigation("Attempts");

                    b.Navigation("Certificates");

                    b.Navigation("Quizzes");
                });
#pragma warning restore 612, 618
        }
    }
}
