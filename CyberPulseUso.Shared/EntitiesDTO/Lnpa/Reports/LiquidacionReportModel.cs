namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa.Reports;

public class LiquidacionReportModel
{
    public string Cargo { get; set; } = "OPERADOR TECNICO";
    public string Grupo { get; set; } = "GRUPO IV";
    public double RetencionSaludPension { get; set; } = 0;
    public double RetencionFuente { get; set; } = 0;
    public string RealizadoPor { get; set; } = "Jhonaldrin C";
    public DateTime Fecha { get; set; } = new DateTime(2026, 11, 2);

    public List<ItemLiquidacion> Items { get; set; } = new();

    // Totales calculados dinámicamente
    public double TotalIngresos => Items.Sum(i => i.DevengadoTotal);
    public double TotalDeducciones => Items.Sum(i => i.Deducido);
    public double TotalAPagar => TotalIngresos - TotalDeducciones;
}
public class ItemLiquidacion
{
    public int PayrollConceptId { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public int? Cantidad { get; set; }
    public double ValorUnitario { get; set; }
    public double DevengadoTotal { get; set; }
    public double Deducido { get; set; }
}