namespace SpecimenCheckIn.Api.Models;

public class Lab
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Clinic> Clinics { get; set; } = [];
    public ICollection<Manifest> Manifests { get; set; } = [];
}