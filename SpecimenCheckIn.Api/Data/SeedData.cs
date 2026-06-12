using SpecimenCheckIn.Api.Enums;
using SpecimenCheckIn.Api.Models;

namespace SpecimenCheckIn.Api.Data;

public class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Labs.Any())
            return;

        var labA = new Lab
        {
            Id = Guid.NewGuid(),
            Name = "Lab A"
        };

        var labB = new Lab
        {
            Id = Guid.NewGuid(),
            Name = "Lab B"
        };

        db.Labs.AddRange(labA, labB);

        var clinicA = new Clinic
        {
            Id = Guid.NewGuid(),
            Name = "Downtown Clinic",
            LabId = labA.Id
        };

        var clinicB = new Clinic
        {
            Id = Guid.NewGuid(),
            Name = "Westside Clinic",
            LabId = labB.Id
        };

        db.Clinics.AddRange(clinicA, clinicB);

        var manifestA1 = new Manifest
        {
            Id = Guid.NewGuid(),
            LabId = labA.Id,
            ClinicId = clinicA.Id,
            Code = "MAN-1001",
            Status = ManifestStatus.Open,
            SentAt = DateTime.UtcNow.AddDays(-1)
        };

        var manifestA2 = new Manifest
        {
            Id = Guid.NewGuid(),
            LabId = labA.Id,
            ClinicId = clinicA.Id,
            Code = "MAN-1002",
            Status = ManifestStatus.Open,
            SentAt = DateTime.UtcNow
        };

        var manifestB1 = new Manifest
        {
            Id = Guid.NewGuid(),
            LabId = labB.Id,
            ClinicId = clinicB.Id,
            Code = "MAN-2001",
            Status = ManifestStatus.Open,
            SentAt = DateTime.UtcNow
        };

        db.Manifests.AddRange(
            manifestA1,
            manifestA2,
            manifestB1);
        db.Specimens.AddRange(

            new Specimen
            {
                Id = Guid.NewGuid(),
                ManifestId = manifestA1.Id,
                Code = "SP001",
                Patient = "John Smith",
                Site = "Lung",
                Provider = "Dr Adams",
                Status = SpecimenStatus.Pending
            },

            new Specimen
            {
                Id = Guid.NewGuid(),
                ManifestId = manifestA1.Id,
                Code = "SP002",
                Patient = "Jane Doe",
                Site = "Liver",
                Provider = "Dr Adams",
                Status = SpecimenStatus.Received
            },

            new Specimen
            {
                Id = Guid.NewGuid(),
                ManifestId = manifestA1.Id,
                Code = "SP003",
                Patient = "Bob White",
                Site = "Skin",
                Provider = "Dr Green",
                Status = SpecimenStatus.Flagged
            },

            new Specimen
            {
                Id = Guid.NewGuid(),
                ManifestId = manifestB1.Id,
                Code = "SP101",
                Patient = "Alice Brown",
                Site = "Colon",
                Provider = "Dr Jones",
                Status = SpecimenStatus.Pending
            }
        );

        await db.SaveChangesAsync();
    }
}
