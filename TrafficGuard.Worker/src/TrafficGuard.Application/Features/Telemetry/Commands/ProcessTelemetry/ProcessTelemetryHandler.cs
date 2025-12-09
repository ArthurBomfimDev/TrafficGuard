using MediatR;
using Microsoft.Extensions.Logging;
using FluentValidation;
using TrafficGuard.Domain.Entities;
using TrafficGuard.Domain.Interfaces;
using TrafficGuard.Domain.Exceptions;

namespace TrafficGuard.Application.Features.Telemetry.Commands.ProcessTelemetry;

public class ProcessTelemetryHandler : IRequestHandler<ProcessTelemetryCommand, bool>
{
    private readonly IViolationRepository _repository;
    private readonly IValidator<ProcessTelemetryCommand> _validator;
    private readonly ILogger<ProcessTelemetryHandler> _logger;
    private readonly IViolationReportGenerator _reportGenerator;

    private const int CURRENT_SPEED_LIMIT = 60;

    public ProcessTelemetryHandler(
        IViolationRepository repository,
        IValidator<ProcessTelemetryCommand> validator,
        ILogger<ProcessTelemetryHandler> logger,
        IViolationReportGenerator reportGenerator) 
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
        _reportGenerator = reportGenerator; 
    }

    public async Task<bool> Handle(ProcessTelemetryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing telemetry: {Plate} at {Speed}km/h", request.LicensePlate, request.Speed);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                _logger.LogWarning("Validation Failed: {Error}", error.ErrorMessage);
            }
            return false; 
        }

        try
        {
            var violation = TrafficGuard.Domain.Entities.TrafficViolation.Register(
                request.LicensePlate,
                request.Speed,
                CURRENT_SPEED_LIMIT,
                request.Timestamp
            );

            await _repository.SaveAsync(violation);

            var pdfPath = _reportGenerator.GenerateFineNotice(violation);
            _logger.LogInformation("PDF Generated successfully: {Path}", pdfPath);

            _logger.LogInformation("VIOLATION CONFIRMED! Severity: {Severity} | Fine: {Fine}",
                violation.Severity, violation.FineAmount);

            return true;
        }
        catch (DomainException ex)
        {

            _logger.LogInformation("Telemetry ignored: {Reason}", ex.Message);
            return true; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error processing telemetry");
            throw; 
        }
    }
}