﻿// <auto-generated />
using System;
using FitnessApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitnessApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FitnessApp.Data.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("Age");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<long>("FacebookId");

                    b.Property<int>("Gender");

                    b.Property<string>("GoogleId");

                    b.Property<float>("Height");

                    b.Property<bool>("IsProfessional");

                    b.Property<double>("Latitude");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<double>("Longitude");

                    b.Property<string>("Nationality");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PictureUrl");

                    b.Property<bool>("ProfileComplete");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<bool>("Visibility");

                    b.Property<float>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.FriendRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatorId");

                    b.Property<DateTime>("DateSent");

                    b.Property<int>("State");

                    b.Property<string>("TargetUserId");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("TargetUserId");

                    b.ToTable("FriendRequests");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.OptimalWeight", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FailCount");

                    b.Property<int>("IncreaseCount");

                    b.Property<DateTime?>("LastIncreaseDay");

                    b.Property<int>("SuccessfullDays");

                    b.Property<Guid>("TrainingId");

                    b.Property<double>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("TrainingId")
                        .IsUnique();

                    b.ToTable("Weights");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.Training", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Day");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsRoutine");

                    b.Property<string>("MuscleGroup");

                    b.Property<string>("UserId");

                    b.Property<double>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Trainings");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.UsersGroups", b =>
                {
                    b.Property<Guid>("GroupId");

                    b.Property<string>("UserId");

                    b.Property<int>("Id");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersGroups");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.Workout", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccelerationValues");

                    b.Property<double>("AverageRepetitionAcceleration");

                    b.Property<double>("AverageRepetitionDuration");

                    b.Property<double>("AverageTilt");

                    b.Property<DateTime>("Date");

                    b.Property<double>("Duration");

                    b.Property<bool>("IsSuccessfull");

                    b.Property<int>("NumberOfRepetitions");

                    b.Property<Guid>("TemplateId");

                    b.Property<string>("TiltValues");

                    b.Property<string>("UserId");

                    b.Property<string>("VelocityValues");

                    b.HasKey("Id");

                    b.HasIndex("TemplateId");

                    b.HasIndex("UserId");

                    b.ToTable("Workouts");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.FriendRequest", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId");

                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser", "TargetUser")
                        .WithMany()
                        .HasForeignKey("TargetUserId");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.OptimalWeight", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.Training", "Training")
                        .WithOne("OptimalWeight")
                        .HasForeignKey("FitnessApp.Data.Entities.OptimalWeight", "TrainingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.Training", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser", "User")
                        .WithMany("Trainings")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.UsersGroups", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser", "User")
                        .WithMany("Groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FitnessApp.Data.Entities.Workout", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.Training", "Template")
                        .WithMany("Workouts")
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("FitnessApp.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
