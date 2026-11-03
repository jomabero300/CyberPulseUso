using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa;

public class LRPReport
{
    public static byte[] LRPPdf(List<LaborRemunerationParameters> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<LaborRemunerationParameters>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = c => c.Id.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Nombre", RelativeWidth = 1, ValueSelector = c => c.Name! },
            new() { Header = "Estado", FixedWidth = 50, ValueSelector = c => c.Statu!.Name!, Align = TextAlign.Center, HeaderAlign = TextAlign.Center }
        };
        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Parámetros de Remuneración laboral", columns);
    }
}