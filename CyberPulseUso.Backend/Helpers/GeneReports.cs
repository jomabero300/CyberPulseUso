using CyberPulseUso.Shared.Entities.Lnpa;
using DocumentFormat.OpenXml.Vml.Office;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CyberPulseUso.Backend.Helpers;

public class GeneReports
{
    public static byte[] GenerarPdf(List<Category> entity, string rutaPath)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        var document = Document.Create(container =>
        {

            container
                .Page(page =>
                {
                    page.Margin(30);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(container => ConstruirContenido(container, entity));
                    page.Footer().Element(container => ComposeFooter(container, 1));
                });

        });


        return document.GeneratePdf();
    }


    private static void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                var tamañoDeLetra = 0.8f;
                var imagePath = $"{rutaPath}\\Images\\Reports\\Logo.jpg";

                row.RelativeItem().Column(column =>
                {
                    column.Item().Scale(tamañoDeLetra).Text("Grupos").FontSize(15).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Scale(tamañoDeLetra).Text("USO - Subdirectiva ARauca");
                    column.Item().Scale(tamañoDeLetra).Text("Carrera 22 No. 25-163");
                    column.Item().Scale(tamañoDeLetra).Text("Nit: 000.000.000-1");
                    column.Item().Scale(tamañoDeLetra).PaddingTop(1).BorderColor(Colors.Grey.Medium).Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });

                row.ConstantItem(100).Height(2, Unit.Centimetre).Image(imagePath); // Puedes reemplazar esto con el logo de tu empresa

            });

        }

    private static void ComposeFooter(IContainer container, int totalRegistros)
    {
        var tamañoDeLetraTitle = 0.8f;

        container.Row(row =>
        {
            row.ConstantItem(150).Text("Total de registros:");
            row.RelativeItem().Text(totalRegistros.ToString()).Bold();
            row.RelativeItem().Scale(tamañoDeLetraTitle).Text(text =>
            {
                text.Span("Seccional Arauca - ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();

            });
        });
    }

    //GUPOSS
    private static void ConstruirContenido(IContainer container, List<Category> entity)
    {
        container.PaddingVertical(40).Column(columna =>
        {
            columna.Spacing(5);
            columna.Item().Element(container => ConstruirTabla(container, entity));
        });
    }

    private static void ConstruirTabla(IContainer container, List<Category> entity)
    {
        container.Table(tabla =>
        {
            var tamañoDeLetra = 0.6f;
            var tamañoDeLetraDesignation = 0.8f;

            tabla.ColumnsDefinition(columnas =>
            {
                columnas.RelativeColumn();
                columnas.RelativeColumn();
            });
            tabla.Header(cabecera =>
            {
                cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text("Código");
                cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text("Nombre");

                static IContainer EstiloCelda(IContainer contenedor)
                {
                    return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            foreach (var item in entity)
            {
                tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Id.ToString());
                tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Name);

                IContainer EstiloCelda(IContainer contenedor)
                {
                    return contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5);
                }
            }
        });
    }
}
