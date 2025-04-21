using Microsoft.EntityFrameworkCore;
using ScheduleApp.Models;
using SQLitePCL;
using System.ComponentModel.Design;
namespace ScheduleApp.Data
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
             : base(options)
        {
            Database.EnsureCreated();//del
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Semester>()
                .HasOne(s => s.Root)
                .WithOne(r => r.Semester)
                .HasForeignKey<Semester>(s => s.RootId);
        }
        public DbSet<Root> Roots { get; set; } = null!;
        public DbSet<ScheduleItem> Schedules { get; set; } = null!;
        public DbSet<ClassItem> ClassItems { get; set; } = null!;
        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Day> Days { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<SemesterClass> SemesterClasses { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Weeks> Weeks { get; set; } = null!;

        public DbSet<SaturdayClass> SaturdayClasses { get; set; } = null!;
    }
}