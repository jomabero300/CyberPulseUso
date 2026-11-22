using CyberPulseUso.Shared.EntitiesDTO.Lnpa.Reports;
using DocumentFormat.OpenXml.Office2010.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CyberPulseUso.Backend.Helpers.Reports;

public class PdfReportBuilder
{
    private static readonly string PrimaryColor = "#1A365D";   // Azul oscuro ejecutivo
    private static readonly string AccentColor = "#3182CE";    // Azul brillante para totales
    private static readonly string BgColor = "#F7FAFC";        // Gris claro de fondo
    private static readonly string BorderColor = "#E2E8F0";    // Gris de bordes
    
    private static void ComposeHeader(IContainer container, string rutaPath, string titulo)
    {
        const float tamañoDeLetra = 0.7f;
        var imagePath = Path.Combine(rutaPath, "Images", "Reports", "Logo.jpg");

        container.Row(row =>
        {
            // IZQUIERDA: LOGO
            row.ConstantItem(65)
                .Height(1.8f, Unit.Centimetre)
                .AlignLeft()
                .AlignMiddle()
                .Image(imagePath)
                .FitArea();

            // CENTRO: TÍTULO
            row.RelativeItem()
                .AlignCenter()
                .AlignMiddle()
                .Text(titulo)
                .FontSize(16)
                .Bold()
                .FontColor(PrimaryColor)
                .AlignCenter();

            // DERECHA: INFORMACIÓN
            row.ConstantItem(155)
                .Background(BgColor)
                .Border(1)
                .BorderColor(BorderColor)
                .Padding(4)
                .AlignMiddle()
                .Column(c =>
                {
                    c.Item().Scale(tamañoDeLetra).Text(t => { t.Span("Fecha: ").Bold(); t.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}"); });
                    c.Item().Scale(tamañoDeLetra).Text(t => { t.Span("USO - ").Bold(); t.Span("Subdirectiva Arauca"); });
                    c.Item().Scale(tamañoDeLetra).Text(t => { t.Span("Carrera ").Bold(); t.Span("22 No. 25-163"); });
                    c.Item().Scale(tamañoDeLetra).Text(t => { t.Span("Nit: ").Bold(); t.Span("800.217.208-1"); });
                });
        });
    }

    private static void ComposeFooter(IContainer container, int totalRegistros)
    {
        const float tamañoDeLetraTitle = 0.8f;

        container.BorderTop(1).BorderColor(BorderColor).PaddingTop(4).Row(row =>
        {
            row.ConstantItem(150).Text("Total de registros:").FontSize(7.5f).Italic(); 
            row.RelativeItem().Text(totalRegistros.ToString()).FontSize(7.5f).Bold();
            row.RelativeItem().Scale(tamañoDeLetraTitle).Text(text =>
            {
                text.Span("Subdirectiva Arauca - ").FontSize(7.5f).Bold();
                text.CurrentPageNumber().FontSize(7.5f).Bold();
                text.Span(" de ").FontSize(7.5f).Bold();
                text.TotalPages().FontSize(7.5f).Bold();
            });
        });
    }

    private static void GenericTable<T>(IContainer container, IReadOnlyList<T> entity, IReadOnlyList<ColumnDefinitionPdf<T>> columns)
    {
        const float tamañoDeLetra = 0.6f;

        container.Table(tabla =>
        {
            // Definición dinámica de columnas
            tabla.ColumnsDefinition(columnas =>
            {
                foreach (var col in columns)
                {
                    if (col.FixedWidth.HasValue)
                        columnas.ConstantColumn(col.FixedWidth.Value);
                    else
                        columnas.RelativeColumn(col.RelativeWidth);
                }
            });

            // Encabezado
            tabla.Header(cabecera =>
            {
                foreach (var col in columns)
                {
                    cabecera.Cell()
                        .Scale(tamañoDeLetra)
                        .Element(HeaderCellStyle)
                        .AlignCenter()
                        .Text(col.Header).FontColor(Colors.White);
                }
            });

            // Filas
            foreach (var item in entity)
            {
                foreach (var col in columns)
                {
                    var cell = tabla.Cell()
                        .Scale(tamañoDeLetra)
                        .Element(BodyCellStyle);

                    cell = col.Align switch
                    {
                        TextAlign.Center => cell.AlignCenter(),
                        TextAlign.Right => cell.AlignRight(),
                        _ => cell.AlignLeft()
                    };

                    cell.Text(col.ValueSelector(item) ?? string.Empty);
                }
            }

            static IContainer HeaderCellStyle(IContainer c) =>
                c.DefaultTextStyle(x => x.SemiBold()).Background(PrimaryColor).Padding(6).AlignCenter()
                 .PaddingVertical(5)
                 .BorderColor(Colors.Black);

            static IContainer BodyCellStyle(IContainer c) =>
                c.BorderBottom(1)
                 .BorderColor(Colors.Grey.Lighten1)
                 .PaddingVertical(5);
        });
    }

    public static byte[] GeneratePdf<T>(IEnumerable<T> entity, string rutaPath, string title, IReadOnlyList<ColumnDefinitionPdf<T>> columns, bool landscape = false)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var lista = entity?.ToList() ?? new List<T>();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(landscape ? PageSizes.A4.Landscape() : PageSizes.A4);
                page.Margin(30);
                page.Header().Element(c => ComposeHeader(c, rutaPath, title));
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(5);
                    col.Item().Element(c => GenericTable(c, lista, columns));
                });
                page.Footer().Element(c => ComposeFooter(c, lista.Count));
            });
        });

        return document.GeneratePdf();
    }
}