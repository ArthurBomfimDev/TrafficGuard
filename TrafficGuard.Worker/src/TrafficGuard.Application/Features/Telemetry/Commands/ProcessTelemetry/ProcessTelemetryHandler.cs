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

    private const int CURRENT_SPEED_LIMIT = 60;

    public ProcessTelemetryHandler(
        IViolationRepository repository,
        IValidator<ProcessTelemetryCommand> validator,
        ILogger<ProcessTelemetryHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<bool> Handle(ProcessTelemetryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing telemetry: {Plate} at {Speed}km/h", request.LicensePlate, request.Speed);

        // 1. Validação de Entrada (Fail Fast)
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                _logger.LogWarning("Validation Failed: {Error}", error.ErrorMessage);
            }
            return false; // Dados inválidos, descarta.
        }

        try
        {
            // 2. Chama a Factory do Domínio (Regra de Negócio Pura)
            // Se a velocidade for baixa, o Register lança DomainException
            var violation = TrafficViolation.Register(
                request.LicensePlate,
                request.Speed,
                CURRENT_SPEED_LIMIT,
                request.Timestamp
            );

            // 3. Persistência
            await _repository.SaveAsync(violation);

            _logger.LogInformation("VIOLATION CONFIRMED! Severity: {Severity} | Fine: {Fine}",
                violation.Severity, violation.FineAmount);

            return true;
        }
        catch (DomainException ex)
        {
            // Isso NÃO é um erro de sistema. É uma regra de negócio agindo.
            // Ex: Carro dentro do limite de velocidade.
            _logger.LogInformation("Telemetry ignored: {Reason}", ex.Message);
            return true; // Processado com sucesso (apenas não gerou multa)
        }
        catch (Exception ex)
        {
            // Isso É um erro de sistema (banco caiu, etc).
            _logger.LogError(ex, "Critical error processing telemetry");
            throw; // Relança para o RabbitMQ tentar de novo (Nack)
        }
    }
}