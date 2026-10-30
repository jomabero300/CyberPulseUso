using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Frontend.Services;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

[Authorize]
public partial class EditUser
{
    private User? user;
    private bool loading = true;
    private string? imageUrl;
    private List<Country>? countries;
    private Country selectedCountry = new();
    private List<Charge>? charges;
    private Charge selectedCharge = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsync();
        await LoadCountiesAsync();
        await LoadChargesAsync();

        selectedCountry = user!.Country!;

        if(user.ChargeId != null)
        {
            selectedCharge = charges!.FirstOrDefault(x => x.Id == user.ChargeId)!;
            //user.Charge = selectedCharge;
            //user.ChargeId = selectedCharge.Id;
        }

        if (!string.IsNullOrWhiteSpace(user!.Photo))
        {
            imageUrl = user.Photo;
            user.Photo = null;
        }
    }
    private void ShowModal()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };

        DialogService.ShowAsync<ChangePassword>(Localizer["ChangePassword"], closeOnEscapeKey);
    }
    private async Task LoadUserAsync()
    {
        var responseHttp = await repository.GetAsync<User>($"/api/accounts");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            var messageError = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(messageError!, Severity.Error);
            return;
        }

        user = responseHttp.Response;

        loading = false;
    }
    private void ImageSelected(string imageBase64)
    {
        user!.Photo = imageBase64;
        imageUrl = null;
    }

    private async Task LoadCountiesAsync()
    {
        var responseHttp = await repository.GetAsync<List<Country>>("/api/countries/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        countries = responseHttp.Response;
    }
    private void CountryChanged(Country country)
    {
        selectedCountry = country;
    }
    private async Task<IEnumerable<Country>> SearchCountries(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return countries!;
        }

        return countries!
            .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }


    private async Task LoadChargesAsync()
    {
        var responseHttp = await repository.GetAsync<List<Charge>>("/api/charges/combo");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        charges = responseHttp.Response;
    }
    private void ChargeChanged(Charge charge)
    {
        selectedCharge = charge;
        user!.ChargeId = charge.Id;
        user.Charge = charge;
    }
    private async Task<IEnumerable<Charge>> SearchCharges(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return charges!;
        }

        return charges!
            .Where(c => c.Name!.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }


    private async Task SaveUserAsync()
    {
        if (_sqlValidator.HasSqlInjection(user!.FirstName) ||
            _sqlValidator.HasSqlInjection(user.LastName) ||
            _sqlValidator.HasSqlInjection(user.PhoneNumber!))
        {
            //Datos del formulario no v lidos
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }

        var responseHttp = await repository.PutAsync<User, TokenDTO>("/api/accounts", user!);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        await LoginService.LoginAsync(responseHttp.Response!.Token);

        Snackbar.Add(Localizer["RecordSavedOk"], Severity.Success);

        NavigationManager.NavigateTo("/");
    }
    
    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/");
    }
}