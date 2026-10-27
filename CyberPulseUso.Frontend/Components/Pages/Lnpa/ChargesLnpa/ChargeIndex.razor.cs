using CyberPulseUso.Frontend.Components.Pages.Shared;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.ChargesLnpa;


public partial class ChargeIndex
{
    private List<Charge>? charges { get; set; }
    private MudTable<Charge> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/charges";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter, SupplyParameterFromForm] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/TotalRecordsPaginated";
        if (!string.IsNullOrWhiteSpace(Filter)) { url += $"?filter={Filter}"; }
        var responseHttp = await repository.GetAsync<int>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        totalRecords = responseHttp.Response;
        loading = false;
    }

    private async Task<TableData<Charge>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";
        if (!string.IsNullOrWhiteSpace(Filter)) 
        { 
            url += $"&filter={Filter}"; 
        }
        
        var responseHttp = await repository.GetAsync<List<Charge>>(url);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return new TableData<Charge> { Items = [], TotalItems = 0 };
        }
        if (responseHttp.Response == null) { return new TableData<Charge> { Items = [], TotalItems = 0 }; }

        return new TableData<Charge> { Items = responseHttp.Response, TotalItems = totalRecords };
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = false, BackdropClick = false };
        IDialogReference? dialog;

        if (isEdit)
        {
            var parameters = new DialogParameters { { "Id", id } };
            dialog = await DialogService.ShowAsync<ChargeEdit>($"{Localizer["Edit"]} {Localizer["Charge"]}", parameters, options);
        }
        else
        {
            dialog = await DialogService.ShowAsync<ChargeCreate>($"{Localizer["New"]} {Localizer["Charge"]}", options);
        }

        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private async Task DeleteAsync(Charge entity)
    {
        var parameters = new DialogParameters { { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["Charge"], entity.Id) } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>(Localizer["Confirmation"], parameters, options);
        var result = await dialog.Result;

        if (result!.Canceled) { return; }

        var responseHttp = await repository.DeleteAsync($"{baseUrl}/full/{entity.Id}");
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/charges");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[message!], Severity.Error);
            }
            return;
        }

        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add(Localizer["RecordDeletedOk"], Severity.Success);
    }
}
