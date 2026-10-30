using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.PayrollSettlementLnpa;

public partial class PayrollSettlementCreate
{
    private PayrollSettlementForm? PayrollSettlementForm;
    private List<PayrollSettlementDTO> payrollSettlementDTO = new();

    [Parameter] public double WithholdingPercentage { get; set; } 
    [Parameter] public double HealthAndPensionDeductions { get; set; } 

    [Parameter] public string DialogTitle { get; set; } = string.Empty;
    [Parameter] public string ChargeName { get; set; } = string.Empty;
    [Parameter] public string OperatorName { get; set; } = string.Empty;


    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    private async Task CreateAsync()
    {
        //if (_sqlValidator.HasSqlInjection(payrollSettlementDTO!.Amount.ToString()))
        //{
        //    Snackbar.Add(Localizer["ERR010"], Severity.Error);
        //    return;
        //}
        var responseHttp = await Repository.PostAsync("/api/payrollsettlements/full", payrollSettlementDTO);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        MudDialog.Close(DialogResult.Ok(true));

        Snackbar.Add(Localizer["RecordCreateOk"], Severity.Success);

    }
    private void Return()
    {
        PayrollSettlementForm!.FormPostedSuccessfully = true;
        MudDialog.Cancel();
    }
}