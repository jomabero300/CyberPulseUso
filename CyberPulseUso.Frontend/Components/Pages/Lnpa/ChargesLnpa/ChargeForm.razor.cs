using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.ChargesLnpa;

public partial class ChargeForm
{
    private EditContext editContext = null!;
    private bool loading;
    private bool _disable = true;
    private GroupDTO selectedGroup = new();
    private List<GroupDTO>? groups;

    private StatuDTO selectedStatu = new();
    private List<StatuDTO>? status;


    [EditorRequired, Parameter] public ChargeDTO ChargeDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        editContext = new(ChargeDTO);
    }
    protected override async Task OnInitializedAsync()
    {
        loading = true;

        await LoadStatusAsync();
        await LoadGroupsAsync();

        if (ChargeDTO.Id > 0)
        {
            selectedGroup = groups!.FirstOrDefault(x => x.Id == ChargeDTO.GroupId)!;
            ChargeDTO.Group = selectedGroup;
            ChargeDTO.GroupId = selectedGroup.Id;

            selectedStatu = status!.FirstOrDefault(x => x.Id == ChargeDTO.StatuId)!;
            _disable = false;
        }
        else
        {
            selectedGroup = null!;
            ChargeDTO.Group = null;
            ChargeDTO.GroupId = 0;

            selectedStatu = status!.FirstOrDefault(s => s.Id == 1)!;
        }

        ChargeDTO.Statu = selectedStatu;
        ChargeDTO.StatuId = selectedStatu.Id;

        loading = false;
    }
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formwasEditad = editContext.IsModified();
        if (!formwasEditad || FormPostedSuccessfully) { return; }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm) { return; }

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
        ChargeDTO.StatuId = entity.Id;
        ChargeDTO.Statu = entity;
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

        groups = responseHttp.Response;
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
        ChargeDTO.GroupId = entity.Id;
        ChargeDTO.Group = entity;
    }
}