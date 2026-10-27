using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class RecoverPassword
{
    private EmailDTO emailDTO = new();
    private bool loading;
    private MudBlazor.MudTextField<string>? myTextField;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

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

    private async Task SendRecoverPasswordEmailTokenAsync()
    {
        if (_sqlValidator.HasSqlInjection(emailDTO.Email))
        {
            //Datos del formulario no v lidos
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }

        emailDTO.Language = System.Globalization.CultureInfo.CurrentCulture.Name.Substring(0, 2);
        loading = true;
        var responseHttp = await repository.PostAsync("/api/accounts/RecoverPassword", emailDTO);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        MudDialog.Cancel();
        NavigationManager.NavigateTo("/");
        Snackbar.Add(Localizer["RecoverPasswordMessage"], Severity.Error);

    }

}