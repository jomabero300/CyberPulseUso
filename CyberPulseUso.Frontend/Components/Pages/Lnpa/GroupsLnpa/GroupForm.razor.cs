using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.GroupsLnpa;

public partial class GroupForm
{
    private EditContext editContext = null!;
    private bool loading;
    private bool _disable = true;
    private CategoryDTO selectedCategory = new();
    private List<CategoryDTO>? category;

    private StatuDTO selectedStatu = new();
    private List<StatuDTO>? status;

    [EditorRequired, Parameter] public GroupDTO GroupDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    public bool FormPostedSuccessfully { get; set; } = false;

    protected override void OnInitialized()
    {
        editContext = new(GroupDTO);
    }
    protected override async Task OnInitializedAsync()
    {
        loading = true;

        await LoadStatusAsync();
        await LoadCategoryAsync();

        if (GroupDTO.Id > 0)
        {
            selectedCategory = category!.FirstOrDefault(x => x.Id == GroupDTO.CategoryId)!;
            GroupDTO.Category = selectedCategory;
            GroupDTO.CategoryId = selectedCategory.Id;

            selectedStatu = status!.FirstOrDefault(x => x.Id == GroupDTO.StatuId)!;
            _disable = false;
        }
        else
        {
            selectedCategory = null!;
            GroupDTO.Category = null;
            GroupDTO.CategoryId = 0;

            selectedStatu = status!.FirstOrDefault(s => s.Id == 1)!;
        }

        GroupDTO.Statu = selectedStatu;
        GroupDTO.StatuId = selectedStatu.Id;

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
        GroupDTO.StatuId = entity.Id;
        GroupDTO.Statu = entity;
    }

    private async Task LoadCategoryAsync()
    {
        var responseHttp = await Repository.GetAsync<List<CategoryDTO>>("/api/categories/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        category = responseHttp.Response;
    }
    private async Task<IEnumerable<CategoryDTO>> SearchCategory(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return category!;
        }

        return category!
            .Where(x => x.Name!.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
    private void CategoryChanged(CategoryDTO entity)
    {
        selectedCategory = entity;
        GroupDTO.CategoryId = entity.Id;
        GroupDTO.Category = entity;
    }

}
