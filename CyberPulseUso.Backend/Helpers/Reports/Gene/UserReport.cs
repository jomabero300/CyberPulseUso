using CyberPulseUso.Shared.Entities.Gene;

namespace CyberPulseUso.Backend.Helpers.Reports.Gene;

public class UserReport
{
    public static byte[] UserPdf(List<User> entity, string rutaPath)
    {
        var columns = new List<ColumnDefinitionPdf<User>>
        {
            new() { Header = "Código", FixedWidth = 50, ValueSelector = c => c.Id.ToString(), Align = TextAlign.Center, HeaderAlign = TextAlign.Center },
            new() { Header = "Usuario", RelativeWidth = 1, ValueSelector = c => c.FullName! },
            new() { Header = "Contacto", RelativeWidth = 1, ValueSelector = c => c.PhoneNumber! },
            new() { Header = "Correo", RelativeWidth = 1, ValueSelector = c => c.Email! },
            new() { Header = "Correo confirmado?", RelativeWidth = 1, ValueSelector = c => c.EmailConfirmed?"Si":"No"},
            new() { Header = "Tipo", RelativeWidth = 1, ValueSelector = c => c.UserType.GetDisplayName() }
        };

        return PdfReportBuilder.GeneratePdf(entity, rutaPath, "Usuarios", columns);
    }
}
