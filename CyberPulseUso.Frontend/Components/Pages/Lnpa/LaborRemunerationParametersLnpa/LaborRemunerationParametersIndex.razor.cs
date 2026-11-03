using CyberPulseUso.Frontend.Components.Pages.Shared;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.LaborRemunerationParametersLnpa;

public partial class LaborRemunerationParametersIndex
{
    private List<LaborRemunerationParameters>? LaborRemunerationParameterss { get; set; }
    private MudTable<LaborRemunerationParameters> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private bool _isProcessing = false;
    private bool _isProcessing2 = false;

    private const string baseUrl = "api/laborremunerationparameterss";
    private string infoFormat = "{first_item}-{last_item} => {all_items}";

    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; } = null!;

    [Parameter, SupplyParameterFromForm] public string Filter { get; set; } = string.Empty;
    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }
    private async Task LoadTotalRecordsAsync()
    {
        loading = true;

        var url = $"{baseUrl}/TotalRecordsPaginated";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += "?filter=$Filter";
        }

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
    private async Task<TableData<LaborRemunerationParameters>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;

        int pageSize = state.PageSize;

        var url = $"{baseUrl}/paginated/?page={page}&recordsnumber={pageSize}";

        if (!string.IsNullOrWhiteSpace(Filter))
        {
            url += $"&filter={Filter}";
        }

        var responseHttp = await repository.GetAsync<List<LaborRemunerationParameters>>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();

            Snackbar.Add(Localizer[message!], Severity.Error);

            return new TableData<LaborRemunerationParameters> { Items = [], TotalItems = 0 };
        }

        if (responseHttp.Response == null)
        {
            return new TableData<LaborRemunerationParameters> { Items = [], TotalItems = 0 };
        }

        return new TableData<LaborRemunerationParameters>
        {
            Items = responseHttp.Response,

            TotalItems = totalRecords
        };
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

            var parameters = new DialogParameters
            {
                { "Id", id }
            };
            dialog = await DialogService.ShowAsync<LaborRemunerationParametersEdit>($"{Localizer["Edit"]} {Localizer["LaborRemunerationParameters"]}", parameters, options);
        }
        else
        {
            dialog = await DialogService.ShowAsync<LaborRemunerationParametersCreate>($"{Localizer["New"]} {Localizer["LaborRemunerationParameters"]}", options);
        }

        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            await LoadTotalRecordsAsync();

            await table.ReloadServerData();
        }
    }
    private async Task DeleteAsync(LaborRemunerationParameters entity)
    {
        var parameters = new DialogParameters
        {
            { "Message", string.Format(Localizer["DeleteConfirm"], Localizer["LaborRemunerationParameters"], entity.Name) }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };

        var dialog = await DialogService.ShowAsync<ConfirmDialog>(Localizer["Confirmation"], parameters, options);

        var result = await dialog.Result;

        if (result!.Canceled)
        {
            return;
        }


        var responseHttp = await repository.DeleteAsync($"{baseUrl}/full/{entity.Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/laborremunerationparameterss");
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

    private async Task ExportAsync(string type)
    {
        if (type == "pdf")
        {
            _isProcessing = true;
        }
        else
        {
            _isProcessing2 = true;
        }
        var response = await repository.GetBytesAsync($"api/LaborRemunerationParameterss/report/{type}");

        if (response.Error || response.Response == null)
        {
            return;
        }

        if (type == "pdf")
        {
            await JS.InvokeVoidAsync("displayPdf", response.Response);
        }
        else
        {
            string nombreArchivo = $"Categorias_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            await JS.InvokeVoidAsync("downloadFileFromBytes", response.Response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        if (type == "pdf")
        {
            _isProcessing = false;
        }
        else
        {
            _isProcessing2 = false;
        }
    }
}