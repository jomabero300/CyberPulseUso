using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa;

public class ChargeReport
{
    public static byte[] ChargePdf(List<Charge> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<Charge>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = c => c.Id.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Nombre", RelativeWidth = 1, ValueSelector = c => c.Name! },
            new() { Header = "Grupo", RelativeWidth = 1, ValueSelector = c => c.Group!.Name! },
            new() { Header = "Estado", FixedWidth = 50, ValueSelector = c => c.Statu!.Name!, Align = TextAlign.Center, HeaderAlign = TextAlign.Center }
        };
        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Cargos", columns);
    }
}