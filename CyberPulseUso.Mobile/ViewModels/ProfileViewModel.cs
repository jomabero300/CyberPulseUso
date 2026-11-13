using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CyberPulseUso.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IApiRepository _repository;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenService _tokenService;

    private User _user = new();

    public User User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    private ObservableCollection<Country> countries = new();

    public ObservableCollection<Country> Countries
    {
        get => countries;
        set => SetProperty(ref countries, value);
    }
    private ObservableCollection<Charge> charges = new();

    public ObservableCollection<Charge> Charges
    {
        get => charges;
        set => SetProperty(ref charges, value);
    }





    private string countryFilter = string.Empty;
    public string CountryFilter
    {
        get => countryFilter;
        set
        {
            if (SetProperty(ref countryFilter, value))
                FilterCountries();
        }
    }

    private ObservableCollection<Country> filteredCountries = new();
    public ObservableCollection<Country> FilteredCountries
    {
        get => filteredCountries;
        set => SetProperty(ref filteredCountries, value);
    }

    private string chargeFilter = string.Empty;
    public string ChargeFilter
    {
        get => chargeFilter;
        set
        {
            if (SetProperty(ref chargeFilter, value))
                FilterCharges();
        }
    }

    private ObservableCollection<Charge> filteredCharges = new();
    public ObservableCollection<Charge> FilteredCharges
    {
        get => filteredCharges;
        set => SetProperty(ref filteredCharges, value);
    }


    private bool isCountryListVisible;
    public bool IsCountryListVisible
    {
        get => isCountryListVisible;
        set => SetProperty(ref isCountryListVisible, value);
    }

    private bool isChargeListVisible;
    public bool IsChargeListVisible
    {
        get => isChargeListVisible;
        set => SetProperty(ref isChargeListVisible, value);
    }

    private string countryDisplay = "Seleccione un país";
    public string CountryDisplay
    {
        get => countryDisplay;
        set => SetProperty(ref countryDisplay, value);
    }

    private string chargeDisplay = "Seleccione un cargo";
    public string ChargeDisplay
    {
        get => chargeDisplay;
        set => SetProperty(ref chargeDisplay, value);
    }



    private Country? selectedCountry;

    public Country? SelectedCountry
    {
        get => selectedCountry;
        set
        {
            if (SetProperty(ref selectedCountry, value) && value != null)
            {
                CountryDisplay = value.Name;
                CountryFilter = string.Empty;
                IsCountryListVisible = false;
            }
        }
    }
    private Charge? selectedCharge;

    public Charge? SelectedCharge
    {
        get => selectedCharge;
        set
        {
            if (SetProperty(ref selectedCharge, value) && value != null)
            {
                ChargeDisplay = value.Name!;
                ChargeFilter = string.Empty;
                IsChargeListVisible = false;
            }
        }
    }
    private string? imageUrl;

    public string? ImageUrl
    {
        get => imageUrl;
        set => SetProperty(ref imageUrl, value);
    }

    private string errorMessage = string.Empty;

    public string ErrorMessage
    {
        get => errorMessage;
        set => SetProperty(ref errorMessage, value);
    }

    private bool isBusy;

    public bool IsBusy
    {
        get => isBusy;
        set => SetProperty(ref isBusy, value);
    }


    private ImageSource? profileImage;
    public ImageSource? ProfileImage
    {
        get => profileImage;
        set => SetProperty(ref profileImage, value);
    }



    public ProfileViewModel(IApiRepository repository, ITokenService tokenService, IAuthenticationService authenticationService)
    {
        _repository = repository;
        _tokenService = tokenService;
        _authenticationService = authenticationService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // Consultar usuario autenticado.
            var user = await _repository.GetAsync<User>("api/accounts");

            if (user is null)
            {
                ErrorMessage = "No fue posible consultar el perfil.";
                return;
            }

            User = user;

            LoadProfileImage();

            // Cargar países.
            var countriesResponse =
                await _repository.GetAsync<ObservableCollection<Country>>(
                    "api/countries/combo");

            Countries = countriesResponse ?? new ObservableCollection<Country>();

            // Cargar cargos.
            var chargesResponse =
                await _repository.GetAsync<ObservableCollection<Charge>>(
                    "api/charges/combo");

            Charges = chargesResponse ?? new ObservableCollection<Charge>();

            // Seleccionar país actual.
            SelectedCountry = Countries.FirstOrDefault(
                x => x.Id == User.CountryId);

            // Seleccionar cargo actual.
            SelectedCharge = User.ChargeId.HasValue
                ? Charges.FirstOrDefault(x => x.Id == User.ChargeId.Value)
                : null;

            FilteredCountries = new ObservableCollection<Country>(Countries);
            FilteredCharges = new ObservableCollection<Charge>(Charges);

            CountryDisplay = SelectedCountry?.Name ?? "Seleccione un país";
            ChargeDisplay = SelectedCharge?.Name ?? "Seleccione un cargo";

            // Mostrar fotografía actual.
            if (!string.IsNullOrWhiteSpace(User.Photo))
            {
                ImageUrl = $"data:image/jpeg;base64,{User.Photo}";
            }
            else
            {
                ImageUrl = null;
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"Error cargando perfil: {ex}");
            ErrorMessage = "No fue posible cargar la información del perfil.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error inesperado: {ex}");
            ErrorMessage = "Ocurrió un error al consultar el perfil.";
        }
        finally
        {
            IsBusy = false;
        }
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
            Debug.WriteLine(ex);
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
            Debug.WriteLine(ex);
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

        User.Photo = Convert.ToBase64String(memoryStream.ToArray());

        LoadProfileImage();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {


        if (IsBusy)
            return;


        if (string.IsNullOrWhiteSpace(User.FirstName))
        {
            ErrorMessage = "Ingrese los nombres.";
            return;
        }

        if (string.IsNullOrWhiteSpace(User.LastName))
        {
            ErrorMessage = "Ingrese los apellidos.";
            return;
        }

        if (SelectedCountry is null)
        {
            ErrorMessage = "Seleccione un país.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedCountry is null)
            {
                ErrorMessage = "Debe seleccionar un país.";
                return;
            }

            // Actualizar los identificadores seleccionados.
            User.CountryId = SelectedCountry.Id;
            User.ChargeId = SelectedCharge?.Id;

            // Conservar la preferencia de sesión.
            var rememberMe = await _tokenService.GetRememberMeAsync();

            // Actualizar el perfil en el Backend.
            var response = await _authenticationService.UpdateProfileAsync(
                User,
                rememberMe);

            if (response is null)
            {
                ErrorMessage = "No fue posible actualizar el perfil.";
                return;
            }

            await Shell.Current.DisplayAlertAsync("Mi perfil","Los datos se actualizaron correctamente.","Aceptar");

            // Actualizar la cabecera del menú.
            if (Shell.Current is AppShell appShell)
                await appShell.RefreshFlyoutHeaderAsync();
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"Error actualizando perfil: {ex}");
            ErrorMessage = "No fue posible actualizar los datos.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error inesperado: {ex}");
            ErrorMessage = "Ocurrió un error al guardar el perfil.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterCountries()
    {
        FilteredCountries = new ObservableCollection<Country>(
            Countries.Where(x =>
                !string.IsNullOrWhiteSpace(x.Name) &&
                x.Name.Contains(CountryFilter, StringComparison.OrdinalIgnoreCase)));
    }

    private void FilterCharges()
    {
        FilteredCharges = new ObservableCollection<Charge>(
            Charges.Where(x =>
                !string.IsNullOrWhiteSpace(x.Name) &&
                x.Name.Contains(ChargeFilter, StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    private void ToggleCountryList()
    {
        IsCountryListVisible = !IsCountryListVisible;
        IsChargeListVisible = false;
    }

    [RelayCommand]
    private void ToggleChargeList()
    {
        IsChargeListVisible = !IsChargeListVisible;
        IsCountryListVisible = false;
    }

    private void LoadProfileImage()
    {
        if (string.IsNullOrWhiteSpace(User.Photo))
        {
            ProfileImage = null;
            return;
        }

        try
        {
            var imageBytes = Convert.FromBase64String(User.Photo);

            ProfileImage = ImageSource.FromStream(
                () => new MemoryStream(imageBytes));
        }
        catch (FormatException ex)
        {
            Debug.WriteLine($"Error convirtiendo fotografía: {ex}");
            ProfileImage = null;
        }
    }

}