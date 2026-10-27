using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.LaborRemunerationParametersValueLnpa;

public partial class LaborRemunerationParametersValueForm
{
    private EditContext editContext = null!;

    private LaborRemunerationParametersDTO selectedLRP = new();
    private List<LaborRemunerationParametersDTO>? lrps;

    private GroupDTO selectedGroup = new();
    private List<GroupDTO>? groups;

    private StatuDTO selectedStatu = new();
    private List<StatuDTO>? status;

    private bool loading;
    private bool _disable;

    [EditorRequired, Parameter] public LaborRemunerationParametersValueDTO LaborRemunerationParametersValueDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        editContext = new(LaborRemunerationParametersValueDTO);
    }

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        await LoadStatusAsync();
        await LoadGroupsAsync();
        await LoadLrpsAsync();

        if (LaborRemunerationParametersValueDTO.Id > 0)
        {
            selectedGroup = groups!.FirstOrDefault(x => x.Id == LaborRemunerationParametersValueDTO.GroupId)!;
            LaborRemunerationParametersValueDTO.Group = selectedGroup;
            LaborRemunerationParametersValueDTO.GroupId = selectedGroup.Id;

            selectedLRP = lrps!.FirstOrDefault(x => x.Id == LaborRemunerationParametersValueDTO.LRPId)!;
            LaborRemunerationParametersValueDTO.LaborRemunerationParameters = selectedLRP;
            LaborRemunerationParametersValueDTO.LRPId = selectedLRP.Id;

            selectedStatu = status!.FirstOrDefault(x => x.Id == LaborRemunerationParametersValueDTO.StatuId)!;
        }
        else
        {

            selectedGroup = null!;
            LaborRemunerationParametersValueDTO.Group = null;
            LaborRemunerationParametersValueDTO.GroupId = 0;

            selectedLRP = null!;
            LaborRemunerationParametersValueDTO.LaborRemunerationParameters = null;
            LaborRemunerationParametersValueDTO.LRPId = 0;

            _disable = true;
            selectedStatu = status!.FirstOrDefault(x => x.Name == "Activo")!;
        }

        LaborRemunerationParametersValueDTO.Statu = selectedStatu;
        LaborRemunerationParametersValueDTO.StatuId = selectedStatu.Id;

        editContext = new(LaborRemunerationParametersValueDTO);

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
        LaborRemunerationParametersValueDTO.StatuId = entity.Id;
        LaborRemunerationParametersValueDTO.Statu = entity;
    }

    private async Task LoadGroupsAsync()
    {
        var responseHttp = await Repository.GetAsync<List<GroupDTO>>("/api/groups/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        groups = responseHttp.Response!;
    }
    private async Task<IEnumerable<GroupDTO>> SearchGroup(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return groups!;
        }

        return groups!
            .Where(x => x.Name!.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
    private void GroupChanged(GroupDTO entity)
    {
        selectedGroup = entity;
        LaborRemunerationParametersValueDTO.GroupId = entity.Id;
        LaborRemunerationParametersValueDTO.Group = entity;
    }

    private async Task LoadLrpsAsync()
    {
        var responseHttp = await Repository.GetAsync<List<LaborRemunerationParametersDTO>>("/api/LaborRemunerationParameterss/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        lrps = responseHttp.Response!;
    }
    private async Task<IEnumerable<LaborRemunerationParametersDTO>> SearchLRP(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return lrps!;
        }

        return lrps!
            .Where(x => x.Name!.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
    private void LRPChanged(LaborRemunerationParametersDTO entity)
    {
        selectedLRP = entity;
        LaborRemunerationParametersValueDTO.LRPId = entity.Id;
        LaborRemunerationParametersValueDTO.LaborRemunerationParameters = entity;
    }

    //OnClick="DiagnosticarErrores"

    //private void DiagnosticarErrores()
    //{
    //    // Forzamos al editContext a validar los datos en este instante
    //    bool isValid = editContext.Validate();

    //    if (!isValid)
    //    {
    //        // Obtenemos la lista de mensajes de error invisibles
    //        var errores = editContext.GetValidationMessages();

    //        foreach (var error in errores)
    //        {
    //            // Esto imprimirá el error directamente en la consola del navegador
    //            Console.WriteLine($"[DIAGNÓSTICO BLAZOR] Error de validación encontrado: {error}");
    //        }
    //    }
    //    else
    //    {
    //        Console.WriteLine("[DIAGNÓSTICO BLAZOR] El formulario es 100% válido. Si no entra a EditAsync, el problema es el enlace del EventCallback.");
    //    }
    //}
}