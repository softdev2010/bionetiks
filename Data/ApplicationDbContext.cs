using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data.Entities;

namespace FitnessApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<OptimalWeight> Weights { get; set; }
        public DbSet<UsersGroups> UsersGroups { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UsersGroups>()
                .HasKey(bc => new { bc.GroupId, bc.UserId });

            builder.Entity<UsersGroups>()
                .HasOne(bc => bc.Group)
                .WithMany(b => b.Users)
                .HasForeignKey(bc => bc.GroupId);

            builder.Entity<UsersGroups>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.Groups)
                .HasForeignKey(bc => bc.UserId);
            
            builder.Entity<Training>()
                .HasOne(x => x.OptimalWeight)
                .WithOne(x => x.Training)
                .HasForeignKey<OptimalWeight>(x => x.TrainingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
