using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.LaborRemunerationParametersLnpa;

public partial class LaborRemunerationParametersForm
{
    private EditContext editContext = null!;

    private StatuDTO selectedStatu = new();
    private List<StatuDTO>? status;

    private bool loading;
    private bool _disable;

    [EditorRequired, Parameter] public LaborRemunerationParametersDTO LaborRemunerationParametersDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    public bool FormPostedSuccessfully { get; set; } = false;
    protected override void OnInitialized()
    {
        editContext = new(LaborRemunerationParametersDTO);
    }

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        await LoadStatusAsync();

        if (LaborRemunerationParametersDTO.Id > 0)
        {
            selectedStatu = status!.FirstOrDefault(x => x.Id == LaborRemunerationParametersDTO.StatuId)!;
        }
        else
        {
            _disable = true;
            selectedStatu = status!.FirstOrDefault(x => x.Name == "Activo")!;
        }

        LaborRemunerationParametersDTO.Statu = selectedStatu;
        LaborRemunerationParametersDTO.StatuId = selectedStatu.Id;
        editContext = new(LaborRemunerationParametersDTO);
        loading = false;
    }


    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formwasEditad = editContext.IsModified();

        if (!formwasEditad || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
        });

        var confirm = !string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private async Task LoadStatusAsync()
    {
        var responseHttp = await Repository.GetAsync<List<StatuDTO>>("/api/status/combo/0");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        status = responseHttp.Response;
    }
    private async Task<IEnumerable<StatuDTO>> SearchStatu(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return status!;
        }

        return status!
            .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
    private void StatuChanged(StatuDTO entity)
    {
        selectedStatu = entity;
        LaborRemunerationParametersDTO.StatuId = entity.Id;
        LaborRemunerationParametersDTO.Statu = entity;
    }

}