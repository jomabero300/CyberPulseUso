using CyberPulseUso.Shared.Entities.Lnpa;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CyberPulseUso.Backend.Helpers;

public class GeneReports
{
    private static void ComposeHeader(IContainer container, string rutaPath, string titulo)
    {
        container.Row(row =>
        {
            var tamañoDeLetra = 0.8f;
            var imagePath = $"{rutaPath}\\Images\\Reports\\Logo.jpg";

            row.RelativeItem().Column(column =>
            {
                column.Item().Scale(tamañoDeLetra).PaddingTop(1).BorderColor(Colors.Grey.Medium).Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
                column.Item().Scale(tamañoDeLetra).Text("USO - Subdirectiva Arauca");
                column.Item().Scale(tamañoDeLetra).Text("Carrera 22 No. 25-163");
                column.Item().Scale(tamañoDeLetra).Text("Nit: 000.000.000-1");
                column.Item().Scale(0.9f).Text(titulo).FontSize(15).Bold().FontColor(Colors.Blue.Darken2).AlignCenter();
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
                text.Span("Subdirectiva Arauca - ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();

            });
        });
    }


    ////CATEGORIAS
    //public static byte[] CategoryPdf(List<Category> entity, string rutaPath, string Title)
    //{
    //    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    //    var document = Document.Create(container =>
    //    {

    //        container
    //            .Page(page =>
    //            {
    //                page.Margin(30);
    //                page.Header().Element(container => ComposeHeader(container, rutaPath, Title));
    //                page.Content().Element(container => CategoryContent(container, entity));
    //                page.Footer().Element(container => ComposeFooter(container, 1));
    //            });

    //    });


    //    return document.GeneratePdf();
    //}
    //private static void CategoryContent(IContainer container, List<Category> entity)
    //{
    //    container.PaddingVertical(10).Column(columna =>
    //    {
    //        columna.Spacing(5);
    //        columna.Item().Element(container => CategoryTabla(container, entity));
    //    });
    //}
    //private static void CategoryTabla(IContainer container, List<Category> entity)
    //{
    //    container.Table(tabla =>
    //    {
    //        var tamañoDeLetra = 0.6f;

    //        tabla.ColumnsDefinition(columnas =>
    //        {
    //            columnas.ConstantColumn(50);
    //            columnas.RelativeColumn();
    //            columnas.ConstantColumn(80);
    //        });
    //        tabla.Header(cabecera =>
    //        {
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Código");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Nombre");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Estado");

    //            static IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
    //            }
    //        });

