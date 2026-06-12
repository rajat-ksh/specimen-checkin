using System.Collections.Generic;

namespace SpecimenCheckIn.Api.Dto;

public class ManifestDetailDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }

    public int TotalSpecimens { get; set; }

    public int ReceivedCount { get; set; }

    public int PendingCount { get; set; }

    public int FlaggedCount { get; set; }

    public int AddedCount { get; set; }

    public bool ReadyToClose { get; set; }

    public List<SpecimenDto> Specimens { get; set; } = new List<SpecimenDto>();
}
