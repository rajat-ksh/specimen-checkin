using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpecimenCheckIn.Api.Models;

namespace SpecimenCheckIn.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<Clinic> Clinics => Set<Clinic>();
    public DbSet<Manifest> Manifests => Set<Manifest>();
    public DbSet<Specimen> Specimens => Set<Specimen>();
    public DbSet<Discrepancy> Discrepancies => Set<Discrepancy>();
    public DbSet<CheckInEvent> CheckInEvents => Set<CheckInEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prevent multiple cascade paths involving Lab -> Clinic -> Manifest
        modelBuilder.Entity<Clinic>()
            .HasOne(c => c.Lab)
            .WithMany(l => l.Clinics)
            .HasForeignKey(c => c.LabId)
            .OnDelete(DeleteBehavior.Restrict);

        // Make deletes from Clinic -> Manifest non-cascading to avoid a second path
        modelBuilder.Entity<Manifest>()
            .HasOne(m => m.Clinic)
            .WithMany()
            .HasForeignKey(m => m.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        // Be explicit about Manifest -> Lab relationship
        modelBuilder.Entity<Manifest>()
            .HasOne(m => m.Lab)
            .WithMany(l => l.Manifests)
            .HasForeignKey(m => m.LabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
