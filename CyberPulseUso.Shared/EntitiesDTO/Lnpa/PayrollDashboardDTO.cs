namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa;


public class PayrollDashboardDTO
{
    // Indicadores generales
    public double TotalAccrued { get; set; }
    public double TotalDeducted { get; set; }
    public double TotalNet { get; set; }

    public int SettlementCount { get; set; }
    public int WorkerCount { get; set; }

    // Información para el gráfico mensual
    public List<PayrollDashboardMonthlyDTO> MonthlyData { get; set; } = new();

    // Liquidaciones recientes
    public List<PayrollDashboardRecentDTO> RecentSettlements { get; set; } = new();
}

public class PayrollDashboardMonthlyDTO
{
    public int Year { get; set; }
    public int Month { get; set; }

    public double AccruedValue { get; set; }
    public double DeductedValue { get; set; }
    public double NetValue { get; set; }
}

public class PayrollDashboardRecentDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public string? UserName { get; set; }
    public string? ChargeName { get; set; }

    public double AccruedValue { get; set; }
    public double DeductedValue { get; set; }
    public double NetValue { get; set; }
}
