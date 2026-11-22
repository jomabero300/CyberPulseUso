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
                page.Size(PageSizes.A4); // Formato horizontal original .Landscape()
                page.Margin(20);
                page.PageColor(Colors.White);

                // Aplicación de la API moderna sin métodos obsoletos
                page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(8.5f).FontColor(Colors.Grey.Darken3));
                
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
            // Primero: Logo a la izquierda
            row.RelativeItem(1)
                .AlignLeft()
                .AlignMiddle()
                .Height(55)
                .Image("wwwroot/Images/Reports/Logo.jpg")
                .FitArea();

            // Segundo: Títulos centrados
            row.RelativeItem(2)
                .AlignCenter()
                .AlignMiddle()
                .Column(col =>
                {
                    col.Item().AlignCenter().Text("SUBDIRECTIVA ARAUCA")
                        .FontSize(16)
                        .Bold()
                        .FontColor(PrimaryColor);

                    col.Item().AlignCenter().Text("LIQUIDACIÓN PARCIAL TRABAJADORES CONTRATISTAS")
                        .FontSize(9)
                        .Medium()
                        .FontColor(Colors.Grey.Darken1);
                });

            // Tercero: Cargo y grupo a la derecha
            row.RelativeItem(2)
                .AlignRight()
                .AlignMiddle()
                .Background(BgColor)
                .Border(1)
                .BorderColor(BorderColor)
                .Padding(6)
                .Column(c =>
                {
                    c.Item().Text(t =>
                    {
                        t.Span("CARGO: ").Bold();
                        t.Span(model.Cargo);
                    });

                    c.Item().Text(t =>
                    {
                        t.Span("GRUPO: ").Bold();
                        t.Span(model.Grupo);
                    });
                });
        });
    }

    private static void ComposeContent(IContainer container, LiquidacionReportModel model)
    {
        container.Layers(layers =>
        {
            // Contenido principal del reporte
            layers.PrimaryLayer().PaddingTop(15).Column(column =>
            {
                // Aquí conservas SIN CAMBIOS tu tabla de conceptos financieros
                // y el bloque de resumen de totales.

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(StyleHeaderCell).Text("CONCEPTO").Bold().FontColor(Colors.White);
                        header.Cell().Element(StyleHeaderCell).Text("CANT.").Bold().FontColor(Colors.White);
                        header.Cell().Element(StyleHeaderCell).Text("VALOR UNITARIO").Bold().FontColor(Colors.White);
                        header.Cell().Element(StyleHeaderCell).Text("DEVENGADO TOTAL").Bold().FontColor(Colors.White);
                        header.Cell().Element(StyleHeaderCell).Text("DEDUCIDO").Bold().FontColor(Colors.White);

                        IContainer StyleHeaderCell(IContainer c) =>
                            c.Background(PrimaryColor).Padding(6).AlignCenter();
                    });

                    foreach (var item in model.Items)
                    {
                        table.Cell().Element(StyleCell).Text(item.Concepto);
                        table.Cell().Element(StyleCell).AlignCenter().Text(item.Cantidad?.ToString() ?? "-");
                        table.Cell().Element(StyleCell).AlignRight().Text(item.ValorUnitario > 0 ? item.ValorUnitario.ToString("C0") : "-");
                        table.Cell().Element(StyleCell).AlignRight().Text(item.DevengadoTotal > 0 ? item.DevengadoTotal.ToString("C0") : "-");
                        table.Cell().Element(StyleCell).AlignRight().Text(item.Deducido > 0 ? item.Deducido.ToString("C0") : "-");

                        IContainer StyleCell(IContainer c) =>
                            c.BorderBottom(1).BorderColor(BorderColor).Padding(3.5f);
                    }
                });

                // Resumen de totales
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem(2).Column(c =>
                    {
                        c.Item().Text("RESUMEN DE TOTALES")
                            .FontSize(10).Bold().FontColor(PrimaryColor);

                        double totalComposicion = model.TotalIngresos + model.TotalDeducciones;

                        float porcentajeIngresos = totalComposicion > 0
                            ? (float)(model.TotalIngresos / totalComposicion)
                            : 1f;

                        c.Item().PaddingTop(4).Height(10).Background(BorderColor).Row(bar =>
                        {
                            bar.RelativeItem(porcentajeIngresos * 100).Background(AccentColor);
                            bar.RelativeItem((1 - porcentajeIngresos) * 100).Background(Colors.Red.Lighten2);
                        });
                    });

                    row.RelativeItem(1);

                    row.RelativeItem(2).Table(t =>
                    {
                        t.ColumnsDefinition(cd =>
                        {
                            cd.RelativeColumn();
                            cd.RelativeColumn();
                        });

                        t.Cell().Padding(3).Text("TOTAL INGRESOS:").Bold();
                        t.Cell().Padding(3).AlignRight().Text(model.TotalIngresos.ToString("C0")).FontColor(AccentColor).Bold();

                        t.Cell().Padding(3).Text("TOTAL DEDUCCIONES:").Bold();
                        t.Cell().Padding(3).AlignRight().Text(model.TotalDeducciones.ToString("C0")).FontColor(Colors.Red.Medium).Bold();

                        t.Cell().Background(PrimaryColor).Padding(5).Text("TOTAL A PAGAR:").Bold().FontColor(Colors.White);
                        t.Cell().Background(PrimaryColor).Padding(5).AlignRight().Text(model.TotalAPagar.ToString("C0")).Bold().FontColor(Colors.White).FontSize(9.5f);
                    });
                });

                // Leyenda informativa
                column.Item()
                    .PaddingTop(20)
                    .AlignCenter()
                    .Text("Liquidación parcial con fines informativos")
                    .FontSize(9)
                    .Italic()
                    .FontColor(Colors.Grey.Darken1);
            });

            // Marca de agua diagonal
            layers.Layer()
                .AlignCenter()
                .AlignMiddle()
                .Rotate(-45)
                .Text("SIMULACIÓN")
                .FontSize(55)
                .Bold()
                .FontColor(Colors.Grey.Lighten3);
        });
    }

    private static void ComposeFooter(IContainer container, LiquidacionReportModel model)
    {
        container.BorderTop(1).BorderColor(BorderColor).PaddingTop(4).Row(row =>
        {
            row.RelativeItem().Text($"Realizado por: {model.RealizadoPor}").FontSize(7.5f).Italic();
            row.RelativeItem().AlignRight().Text($"Fecha de Emisión: {model.Fecha:dd 'de' MMMM 'de' yyyy}").FontSize(7.5f);
        });
    }
}