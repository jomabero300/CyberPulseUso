using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa;

public class ConventionReport
{
    public static byte[] ConventionPdf(List<Convention> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<Convention>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = g => g.Id.ToString() , Align = TextAlign.Center,HeaderAlign = TextAlign.Center },
            new() { Header = "Nombre", RelativeWidth = 1, ValueSelector = g => g.Name! },
            new() { Header = "Estado", FixedWidth = 50, ValueSelector = g => g.Statu!.Name!,Align = TextAlign.Center,HeaderAlign = TextAlign.Center }
        };

        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Convenciones", columns);
    }
}