using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // create instance of class
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<TeamModel> Teams { get; set; }
        public DbSet<ToDoModel> ToDo { get; set; }
        public DbSet<UserModel> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // create relation between projects and todo, where single project can have more than one todo and one todo only had one project
            // when project is being deleted all related to do will also being deleted
            modelBuilder.Entity<ProjectModel>()
                .HasMany(a => a.ToDos)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);


            // create relation project with user as project lead. One project can only have one user as lead.
            // when user is being deleted, all project related to that user also being deleted
            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.ProjectUserLead)
                .HasForeignKey(p => p.ProjectUserLeadId)
                .OnDelete(DeleteBehavior.Cascade);

            // create relation with team as a part of team project. One team can have multiple project
            // when team is being deleted, all project related to that is being deleted
            modelBuilder.Entity<TeamModel>()
                .HasMany(t => t.Projects)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.ProjectTeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // create relation between users and teams as a team lead. User can have more than one team
            // when user is being deleted, team related to that user also being deleted
            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.Teams)
                .WithOne(t => t.TeamUserLeader)
                .HasForeignKey(t => t.TeamLeader)
                .OnDelete(DeleteBehavior.Cascade);

            // create relation on Project User Junction, to user model as one to many
            modelBuilder.Entity<ProjectUserJunction>()
                .HasOne(puj => puj.User)
                .WithMany(u => u.ProjectUsersJunction)
                .HasForeignKey(puj => puj.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // create relation on Project User Junction, to project model as on to many
            modelBuilder.Entity<ProjectUserJunction>()
                .HasOne(puj => puj.Project)
                .WithMany(p => p.ProjectUserJunctions)
                .HasForeignKey(puj => puj.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // create relation on TeamUserJunction, to team model as one to many
            modelBuilder.Entity<TeamUserJunction>()
                .HasOne(tuj => tuj.Team)
                .WithMany(t => t.TeamUserJunction)
                .HasForeignKey(tuj => tuj.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // create rekation on TeamUserJunction, to user model as one to many
            modelBuilder.Entity<TeamUserJunction>()
                .HasOne(tuj => tuj.User)
                .WithMany(u => u.TeamUsersJunction)
                .HasForeignKey(tuj => tuj.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
