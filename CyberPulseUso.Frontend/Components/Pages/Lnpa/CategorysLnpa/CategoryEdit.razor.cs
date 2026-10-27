using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.CategorysLnpa;

public partial class CategoryEdit
{
    private CategoryForm? categoryFormRef;
    private CategoryDTO? categoryDTO;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter] public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<CategoryDTO>("/api/categories/" + Id);
        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/categories");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(Localizer[messageError!], Severity.Error);
            }
        }
        else
        {
            categoryDTO = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {

        if (_sqlValidator.HasSqlInjection(categoryDTO!.Name!))
        {
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }
        var responseHttp = await Repository.PutAsync("/api/categories/full", categoryDTO);
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
        MudDialog.Cancel();
    }
}
