using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Frontend.Services;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class Login
{
    private LoginDTO loginDTO = new();
    private bool wasClore;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    private MudBlazor.MudTextField<string>? myTextField;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(2);
            await myTextField!.FocusAsync();
        }

        await base.OnAfterRenderAsync(firstRender);

    }
    private void ShowModalRecoverPassword()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraExtraLarge };
        DialogService.ShowAsync<RecoverPassword>(Localizer["PasswordRecovery"], closeOnEscapeKey);
    }

    private void ShowModalResendConfirmationEmail()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true, MaxWidth = MaxWidth.ExtraLarge };
        DialogService.ShowAsync<ResendConfirmationEmailToken>(Localizer["MailForwarding"], closeOnEscapeKey);
    }

    private void CloseModal()
    {
        wasClore = true;
        MudDialog.Cancel();
    }

    private async Task LoginAsync()
    {
        if (wasClore)
        {
            NavigationManager.NavigateTo("/");
            return;
        }


        if (_sqlValidator.HasSqlInjection(loginDTO.Email) ||
            _sqlValidator.HasSqlInjection(loginDTO.Password))
        {
            //Datos del formulario no v lidos
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }


        try
        {
            var response = await LoginService.LoginWithCredentialsAsync("/api/accounts/Login", loginDTO);

            if (!response.Ok)
            {
                Snackbar.Add(response.Body?? "No fue posible iniciar sesión.", Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            // Esto atrapará cualquier fallo de conexión, conversión de JSON o error interno de Blazor
            Snackbar.Add($"Error crítico: {ex.Message}", Severity.Error);
            Console.WriteLine(ex.ToString()); // Volcará la traza completa en la consola del navegador (F12)
        }
    }
}