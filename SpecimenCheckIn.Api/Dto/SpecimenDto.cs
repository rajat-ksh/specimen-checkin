namespace SpecimenCheckIn.Api.Dto
{
    public class SpecimenDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Patient { get; set; } = string.Empty;

        public string Site { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
