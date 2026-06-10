using SpecimenCheckIn.Api.Enums;

namespace SpecimenCheckIn.Api.Models;

public class Discrepancy
{
    public Guid Id { get; set; }

    public Guid ManifestId { get; set; }

    public Guid? SpecimenId { get; set; }

    public DiscrepancyType Type { get; set; }

    public string Note { get; set; } = string.Empty;

    public DiscrepancyStatus Status { get; set; }

    public Manifest Manifest { get; set; } = null!;
}