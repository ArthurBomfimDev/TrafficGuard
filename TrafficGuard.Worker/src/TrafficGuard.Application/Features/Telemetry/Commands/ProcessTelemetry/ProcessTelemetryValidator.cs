using FluentValidation;

namespace TrafficGuard.Application.Features.Telemetry.Commands.ProcessTelemetry;

public class ProcessTelemetryValidator : AbstractValidator<ProcessTelemetryCommand>
{
    public ProcessTelemetryValidator()
    {
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .Length(7).WithMessage("License plate must have exactly 7 characters (e.g., ABC1234).");

        RuleFor(x => x.Speed)
            .GreaterThan(0).WithMessage("Speed must be greater than zero.")
            .LessThan(500).WithMessage("Speed implies a supersonic jet, probably a sensor error.");

        RuleFor(x => x.Timestamp)
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1)) 
            .WithMessage("Timestamp cannot be in the future.");
    }
}