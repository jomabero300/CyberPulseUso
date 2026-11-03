using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa;

public class LRPV
{
    public static byte[] LRPVPdf(List<LaborRemunerationParametersValue> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<LaborRemunerationParametersValue>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = c => c.Id.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Descripción", RelativeWidth = 1, ValueSelector = c => c.LRP!.Name! },
            new() { Header = "Grupo", FixedWidth = 50, ValueSelector = c => c.Group!.Name!, Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Fecha", FixedWidth = 50, ValueSelector = c => c.Date.ToString("yyyy-MM-dd"), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Valor", FixedWidth = 50, ValueSelector = c => c.Value.ToString("N2"), Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Estado", FixedWidth = 50, ValueSelector = c => c.Statu!.Name!, Align = TextAlign.Center, HeaderAlign = TextAlign.Center }
        };
        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Parámetros de Remuneración laboral - Valor", columns);
    }
}