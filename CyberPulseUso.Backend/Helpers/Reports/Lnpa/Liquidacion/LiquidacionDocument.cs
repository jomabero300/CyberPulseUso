using CyberPulseUso.Shared.EntitiesDTO.Lnpa.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CyberPulseUso.Backend.Helpers.Reports.Lnpa.Liquidacion;

public static class LiquidacionDocument 
{
    // Paleta de colores corporativa (Tema Claro)
    private static readonly string PrimaryColor = "#1A365D";   // Azul oscuro ejecutivo
    private static readonly string AccentColor = "#3182CE";    // Azul brillante para totales
    private static readonly string BgColor = "#F7FAFC";        // Gris claro de fondo
    private static readonly string BorderColor = "#E2E8F0";    // Gris de bordes

    public static byte[] GenerarReportePdf(LiquidacionReportModel model)
    {
        // Registro automático de la Licencia Community
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape()); // Formato horizontal original
                page.Margin(30);
                page.PageColor(Colors.White);

                // Aplicación de la API moderna sin métodos obsoletos
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(9).FontColor(Colors.Grey.Darken3));

                page.Header().Element(headerContainer => ComposeHeader(headerContainer, model));
                page.Content().Element(contentContainer => ComposeContent(contentContainer, model));
                page.Footer().Element(footerContainer => ComposeFooter(footerContainer, model));
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, LiquidacionReportModel model)
    {
        container.Row(row =>
        {
            row.RelativeItem(2).Column(col =>
            {
                col.Item().Text("SUBDIRECTIVA ARAUCA").FontSize(18).Bold().FontColor(PrimaryColor);
                col.Item().Text("LIQUIDACIÓN PARCIAL TRABAJADORES CONTRATISTAS").FontSize(10).Medium().FontColor(Colors.Grey.Darken1);
            });

            row.RelativeItem(3).Background(BgColor).Border(1).BorderColor(BorderColor).Padding(8).Row(headerRow =>
            {
                headerRow.RelativeItem().Column(c =>
                {
                    c.Item().Text(t => { t.Span("CARGO: ").Bold(); t.Span(model.Cargo); });
                    c.Item().Text(t => { t.Span("GRUPO: ").Bold(); t.Span(model.Grupo); });
                });
                headerRow.RelativeItem().Column(c =>
                {
                    c.Item().Text(t => { t.Span("Retención Salud y Pensión: ").Bold(); t.Span($"{model.RetencionSaludPension}%"); });
                    c.Item().Text(t => { t.Span("Retención En La Fuente: ").Bold(); t.Span($"{model.RetencionFuente}%"); });
                });
            });
        });
    }

    private static void ComposeContent(IContainer container, LiquidacionReportModel model)
    {
        container.PaddingTop(15).Column(column =>
        {
            // Tabla de Conceptos Financieros
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    // CORRECCIÓN APLICADA: Se usa estrictamente RelativeColumn para tablas
                    columns.RelativeColumn(4); // Concepto
                    columns.RelativeColumn(1); // Cantidad
                    columns.RelativeColumn(2); // Valor Unitario
                    columns.RelativeColumn(2); // Devengado Total
                    columns.RelativeColumn(2); // Deducido
                });

                // Encabezados de la Tabla
                table.Header(header =>
                {
                    header.Cell().Element(StyleHeaderCell).Text("CONCEPTO").Bold().FontColor(Colors.White);
                    header.Cell().Element(StyleHeaderCell).Text("CANT.").Bold().FontColor(Colors.White);
                    header.Cell().Element(StyleHeaderCell).Text("VALOR UNITARIO").Bold().FontColor(Colors.White);
                    header.Cell().Element(StyleHeaderCell).Text("DEVENGADO TOTAL").Bold().FontColor(Colors.White);
                    header.Cell().Element(StyleHeaderCell).Text("DEDUCIDO").Bold().FontColor(Colors.White);

                    IContainer StyleHeaderCell(IContainer c) => c.Background(PrimaryColor).Padding(6).AlignCenter();
                });

                // Renderizado de las filas de forma dinámica
                foreach (var item in model.Items)
                {
                    table.Cell().Element(StyleCell).Text(item.Concepto);
                    table.Cell().Element(StyleCell).AlignCenter().Text(item.Cantidad?.ToString() ?? "-");
                    table.Cell().Element(StyleCell).AlignRight().Text(item.ValorUnitario > 0 ? item.ValorUnitario.ToString("C0") : "-");
                    table.Cell().Element(StyleCell).AlignRight().Text(item.DevengadoTotal > 0 ? item.DevengadoTotal.ToString("C0") : "-");
                    table.Cell().Element(StyleCell).AlignRight().Text(item.Deducido > 0 ? item.Deducido.ToString("C0") : "-");

                    IContainer StyleCell(IContainer c) => c.BorderBottom(1).BorderColor(BorderColor).Padding(5);
                }
            });

            // Resumen de Totales y Barra de Progresos Visuales
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem(2).Column(c =>
                {
                    c.Item().Text("RESUMEN DE TOTALES").FontSize(11).Bold().FontColor(PrimaryColor);

                    // Barra de progreso proporcional automatizada (Ingresos vs Deducciones)
                    double totalComposicion = model.TotalIngresos + model.TotalDeducciones;
                    float porcentajeIngresos = totalComposicion > 0 ? (float)(model.TotalIngresos / totalComposicion) : 1f;

                    c.Item().PaddingTop(5).Height(12).Background(BorderColor).Row(bar =>
                    {
                        bar.RelativeItem(porcentajeIngresos * 100).Background(AccentColor);
                        bar.RelativeItem((1 - porcentajeIngresos) * 100).Background(Colors.Red.Lighten2);
                    });
                });

                row.RelativeItem(1); // Espacio separador intermedio

                // Bloque de Cierre y Saldos Totales
                row.RelativeItem(2).Table(t =>
                {
                    t.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.RelativeColumn(); });

                    t.Cell().Padding(4).Text("TOTAL INGRESOS:").Bold();
                    t.Cell().Padding(4).AlignRight().Text(model.TotalIngresos.ToString("C0")).FontColor(AccentColor).Bold();

                    t.Cell().Padding(4).Text("TOTAL DEDUCCIONES:").Bold();
                    t.Cell().Padding(4).AlignRight().Text(model.TotalDeducciones.ToString("C0")).FontColor(Colors.Red.Medium).Bold();

                    t.Cell().Background(PrimaryColor).Padding(6).Text("TOTAL A PAGAR:").Bold().FontColor(Colors.White);
                    t.Cell().Background(PrimaryColor).Padding(6).AlignRight().Text(model.TotalAPagar.ToString("C0")).Bold().FontColor(Colors.White).FontSize(10);
                });
            });
        });
    }

    private static void ComposeFooter(IContainer container, LiquidacionReportModel model)
    {
        container.BorderTop(1).BorderColor(BorderColor).PaddingTop(5).Row(row =>
        {
            row.RelativeItem().Text($"Realizado por: model.RealizadoPor").FontSize(8).Italic();
            row.RelativeItem().AlignRight().Text($"Fecha de Emisión: {model.Fecha:dd 'de' MMMM 'de' yyyy}").FontSize(8);
        });
    }
}