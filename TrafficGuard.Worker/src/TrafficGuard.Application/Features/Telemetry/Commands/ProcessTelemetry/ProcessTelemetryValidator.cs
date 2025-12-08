using MediatR;

namespace TrafficGuard.Application.Features.Telemetry.Commands.ProcessTelemetry;

public record ProcessTelemetryCommand(
    string LicensePlate,
    int Speed,
    string RoadName,
    DateTime Timestamp
) : IRequest<bool>;