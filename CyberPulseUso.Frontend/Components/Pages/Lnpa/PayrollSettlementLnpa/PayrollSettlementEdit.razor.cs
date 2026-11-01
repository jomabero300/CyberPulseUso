using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.PayrollSettlementLnpa;

public partial class PayrollSettlementEdit
{
    private PayrollSettlementForm? PayrollSettlementForm;

    private List<PayrollSettlementDTO> payrollSettlementDTO = new();
    
    [Parameter] public DateTime Date { get; set; }
    [Parameter] public int ChargeId { get; set; }
    [Parameter] public string UserId { get; set; }
    [Parameter] public string DialogTitle { get; set; } = string.Empty;
    [Parameter] public string ChargeName { get; set; } = string.Empty;
    [Parameter] public string OperatorName { get; set; } = string.Empty;
    [Parameter] public double WithholdingPercentage { get; set; } 
    [Parameter] public double HealthAndPensionDeductions { get; set; }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    protected override async Task OnInitializedAsync()
    {
        string fechaFormateada = Date.ToString("yyyy-MM-ddTHH:mm:ss");
        int id = ChargeId;
        var responseHttp = await Repository.GetAsync<List<PayrollSettlementDTO>>($"/api/payrollsettlements/full/{fechaFormateada}/{id}/{UserId}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/payrollsettlements");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();

                Snackbar.Add(Localizer[messageError!], Severity.Error);
            }
        }
        else
        {
            payrollSettlementDTO = responseHttp.Response!;
        }
    }

    private async Task EditAsync()
    {
        //if (_sqlValidator.HasSqlInjection(payrollSettlementDTO!.Amount.ToString()))
        //{
        //    Snackbar.Add(Localizer["ERR010"], Severity.Error);
        //    return;
        //}
        var responseHttp = await Repository.PutAsync("api/payrollsettlements/full", payrollSettlementDTO);

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();

            Snackbar.Add(Localizer[messageError!], Severity.Error);

            return;
        }
        MudDialog.Close(DialogResult.Ok(true));

        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);

    }

    private void Return()
    {
        PayrollSettlementForm!.FormPostedSuccessfully = true;

        MudDialog.Cancel();
    }
}