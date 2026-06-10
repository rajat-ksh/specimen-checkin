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
    }
}