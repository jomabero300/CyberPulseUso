using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Text.RegularExpressions;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class ResetPassword
{
    private ResetPasswordDTO resetPasswordDTO = new();
    private bool loading;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    private async Task ChangePasswordAsync()
    {
        if (!ValidateForm())
        {
            return;
        }

        if (_sqlValidator.HasSqlInjection(resetPasswordDTO.Email!)||
            _sqlValidator.HasSqlInjection(resetPasswordDTO.NewPassword) ||
            _sqlValidator.HasSqlInjection(resetPasswordDTO.ConfirmPassword))
        {
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }


        resetPasswordDTO.Token = Token;
        loading = true;
        var responseHttp = await repository.PostAsync("/api/accounts/ResetPassword", resetPasswordDTO);
        loading = false;
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Snackbar.Add(Localizer["PasswordRecoveredMessage"], Severity.Success);
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        await DialogService.ShowAsync<Login>(Localizer["Login"], closeOnEscapeKey);
    }
    private bool ValidateForm()
    {
        var hasErrors = false;

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        if (!Regex.IsMatch(resetPasswordDTO.NewPassword, pattern))
        {
            Snackbar.Add(string.Format(Localizer["PasswordParameters"], string.Format(Localizer["Password"])), Severity.Error);
            hasErrors = true;
        }

        return !hasErrors;
    }
}