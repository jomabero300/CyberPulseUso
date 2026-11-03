using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa;

public class CategoryReport
{
    public static byte[] CategoryPdf(List<Category> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<Category>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = c => c.Id.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Nombre", RelativeWidth = 1, ValueSelector = c => c.Name! },
            new() { Header = "Estado", FixedWidth = 50, ValueSelector = c => c.Statu!.Name!, Align = TextAlign.Center, HeaderAlign = TextAlign.Center }
        };
        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Categorías", columns);
    }
}