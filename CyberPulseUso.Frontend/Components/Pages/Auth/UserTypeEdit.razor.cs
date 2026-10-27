using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class UserTypeEdit
{
    private UserTypeForm? userTypeForm;

    private User? user;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter] public string Id { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var responseHttp = await Repository.GetAsync<User>($"/api/accounts/UserType?id={Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/users");
            }
            else
            {
                var messageError = await responseHttp.GetErrorMessageAsync();

                Snackbar.Add(Localizer[messageError!], Severity.Error);
            }
        }
        else
        {
            user = responseHttp.Response;
        }
    }

    private async Task EditAsync()
    {
        var responseHttp = await Repository.GetAsync($"api/accounts/UserTypeUp?id={user!.Id}&userType={user.UserType}");

        if (responseHttp.Error)
        {
            var messageError = await responseHttp.GetErrorMessageAsync();

            Snackbar.Add(Localizer[messageError!], Severity.Error);

            return;
        }
        Return();

        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);

    }

    private void Return()
    {
        userTypeForm!.FormPostedSuccessfully = true;

        NavigationManager.NavigateTo("/users");
    }
}