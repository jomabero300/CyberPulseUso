using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Frontend.Services;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Text.RegularExpressions;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class Register
{
    private UserDTO userDTO = new();
    private List<Country>? countries;
    private bool loading;
    private string? imageUrl;
    private string? titleLabel;
    private MudBlazor.MudTextField<string>? myTextField;
    private Country selectedCountry = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILoginService LogInService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public bool IsAdmin { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCountriesAsync();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        titleLabel = IsAdmin ? Localizer["AdminRegister"] : Localizer["UserRegister"];
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(2);
            await myTextField!.FocusAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }
    private void ImageSelected(string imageBase64)
    {
        userDTO.Photo = imageBase64;
        imageUrl = null;
    }

    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }
        countries = responseHttp.Response;
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
    private void CountryChanged(Country country)
    {
        selectedCountry = country;
    }



    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/");
    }

    private async Task CreateUserAsync()
    {
        if (!ValidateForm())
        {
            return;
        }

        if (_sqlValidator.HasSqlInjection(userDTO.Email!) ||
            _sqlValidator.HasSqlInjection(userDTO.LastName!) ||
            _sqlValidator.HasSqlInjection(userDTO.FirstName!) ||
            _sqlValidator.HasSqlInjection(userDTO.DocumentId!) ||
            _sqlValidator.HasSqlInjection(userDTO.UserName!) ||
            _sqlValidator.HasSqlInjection(userDTO.PhotoFull!) ||
            _sqlValidator.HasSqlInjection(userDTO.Photo!) ||
            _sqlValidator.HasSqlInjection(userDTO.PhoneNumber!) ||
            _sqlValidator.HasSqlInjection(userDTO.PasswordConfirm!) ||
            _sqlValidator.HasSqlInjection(userDTO.Password))
        {
            //Datos del formulario no v lidos
            Snackbar.Add(Localizer["ERR010"], Severity.Error);
            return;
        }

        userDTO.UserType = UserType.User;
        userDTO.UserName = userDTO.Email;
        userDTO.Country = selectedCountry;
        userDTO.CountryId = selectedCountry.Id;
        userDTO.Language = System.Globalization.CultureInfo.CurrentCulture.Name.Substring(0, 2);

        if (IsAdmin)
        {
            userDTO.UserType = UserType.Admi;
        }

        loading = true;
        var responseHttp = await Repository.PostAsync<UserDTO>("/api/accounts/CreateUser", userDTO);
        loading = false;
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();

            if (message!.Contains("DuplicateUserName"))
            {
                Snackbar.Add(Localizer["EmailAlreadyExists"], Severity.Error);
                return;
            }
            if (message!.Contains("EmailDomainInvalid"))
            {
                Snackbar.Add(Localizer["EmailDomainInvalid"], Severity.Error);
                return;
            }

            Snackbar.Add(Localizer[message], Severity.Error);

            return;
        }

        NavigationManager.NavigateTo("/");
        await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["SendEmailConfirmationMessage"],
            Icon = SweetAlertIcon.Info,
        });
    }

    private bool ValidateForm()
    {
        var hasErrors = false;

        if (string.IsNullOrEmpty(userDTO.DocumentId))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["DocumentId"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.FirstName))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["FirstName"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.LastName))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["LastName"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.PhoneNumber))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["PhoneNumber"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.Email))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["Email"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.Password))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["Password"])), Severity.Error);
            hasErrors = true;
        }
        if (string.IsNullOrEmpty(userDTO.PasswordConfirm))
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["PasswordConfirm"])), Severity.Error);
            hasErrors = true;
        }
        if (selectedCountry.Id == 0)
        {
            Snackbar.Add(string.Format(Localizer["RequiredField"], string.Format(Localizer["Country"])), Severity.Error);
            hasErrors = true;
        }

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        if(!Regex.IsMatch(userDTO.Password, pattern))
        {
            Snackbar.Add(string.Format(Localizer["PasswordParameters"], string.Format(Localizer["Password"])), Severity.Error);
            hasErrors = true;
        }
        return !hasErrors;
    }
}