    //        foreach (var item in entity)
    //        {
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text(item.Id.ToString());
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Name);
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Statu?.Name);

    //            IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5);
    //            }
    //        }
    //    });
    //}

    //CARGOS
    //public static byte[] ChargePdf(List<Charge> entity, string rutaPath, string Title)
    //{
    //    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    //    var document = Document.Create(container =>
    //    {
    //        container
    //            .Page(page =>
    //            {
    //                page.Margin(30);
    //                page.Header().Element(container => ComposeHeader(container, rutaPath, Title));
    //                page.Content().Element(container => ChargeContent(container, entity));
    //                page.Footer().Element(container => ComposeFooter(container, 1));
    //            });
    //    });

    //    return document.GeneratePdf();
    //}
    //private static void ChargeContent(IContainer container, List<Charge> entity)
    //{
    //    container.PaddingVertical(10).Column(columna =>
    //    {
    //        columna.Spacing(5);
    //        columna.Item().Element(container => ChargeTabla(container, entity));
    //    });
    //}
    //private static void ChargeTabla(IContainer container, List<Charge> entity)
    //{
    //    container.Table(tabla =>
    //    {
    //        var tamañoDeLetra = 0.6f;

    //        tabla.ColumnsDefinition(columnas =>
    //        {
    //            columnas.ConstantColumn(50);
    //            columnas.RelativeColumn();
    //            columnas.ConstantColumn(80);
    //            columnas.ConstantColumn(50);
    //        });
    //        tabla.Header(cabecera =>
    //        {
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text("Código");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text("Nombre");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text("Grupo");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text("Estado");

    //            static IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
    //            }
    //        });

    //        foreach (var item in entity)
    //        {
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Id.ToString());
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Name);
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Group?.Name);
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Statu?.Name);

    //            IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5);
    //            }
    //        }
    //    });
    //}

    //Convenciones
    //public static byte[] ConventionPdf(List<Convention> entity, string rutaPath, string Title)
    //{
    //    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    //    var document = Document.Create(container =>
    //    {
    //        container
    //            .Page(page =>
    //            {
    //                page.Margin(30);
    //                page.Header().Element(container => ComposeHeader(container, rutaPath, Title));
    //                page.Content().Element(container => ConventionContent(container, entity));
    //                page.Footer().Element(container => ComposeFooter(container, 1));
    //            });
    //    });

    //    return document.GeneratePdf();
    //}
    //private static void ConventionContent(IContainer container, List<Convention> entity)
    //{
    //    container.PaddingVertical(10).Column(columna =>
    //    {
    //        columna.Spacing(5);
    //        columna.Item().Element(container => ConventionTabla(container, entity));
    //    });
    //}
    //private static void ConventionTabla(IContainer container, List<Convention> entity)
    //{
    //    container.Table(tabla =>
    //    {
    //        var tamañoDeLetra = 0.6f;

    //        tabla.ColumnsDefinition(columnas =>
    //        {
    //            columnas.ConstantColumn(50);
    //            columnas.RelativeColumn();
    //            columnas.ConstantColumn(50);
    //        });
    //        tabla.Header(cabecera =>
    //        {
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Código");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Nombre");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Estado");

    //            static IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
    //            }
    //        });

    //        foreach (var item in entity)
    //        {
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text(item.Id.ToString());
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Name);
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Statu?.Name);

    //            IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5);
    //            }
    //        }
    //    });
    //}

    //Convenciones Valor
    //public static byte[] ConventionValuePdf(List<ConventionValue> entity, string rutaPath, string Title)
    //{
    //    QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    //    var document = Document.Create(container =>
    //    {
    //        container
    //            .Page(page =>
    //            {
    //                page.Margin(30);
    //                page.Header().Element(container => ComposeHeader(container, rutaPath, Title));
    //                page.Content().Element(container => ConventionValueContent(container, entity));
    //                page.Footer().Element(container => ComposeFooter(container, 1));
    //            });
    //    });

    //    return document.GeneratePdf();
    //}
    //private static void ConventionValueContent(IContainer container, List<ConventionValue> entity)
    //{
    //    container.PaddingVertical(10).Column(columna =>
    //    {
    //        columna.Spacing(5);
    //        columna.Item().Element(container => ConventionValueTabla(container, entity));
    //    });
    //}
    //private static void ConventionValueTabla(IContainer container, List<ConventionValue> entity)
    //{
    //    container.Table(tabla =>
    //    {
    //        var tamañoDeLetra = 0.6f;

    //        tabla.ColumnsDefinition(columnas =>
    //        {
    //            columnas.ConstantColumn(50);
    //            columnas.RelativeColumn();
    //            columnas.ConstantColumn(40);
    //            columnas.ConstantColumn(50);
    //            columnas.ConstantColumn(50);
    //        });
    //        tabla.Header(cabecera =>
    //        {
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Código");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Convención");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Fecha");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Valor");
    //            cabecera.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text("Estado");

    //            static IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
    //            }
    //        });

    //        foreach (var item in entity)
    //        {
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text(item.Id.ToString());
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignLeft().Text(item.Convention!.Name);
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignCenter().Text(item.Date.ToString("dd/MMM/yyyy"));
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignRight().Text(item.Prince.ToString("N2"));
    //            tabla.Cell().Scale(tamañoDeLetra).Element(EstiloCelda).AlignRight().Text(item.Statu!.Name);

    //            IContainer EstiloCelda(IContainer contenedor)
    //            {
    //                return contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5);
    //            }
    //        }
    //    });
    //}
}
