using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.LaborRemunerationParametersValueLnpa;

public partial class LaborRemunerationParametersValueEdit
{
    private LaborRemunerationParametersValueForm? laborRemunerationParametersValueForm;
    private LaborRemunerationParametersValueDTO laborRemunerationParametersValueDTO = new();
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<LaborRemunerationParametersValueDTO>($"/api/laborremunerationparametersvalues/{Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/laborremunerationparametersvalues");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();

                Snackbar.Add(Localizer[messageError!], Severity.Error);
            }
        }
        else
        {
            laborRemunerationParametersValueDTO = responseHttp.Response!;
        }
    }

    private async Task EditAsync()
    {
        if (_sqlValidator.HasSqlInjection(laborRemunerationParametersValueDTO!.Value.ToString()))
        {
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }
        var responseHttp = await Repository.PutAsync("api/laborremunerationparametersvalues/full", laborRemunerationParametersValueDTO);

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
        laborRemunerationParametersValueForm!.FormPostedSuccessfully = true;

        MudDialog.Cancel();
    }
}