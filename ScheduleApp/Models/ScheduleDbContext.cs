using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ScheduleApp.Models;

public partial class ScheduleDbContext : DbContext
{
    public ScheduleDbContext()
    {
    }

    public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=scheduledb;Username=postgres;Password=1634532h");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        modelBuilder.Entity<SaturdayClass>(entity =>
        {
            entity.Property(e => e.StartSaturday).HasColumnType("date");
            entity.Property(e => e.EndSaturday).HasColumnType("date");
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
