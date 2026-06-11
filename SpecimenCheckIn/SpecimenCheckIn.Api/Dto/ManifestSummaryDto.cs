namespace SpecimenCheckIn.Api.Dto;

public class ManifestSummaryDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }
}