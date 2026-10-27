using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class ChangePassword
{
    private ChangePasswordDTO changePasswordDTO = new();
    private bool loading;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    private async Task ChangePasswordAsync()
    {

        if (_sqlValidator.HasSqlInjection(changePasswordDTO.CurrentPassword) ||
            _sqlValidator.HasSqlInjection(changePasswordDTO.NewPassword)||
            _sqlValidator.HasSqlInjection(changePasswordDTO.Confirm))
        {
            //Datos del formulario no v lidos
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }

        loading = true;

        var responseHttp = await repository.PostAsync("/api/accounts/changePassword", changePasswordDTO);

        loading = false;

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        MudDialog.Cancel();
        NavigationManager.NavigateTo("/");
        Snackbar.Add(Localizer["PasswordChangedSuccessfully"], Severity.Error);
    }

    private void ReturnAction()
    {
        MudDialog.Cancel();
        NavigationManager.NavigateTo("/EditUser");
    }
}