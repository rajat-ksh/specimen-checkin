using SpecimenCheckIn.Api.Enums;

namespace SpecimenCheckIn.Api.Models;

public class Manifest
{
    public Guid Id { get; set; }

    public Guid LabId { get; set; }

    public Guid ClinicId { get; set; }

    public string Code { get; set; } = string.Empty;

    public ManifestStatus Status { get; set; }

    public DateTime SentAt { get; set; }

    public Lab Lab { get; set; } = null!;

    public Clinic Clinic { get; set; } = null!;

    public ICollection<Specimen> Specimens { get; set; } = [];

    public ICollection<Discrepancy> Discrepancies { get; set; } = [];
}