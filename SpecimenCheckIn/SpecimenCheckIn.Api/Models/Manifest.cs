using SpecimenCheckIn.Api.Enums;
using System.Collections.Generic;

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

    public ICollection<Specimen> Specimens { get; set; } = new List<Specimen>();

    public ICollection<Discrepancy> Discrepancies { get; set; } = new List<Discrepancy>();
}