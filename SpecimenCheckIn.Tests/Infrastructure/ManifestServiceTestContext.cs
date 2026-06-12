using Microsoft.EntityFrameworkCore;
using SpecimenCheckIn.Api.Data;
using SpecimenCheckIn.Api.Enums;
using SpecimenCheckIn.Api.Infrastructure;
using SpecimenCheckIn.Api.Models;
using SpecimenCheckIn.Api.Services;
using System;

namespace SpecimenCheckIn.Tests.Infrastructure;

public sealed class ManifestServiceTestContext : IDisposable
{
    public static readonly Guid LabAId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid LabBId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid ClinicAId =
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public static readonly Guid ClinicBId =
        Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public static readonly Guid ManifestAId =
        Guid.Parse("10000000-0000-0000-0000-000000000001");

    public static readonly Guid ManifestBId =
        Guid.Parse("20000000-0000-0000-0000-000000000001");

    public static readonly Guid PendingSpecimenAId =
        Guid.Parse("30000000-0000-0000-0000-000000000001");

    public static readonly Guid SecondPendingSpecimenAId =
        Guid.Parse("30000000-0000-0000-0000-000000000002");

    public static readonly Guid ReceivedSpecimenAId =
        Guid.Parse("30000000-0000-0000-0000-000000000003");

    public static readonly Guid PendingSpecimenBId =
        Guid.Parse("40000000-0000-0000-0000-000000000001");

    public AppDbContext Db { get; }

    public TenantContext TenantContext { get; }

    public ManifestService Service { get; }

    public ManifestServiceTestContext(Guid? tenantId = null)
    {
        var databaseName =
            $"SpecimenCheckIn_Test_{Guid.NewGuid():N}";

        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

        Db = new AppDbContext(options);

        SeedDatabase();

        TenantContext = new TenantContext
        {
            LabId = tenantId ?? LabAId
        };

        Service = new ManifestService(
            Db,
            TenantContext
        );
    }

    private void SeedDatabase()
    {
        Db.Labs.AddRange(
            new Lab
            {
                Id = LabAId,
                Name = "Lab A"
            },
            new Lab
            {
                Id = LabBId,
                Name = "Lab B"
            }
        );

        Db.Clinics.AddRange(
            new Clinic
            {
                Id = ClinicAId,
                LabId = LabAId,
                Name = "Clinic A"
            },
            new Clinic
            {
                Id = ClinicBId,
                LabId = LabBId,
                Name = "Clinic B"
            }
        );

        Db.Manifests.AddRange(
            new Manifest
            {
                Id = ManifestAId,
                LabId = LabAId,
                ClinicId = ClinicAId,
                Code = "MAN-A-001",
                Status = ManifestStatus.Open,
                SentAt = DateTime.UtcNow.AddDays(-1)
            },
            new Manifest
            {
                Id = ManifestBId,
                LabId = LabBId,
                ClinicId = ClinicBId,
                Code = "MAN-B-001",
                Status = ManifestStatus.Open,
                SentAt = DateTime.UtcNow.AddDays(-1)
            }
        );

        Db.Specimens.AddRange(
            new Specimen
            {
                Id = PendingSpecimenAId,
                ManifestId = ManifestAId,
                Code = "SP-A-001",
                Patient = "Synthetic Patient A",
                Site = "Skin",
                Provider = "Dr Test",
                Status = SpecimenStatus.Pending
            },
            new Specimen
            {
                Id = SecondPendingSpecimenAId,
                ManifestId = ManifestAId,
                Code = "SP-A-002",
                Patient = "Synthetic Patient B",
                Site = "Lung",
                Provider = "Dr Test",
                Status = SpecimenStatus.Pending
            },
            new Specimen
            {
                Id = ReceivedSpecimenAId,
                ManifestId = ManifestAId,
                Code = "SP-A-003",
                Patient = "Synthetic Patient C",
                Site = "Liver",
                Provider = "Dr Test",
                Status = SpecimenStatus.Received
            },
            new Specimen
            {
                Id = PendingSpecimenBId,
                ManifestId = ManifestBId,
                Code = "SP-B-001",
                Patient = "Synthetic Patient D",
                Site = "Colon",
                Provider = "Dr Example",
                Status = SpecimenStatus.Pending
            }
        );

        Db.SaveChanges();
    }

    public void Dispose()
    {
        Db.Dispose();
    }
}