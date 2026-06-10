namespace SpecimenCheckIn.Api.Models;

public class CheckInEvent
{
    public Guid Id { get; set; }

    public Guid ManifestId { get; set; }

    public Guid? SpecimenId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string UserId { get; set; } = "demo-user";

    public DateTime At { get; set; }
}