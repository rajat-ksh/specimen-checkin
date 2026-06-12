using System.Collections.Generic;

namespace SpecimenCheckIn.Api.Models;

public class Lab
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Clinic> Clinics { get; set; } = new List<Clinic>();
    public ICollection<Manifest> Manifests { get; set; } = new List<Manifest>();
}