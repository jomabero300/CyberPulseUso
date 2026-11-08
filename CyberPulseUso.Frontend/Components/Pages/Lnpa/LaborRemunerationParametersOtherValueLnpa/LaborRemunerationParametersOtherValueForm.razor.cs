using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.LaborRemunerationParametersOtherValueLnpa;


public partial class LaborRemunerationParametersOtherValueForm
{
    private List<LaborRemunerationParametersOtherValueDTO>laborRemunerationParametersOtherValueDTO = new();
    private bool loading = true;

    [Inject] private NavigationManager navigationManager { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;


    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadLaborRemunerationParametersOtherValuesAsync();
        }
        finally
        {
            loading = false;
        }
    }


    private async Task LoadLaborRemunerationParametersOtherValuesAsync()
    {
        var responseHttp = await repository.GetAsync<List<LaborRemunerationParametersOtherValueDTO>>("/api/LaborRemunerationParametersOtherValues");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();

            await SweetAlertService.FireAsync("Error",message,SweetAlertIcon.Error);

            return;
        }

        laborRemunerationParametersOtherValueDTO =responseHttp.Response ?? new();
    }


    private async Task SaveUserAsync()
    {

        var model = laborRemunerationParametersOtherValueDTO.Select(x => new LaborRemunerationParametersOtherValueCRUDDTO
        {
            Id=x.Id,
            LRPId= x.LRPId,
            StatuId = x.StatuId,
            Value = x.Value,
        }).ToList();

        var responseHttp = await repository.PostAsync("/api/laborremunerationparametersothervalues/full", model);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();

            Snackbar.Add(Localizer[message!],Severity.Error);

            return;
        }


        Snackbar.Add(Localizer["RecordSavedOk"],Severity.Success);
        navigationManager.NavigateTo("/");
    }

    private void ReturnAction()
    {
        navigationManager.NavigateTo("/");
    }
}
