using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CyberPulseUso.Backend.Helpers.Reports;

public class PdfReportBuilder
{
    private static void ComposeHeader(IContainer container, string rutaPath, string titulo)
    {
        container.Row(row =>
        {
            const float tamañoDeLetra = 0.8f;
            var imagePath = Path.Combine(rutaPath, "Images", "Reports", "Logo.jpg");

            row.RelativeItem().Column(column =>
            {
                column.Item().Scale(tamañoDeLetra).PaddingTop(1)
                    .Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                column.Item().Scale(tamañoDeLetra).Text("USO - Subdirectiva Arauca");
                column.Item().Scale(tamañoDeLetra).Text("Carrera 22 No. 25-163");
                column.Item().Scale(tamañoDeLetra).Text("Nit: 000.000.000-1");
                column.Item().Scale(0.9f).Text(titulo)
                    .FontSize(15).Bold().FontColor(Colors.Blue.Darken2).AlignCenter();
            });

            row.ConstantItem(100).Height(2, Unit.Centimetre).Image(imagePath);
        });
    }
    private static void ComposeFooter(IContainer container, int totalRegistros)
    {
        const float tamañoDeLetraTitle = 0.8f;

        container.Row(row =>
        {
            row.ConstantItem(150).Text("Total de registros:");
            row.RelativeItem().Text(totalRegistros.ToString()).Bold();
            row.RelativeItem().Scale(tamañoDeLetraTitle).Text(text =>
            {
                text.Span("Subdirectiva Arauca - ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        });
    }

    private static void GenericTable<T>(
        IContainer container,
        IReadOnlyList<T> entity,
        IReadOnlyList<ColumnDefinitionPdf<T>> columns)
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
                        .Text(col.Header);
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
                c.DefaultTextStyle(x => x.SemiBold())
                 .PaddingVertical(5)
                 .BorderBottom(1)
                 .BorderColor(Colors.Black);

            static IContainer BodyCellStyle(IContainer c) =>
                c.BorderBottom(1)
                 .BorderColor(Colors.Grey.Lighten1)
                 .PaddingVertical(5);
        });
    }

    public static byte[] GeneratePdf<T>(
        IEnumerable<T> entity,
        string rutaPath,
        string title,
        IReadOnlyList<ColumnDefinitionPdf<T>> columns)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var lista = entity?.ToList() ?? new List<T>();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
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