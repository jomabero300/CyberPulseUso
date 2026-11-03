using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa;

public class ConventionValueReport
{
    public static byte[] ConventionValuePdf(List<ConventionValue> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<ConventionValue>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = c => c.Id.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Nombre", RelativeWidth = 1, ValueSelector = c => c.Convention!.Name!,HeaderAlign=TextAlign.Center },
            new() { Header = "Precio", FixedWidth = 80, ValueSelector = c => c.Prince.ToString("N2")!,Align=TextAlign.Right,HeaderAlign=TextAlign.Center},
            new() { Header = "Estado", FixedWidth = 50, ValueSelector = c => c.Statu!.Name!, Align = TextAlign.Center, HeaderAlign = TextAlign.Center }
        };
        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Beneficios", columns);
    }
}