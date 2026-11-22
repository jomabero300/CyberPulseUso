using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;

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

    public static byte[] LRPVPivotPdf(List<LaborRemunerationParametersValuePivotDTO> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<LaborRemunerationParametersValuePivotDTO>>
        {
            new() { Header = "Código", FixedWidth = 40, ValueSelector = c => c.LRPId.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Descripción", RelativeWidth = 2, ValueSelector = c => c.Name },

            new() { Header = "Grupo VI", RelativeWidth = 1, ValueSelector = c => c.GroupVI?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo III", RelativeWidth = 1, ValueSelector = c => c.GroupIII?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo II", RelativeWidth = 1, ValueSelector = c => c.GroupII?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo I", RelativeWidth = 1, ValueSelector = c => c.GroupI?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 8", RelativeWidth = 1, ValueSelector = c => c.Group8?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 7", RelativeWidth = 1, ValueSelector = c => c.Group7?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 6", RelativeWidth = 1, ValueSelector = c => c.Group6?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 5", RelativeWidth = 1, ValueSelector = c => c.Group5?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 4", RelativeWidth = 1, ValueSelector = c => c.Group4?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 3", RelativeWidth = 1, ValueSelector = c => c.Group3?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 2A", RelativeWidth = 1, ValueSelector = c => c.Group2A?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 2", RelativeWidth = 1, ValueSelector = c => c.Group2?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center },
            new() { Header = "Grupo 1", RelativeWidth = 1, ValueSelector = c => c.Group1?.ToString("N2") ?? "-", Align = TextAlign.Right, HeaderAlign = TextAlign.Center }
        };

        return PdfReportBuilder.GeneratePdf(
            entity,
            rutaPath,
            "Parámetros de Remuneración Laboral por Grupos",
            columns,
            landscape: true);
    }

}