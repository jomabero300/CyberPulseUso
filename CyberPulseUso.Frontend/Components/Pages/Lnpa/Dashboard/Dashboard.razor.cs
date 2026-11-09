using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Globalization;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.Dashboard;

public partial class Dashboard
{
    private PayrollDashboardDTO dashboard = new();
    private bool loading = true;
    private bool isAdmin;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    private List<ChartSeries<double>> monthlySeries = new();
    private string[] monthlyLabels = [];
    private LineChartOptions monthlyChartOptions = new()
    {
        YAxisTicks = 5,
        YAxisFormat = "N0"
    };

    
    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        loading = true;

        try
        {
            var authenticationState =
                await AuthenticationStateProvider.GetAuthenticationStateAsync();

            var user = authenticationState.User;

            string url;
            
            isAdmin = user.IsInRole("Admi");

            if (isAdmin)
            {
                url = "/api/PayrollDashboard/admin";
            }
            else if (user.IsInRole("work"))
            {
                url = "/api/PayrollDashboard/worker";
            }
            else
            {
                Snackbar.Add("No tienes un perfil autorizado para consultar este dashboard.",Severity.Warning);
                return;
            }

            var responseHttp =await Repository.GetAsync<PayrollDashboardDTO>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();

                Snackbar.Add(message!, Severity.Error);
                return;
            }

            if (responseHttp.Response != null)
            {
                dashboard = responseHttp.Response;
                LoadMonthlyChart();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            loading = false;
        }
    }


    private void LoadMonthlyChart()
    {
        var culture = CultureInfo.GetCultureInfo("es-CO");

        var monthlyData = dashboard.MonthlyData
            .OrderBy(x => x.Month)
            .ToList();

        monthlyLabels = monthlyData
            .Select(x => culture.DateTimeFormat
                .GetAbbreviatedMonthName(x.Month))
            .ToArray();

        monthlySeries =
        [
            new ChartSeries<double>
        {
            Name = "Devengado",
            Data = monthlyData.Select(x => x.AccruedValue).ToArray()
        },
        new ChartSeries<double>
        {
            Name = "Deducciones",
            Data = monthlyData.Select(x => x.DeductedValue).ToArray()
        },
        new ChartSeries<double>
        {
            Name = "Neto liquidado",
            Data = monthlyData.Select(x => x.NetValue).ToArray()
        }
        ];
    }
}