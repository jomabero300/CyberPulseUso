using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace CyberPulseUso.Mobile.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IApiRepository _repository;
       
    
    [ObservableProperty]    public partial UserDTO User { get; set; } = new();

    [ObservableProperty]    public partial ObservableCollection<Country> Countries { get; set; } = new();

    [ObservableProperty]    public partial ObservableCollection<Country> FilteredCountries { get; set; } = new();

    [ObservableProperty]    public partial Country? SelectedCountry { get; set; }

    [ObservableProperty]    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]    public partial string ImageUrl { get; set; } = string.Empty;

    [ObservableProperty]    public partial bool IsBusy { get; set; }
    public bool CanRegister => !IsBusy;

    public string RegisterButtonText => IsBusy ? "Registrando..." : "Registrarme";


    public RegisterViewModel(IApiRepository repository)
    {
        _repository = repository;
    }

    public async Task InitializeAsync()
    {
        await LoadCountriesAsync();
    }

    private async Task LoadCountriesAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var result = await _repository.GetAsync<List<Country>>(
                "/api/countries/combo");

            Countries = new ObservableCollection<Country>(
                result ?? new List<Country>());

            FilteredCountries = new ObservableCollection<Country>(Countries);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            ErrorMessage = "No fue posible cargar los países.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        var filtered = string.IsNullOrWhiteSpace(value)
            ? Countries
            : Countries.Where(c =>
                c.Name.Contains(
                    value,
                    StringComparison.InvariantCultureIgnoreCase));

        FilteredCountries = new ObservableCollection<Country>(filtered);
    }

    partial void OnSelectedCountryChanged(Country? value)
    {
        if (value is not null && SearchText != value.Name)
            SearchText = value.Name;
    }


    [RelayCommand]
    private async Task SelectImageAsync()
    {
        try
        {
            var images = await MediaPicker.Default.PickPhotosAsync();

            var image = images.FirstOrDefault();

            if (image is not null)
                await ProcessImageAsync(image);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            ErrorMessage = "No fue posible seleccionar la fotografía.";
        }
    }


    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (permission != PermissionStatus.Granted)
                permission = await Permissions.RequestAsync<Permissions.Camera>();

            if (permission != PermissionStatus.Granted)
            {
                ErrorMessage = "No se concedió el permiso de cámara.";
                return;
            }

            if (!MediaPicker.Default.IsCaptureSupported)
            {
                ErrorMessage = "El dispositivo no admite captura de fotografías.";
                return;
            }

            var image = await MediaPicker.Default.CapturePhotoAsync();

            if (image is not null)
                await ProcessImageAsync(image);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            ErrorMessage = "No fue posible abrir la cámara.";
        }
    }

    private async Task ProcessImageAsync(FileResult image)
    {
        using var stream = await image.OpenReadAsync();
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        User.Photo = Convert.ToBase64String(memoryStream.ToArray());

        ImageUrl = image.FullPath;

        OnPropertyChanged(nameof(User));
    }

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task RegisterAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;

        User.DocumentId = User.DocumentId?.Trim()!;
        User.FirstName = User.FirstName?.Trim()!;
        User.LastName = User.LastName?.Trim()!;
        User.Email = User.Email?.Trim()!;
        User.UserName = User.Email;

        if (SelectedCountry is null)
        {
            ErrorMessage = "Debe seleccionar un país.";
            return;
        }

        User.CountryId = SelectedCountry.Id;
        User.Country = SelectedCountry;
        User.UserType = UserType.User;
        User.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        var validationResults = new List<ValidationResult>();

        var validationContext = new ValidationContext(User);

        if (!string.Equals(User.Password, User.PasswordConfirm, StringComparison.Ordinal))
        {
            ErrorMessage = "Las contraseñas no coinciden. Verifica ambas contraseñas.";

            await Shell.Current.DisplayAlertAsync(
                "Validación",
                ErrorMessage,
                "Aceptar");

            return;
        }
        if (!Validator.TryValidateObject(
            User,
            validationContext,
            validationResults,
            validateAllProperties: true))
        {
            ErrorMessage = validationResults.FirstOrDefault()?.ErrorMessage
                ?? "Verifique los datos ingresados.";

            return;
        }

        try
        {
            IsBusy = true;

            // El endpoint devuelve 204 No Content cuando todo sale bien.
            await _repository.PostAsync<UserDTO, object?>("/api/accounts/CreateUser",User);

            await Shell.Current.DisplayAlertAsync(
                "Registro",
                "Tu cuenta fue creada. Revisa tu correo electrónico para confirmar tu registro.",
                "Aceptar");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            ErrorMessage = "Ocurrió un error al registrar el usuario.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ReturnAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    partial void OnIsBusyChanged(bool value)
    {
        RegisterCommand.NotifyCanExecuteChanged();
    }
}