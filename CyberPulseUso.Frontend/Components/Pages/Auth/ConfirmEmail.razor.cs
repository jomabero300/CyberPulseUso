using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class ConfirmEmail
{
    private string? message;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = null!;

    protected async Task ConfirmAccountAsync()
    {
        var responseHttp = await repository.GetAsync($"/api/accounts/confirmEmail/?userId={UserId}&token={Token}");

        if (responseHttp.Error)
        {
            message = await responseHttp.GetErrorMessageAsync();
            NavigationManager.NavigateTo("/");
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        Snackbar.Add(Localizer["ConfirmedEmailMessage"], Severity.Success);
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<Login>(Localizer["Login"], closeOnEscapeKey);
    }
}