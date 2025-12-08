namespace TrafficGuard.Application.TrafficViolation;

public class TrafficViolationDTO
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int MeasuredSpeed { get; set; }
    public int SpeedLimit { get; set; }
    public decimal FineAmount { get; set; }
    public string Severity { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public static TrafficViolationDTO FromEntity(Domain.Entities.TrafficViolation entity)
    {
        return new TrafficViolationDTO
        {
            Id = entity.Id,
            LicensePlate = entity.LicensePlate.Value,
            MeasuredSpeed = entity.MeasuredSpeed,
            SpeedLimit = entity.SpeedLimit,
            FineAmount = entity.FineAmount,
            Severity = entity.Severity.ToString(),
            OccurredAt = entity.OccurredAt,
            Status = entity.Status
        };
    }
}