using TrafficGuard.Domain.Entities;

namespace TrafficGuard.Domain.Interfaces;

public interface IViolationReportGenerator
{
    // Gera o PDF e retorna o caminho do arquivo salvo
    string GenerateFineNotice(TrafficViolation violation);
}