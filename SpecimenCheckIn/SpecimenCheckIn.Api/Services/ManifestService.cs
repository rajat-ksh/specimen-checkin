using Microsoft.EntityFrameworkCore;
using SpecimenCheckIn.Api.Data;
using SpecimenCheckIn.Api.Dto;
using SpecimenCheckIn.Api.Enums;
using SpecimenCheckIn.Api.Infrastructure;
using SpecimenCheckIn.Api.Models;

namespace SpecimenCheckIn.Api.Services;

public class ManifestService(AppDbContext appDbContext, TenantContext tenantContext)
{
    private readonly AppDbContext _db = appDbContext;
    private readonly TenantContext _tenant = tenantContext;

    public async Task<List<Manifest>> GetManifestsAsync()
    {
        return await _db.Manifests
            .Where(x => x.LabId == _tenant.LabId)
            .OrderByDescending(x => x.SentAt)
            .ToListAsync();
    }
    public async Task<Manifest?> GetManifestAsync(Guid id)
    {
        return await _db.Manifests
            .Include(x => x.Specimens)
            .Include(x => x.Discrepancies)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.LabId == _tenant.LabId);
    }

    public async Task ReceiveSpecimenAsync(
     Guid manifestId,
     Guid specimenId)
    {
        var specimen = await _db.Specimens
            .Include(x => x.Manifest)
            .FirstOrDefaultAsync(x =>
                x.Id == specimenId &&
                x.ManifestId == manifestId &&
                x.Manifest.LabId == _tenant.LabId);

        if (specimen == null)
            throw new Exception("Specimen not found");

        if (specimen.Status == SpecimenStatus.Received)
            return;

        specimen.Status = SpecimenStatus.Received;

        _db.CheckInEvents.Add(new CheckInEvent
        {
            Id = Guid.NewGuid(),
            ManifestId = manifestId,
            SpecimenId = specimenId,
            Action = "Received",
            At = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
    public async Task FlagSpecimenAsync(
    Guid manifestId,
    Guid specimenId)
    {
        var specimen = await _db.Specimens
            .Include(x => x.Manifest)
            .FirstOrDefaultAsync(x =>
                x.Id == specimenId &&
                x.ManifestId == manifestId &&
                x.Manifest.LabId == _tenant.LabId);

        if (specimen is null)
        {
            throw new InvalidOperationException(
                "Specimen not found");
        }

        if (specimen.Status == SpecimenStatus.Flagged)
        {
            return;
        }

        specimen.Status = SpecimenStatus.Flagged;

        var existingDiscrepancy = await _db.Discrepancies
            .AnyAsync(x =>
                x.ManifestId == manifestId &&
                x.SpecimenId == specimenId &&
                x.Type == DiscrepancyType.Missing);

        if (!existingDiscrepancy)
        {
            _db.Discrepancies.Add(new Discrepancy
            {
                Id = Guid.NewGuid(),
                ManifestId = manifestId,
                SpecimenId = specimenId,
                Type = DiscrepancyType.Missing,
                Note = "Specimen marked as missing",
                Status = DiscrepancyStatus.Open
            });
        }

        _db.CheckInEvents.Add(new CheckInEvent
        {
            Id = Guid.NewGuid(),
            ManifestId = manifestId,
            SpecimenId = specimenId,
            Action = "Flagged Missing",
            UserId = "demo-user",
            At = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task AddOffManifestSpecimenAsync(
    Guid manifestId,
    AddSpecimenRequest request)
    {
        var manifest = await _db.Manifests
            .FirstOrDefaultAsync(x =>
                x.Id == manifestId &&
                x.LabId == _tenant.LabId);

        if (manifest is null)
        {
            throw new InvalidOperationException(
                "Manifest not found");
        }

        var specimen = new Specimen
        {
            Id = Guid.NewGuid(),
            ManifestId = manifestId,
            Code = request.Code,
            Patient = request.Patient,
            Site = request.Site,
            Provider = request.Provider,
            Status = SpecimenStatus.Added
        };

        _db.Specimens.Add(specimen);

        _db.Discrepancies.Add(new Discrepancy
        {
            Id = Guid.NewGuid(),
            ManifestId = manifestId,
            SpecimenId = specimen.Id,
            Type = DiscrepancyType.OffManifest,
            Note = "Off-manifest specimen received",
            Status = DiscrepancyStatus.Open
        });

        _db.CheckInEvents.Add(new CheckInEvent
        {
            Id = Guid.NewGuid(),
            ManifestId = manifestId,
            SpecimenId = specimen.Id,
            Action = "Off Manifest Specimen Added",
            UserId = "demo-user",
            At = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task CloseManifestAsync(Guid manifestId)
    {
        var manifest = await _db.Manifests
            .Include(x => x.Specimens)
            .Include(x => x.Discrepancies)
            .FirstOrDefaultAsync(x =>
                x.Id == manifestId &&
                x.LabId == _tenant.LabId);

        if (manifest is null)
        {
            throw new InvalidOperationException(
                "Manifest not found");
        }

        var hasPendingSpecimens =
            manifest.Specimens.Any(x =>
                x.Status == SpecimenStatus.Pending);

        if (hasPendingSpecimens)
        {
            throw new InvalidOperationException(
                "Manifest cannot be closed. Pending specimens remain.");
        }

        var hasOpenDiscrepancies =
            manifest.Discrepancies.Any();

        manifest.Status = hasOpenDiscrepancies
            ? ManifestStatus.ClosedWithDiscrepancy
            : ManifestStatus.Closed;

        _db.CheckInEvents.Add(new CheckInEvent
        {
            Id = Guid.NewGuid(),
            ManifestId = manifest.Id,
            Action = "Manifest Closed",
            UserId = "demo-user",
            At = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

}
