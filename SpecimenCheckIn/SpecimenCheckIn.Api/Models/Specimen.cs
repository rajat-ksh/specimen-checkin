using SpecimenCheckIn.Api.Enums;

namespace SpecimenCheckIn.Api.Models;

public class Specimen
{
    public Guid Id { get; set; }

    public Guid ManifestId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Patient { get; set; } = string.Empty;

    public string Site { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public SpecimenStatus Status { get; set; }

    public Manifest Manifest { get; set; } = null!;
}