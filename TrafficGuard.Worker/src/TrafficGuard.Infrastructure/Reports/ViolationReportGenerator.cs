using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TrafficGuard.Domain.Entities;
using TrafficGuard.Domain.Interfaces;

namespace TrafficGuard.Infrastructure.Reports;

public class ViolationReportGenerator : IViolationReportGenerator
{
    public string GenerateFineNotice(TrafficViolation violation)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Multas_Geradas");
        Directory.CreateDirectory(folderPath);

        var fileName = $"Multa_{violation.LicensePlate}_{violation.Id}_{DateTime.Now.Ticks}.pdf";
        var fullPath = Path.Combine(folderPath, fileName);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("AUTO DE INFRAÇÃO DE TRÂNSITO")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium).AlignCenter();

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Text($"Número do Auto: {violation.Id:D6}");
                    col.Item().Text($"Data da Emissão: {DateTime.Now:dd/MM/yyyy HH:mm}");

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    col.Item().Text("DADOS DO VEÍCULO").Bold();
                    col.Item().Text($"Placa: {violation.LicensePlate}");

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    col.Item().Text("DADOS DA INFRAÇÃO").Bold();
                    col.Item().Text($"Data/Hora: {violation.OccurredAt:dd/MM/yyyy HH:mm:ss}");
                    col.Item().Text($"Velocidade Medida: {violation.MeasuredSpeed} km/h").FontColor(Colors.Red.Medium);
                    col.Item().Text($"Velocidade Limite: {violation.SpeedLimit} km/h");

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                    {
                        c.Item().Text("PENALIDADE").Bold();
                        c.Item().Text($"Gravidade: {violation.Severity}");
                        c.Item().Text($"Valor da Multa: R$ {violation.FineAmount:F2}").FontSize(16).Bold();
                        c.Item().Text("Status: Aguardando Pagamento");
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("TrafficGuard System - ");
                        x.Span(DateTime.Now.Year.ToString());
                    });
            });
        }).GeneratePdf(fullPath);

        return fullPath;
    }
}