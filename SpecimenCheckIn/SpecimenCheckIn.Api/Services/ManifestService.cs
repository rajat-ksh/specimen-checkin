using Microsoft.EntityFrameworkCore;
using SpecimenCheckIn.Api.Data;
using SpecimenCheckIn.Api.Dto;
using SpecimenCheckIn.Api.Enums;
using SpecimenCheckIn.Api.Infrastructure;
using SpecimenCheckIn.Api.Models;
using System.Linq;
using System.Collections.Generic;

namespace SpecimenCheckIn.Api.Services;

public class ManifestService(AppDbContext appDbContext, TenantContext tenantContext)
{
    private readonly AppDbContext _db = appDbContext;
    private readonly TenantContext _tenant = tenantContext;

    public async Task<List<ManifestSummaryDto>> GetManifestsAsync()
    {
        return await _db.Manifests
            .Where(x => x.LabId == _tenant.LabId)
            .OrderByDescending(x => x.SentAt)
            .Select(m => new ManifestSummaryDto
            {
                Id = m.Id,
                Code = m.Code,
                Status = m.Status.ToString(),
                SentAt = m.SentAt
            })
            .ToListAsync();
    }

    public async Task<ManifestDetailDto?> GetManifestAsync(Guid id)
    {
        var manifest = await _db.Manifests
            .Include(x => x.Specimens)
            .Include(x => x.Discrepancies)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.LabId == _tenant.LabId);

        if (manifest is null)
            return null;

        var specimens = manifest.Specimens
            .Select(s => new SpecimenDto
            {
                Id = s.Id,
                Code = s.Code,
                Patient = s.Patient,
                Site = s.Site,
                Provider = s.Provider,
                Status = s.Status.ToString()
            })
            .ToList();

        var total = specimens.Count;
        var received = manifest.Specimens.Count(s => s.Status == SpecimenStatus.Received);
        var pending = manifest.Specimens.Count(s => s.Status == SpecimenStatus.Pending);
        var flagged = manifest.Specimens.Count(s => s.Status == SpecimenStatus.Flagged);
        var added = manifest.Specimens.Count(s => s.Status == SpecimenStatus.Added);

        var dto = new ManifestDetailDto
        {
            Id = manifest.Id,
            Code = manifest.Code,
            Status = manifest.Status.ToString(),
            SentAt = manifest.SentAt,
            TotalSpecimens = total,
            ReceivedCount = received,
            PendingCount = pending,
            FlaggedCount = flagged,
            AddedCount = added,
            ReadyToClose = pending == 0,
            Specimens = specimens
        };

        return dto;
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
            throw new InvalidOperationException("Specimen not found");

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
