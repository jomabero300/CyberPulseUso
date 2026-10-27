using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.LaborRemunerationParametersValueLnpa;

public partial class LaborRemunerationParametersValueCreate
{
    private LaborRemunerationParametersValueForm? LaborRemunerationParametersValueForm;
    private LaborRemunerationParametersValueDTO LaborRemunerationParametersValueDTO = new();
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    private async Task CreateAsync()
    {
        if (_sqlValidator.HasSqlInjection(LaborRemunerationParametersValueDTO.Value.ToString()))
        {
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }
        var responseHttp = await Repository.PostAsync("/api/laborremunerationparametersvalues/full", LaborRemunerationParametersValueDTO);

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
        LaborRemunerationParametersValueForm!.FormPostedSuccessfully = true;
        MudDialog.Cancel();
    }
}