namespace CyberPulseUso.Backend.Helpers.Reports;

public class ColumnDefinitionPdf<T>
{
    public string Header { get; set; } = string.Empty;

    /// <summary>Selector del valor a mostrar en la celda.</summary>
    public Func<T, string> ValueSelector { get; set; } = _ => string.Empty;

    /// <summary>Ancho fijo en puntos. Si es null, se usa RelativeWidth.</summary>
    public float? FixedWidth { get; set; }

    /// <summary>Peso relativo respecto a las demás columnas relativas. Por defecto 1.</summary>
    public float RelativeWidth { get; set; } = 1f;

    /// <summary>Alineación del contenido. Por defecto izquierda.</summary>
    public TextAlign Align { get; set; } = TextAlign.Left;

    /// <summary>Alineación del encabezado. Si null usa Align.</summary>
    public TextAlign? HeaderAlign { get; set; }
}

