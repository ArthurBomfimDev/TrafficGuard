using TrafficGuard.Domain.Enums;
using TrafficGuard.Domain.Exceptions;
using TrafficGuard.Domain.ValueObjects;

namespace TrafficGuard.Domain.Entities;

public class TrafficViolation
{
    public int Id { get; private set; }
    public LicensePlate LicensePlate { get; private set; }
    public int MeasuredSpeed { get; private set; }
    public int SpeedLimit { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public decimal FineAmount { get; private set; }
    public ViolationSeverity Severity { get; private set; }
    public string Status { get; private set; }

    protected TrafficViolation() { }

    private TrafficViolation(LicensePlate plate, int measuredSpeed, int speedLimit, DateTime occurredAt)
    {
        LicensePlate = plate;
        MeasuredSpeed = measuredSpeed;
        SpeedLimit = speedLimit;
        OccurredAt = occurredAt;
        CreatedAt = DateTime.UtcNow;
        Status = "Processing";

        CalculatePenalty();
    }

    public static TrafficViolation Register(string rawPlate, int measuredSpeed, int speedLimit, DateTime occurredAt)
    {
        if (measuredSpeed <= 0) throw new DomainException("Measured speed must be positive.");
        if (speedLimit <= 0) throw new DomainException("Speed limit must be positive.");
        if (occurredAt > DateTime.UtcNow) throw new DomainException("Occurrence date cannot be in the future.");

        if (measuredSpeed <= speedLimit)
            throw new DomainException($"No violation. Speed {measuredSpeed} is within limit {speedLimit}.");

        var plate = LicensePlate.Create(rawPlate);

        return new TrafficViolation(plate, measuredSpeed, speedLimit, occurredAt);
    }

    private void CalculatePenalty()
    {
        double percentageOver = ((double)MeasuredSpeed / SpeedLimit) - 1.0;

        if (percentageOver <= 0.20) 
        {
            Severity = ViolationSeverity.Medium;
            FineAmount = 130.16m;
        }
        else if (percentageOver <= 0.50) 
        {
            Severity = ViolationSeverity.Serious;
            FineAmount = 195.23m;
        }
        else 
        {
            Severity = ViolationSeverity.VerySerious;
            FineAmount = 880.41m;
        }

        Status = "Confirmada";
    }
}