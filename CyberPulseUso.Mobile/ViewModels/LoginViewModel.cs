using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.Views;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using System.Diagnostics;
using System.Net;

namespace CyberPulseUso.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authenticationService;

    private string email = string.Empty;
    private string password = string.Empty;
    private bool rememberMe;
    private bool isBusy;
    private string errorMessage = string.Empty;

    public string Email
    {
        get => email;
        set
        {
            if (email == value)
                return;

            email = value;
            OnPropertyChanged(nameof(Email));
        }
    }
    public string Password
    {
        get => password;
        set {
            if (password == value)
                return;
            password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public bool RememberMe
    {
        get => rememberMe;
        set
        {
            if (rememberMe == value)
                return;

            rememberMe = value;
            OnPropertyChanged(nameof(RememberMe));
        }
    }

    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (isBusy == value)
                return;

            isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            if (errorMessage == value)
                return;

            errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public LoginViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingrese el correo y la contraseña.";
            return;
        }

        try
        {
            IsBusy = true;

            var login = new LoginDTO
            {
                Email = Email.Trim(),
                Password = Password,
                RememberMe = RememberMe
            };

            var result = await _authenticationService.LoginAsync(login);

            if (result is null)
            {
                ErrorMessage = "No fue posible iniciar sesión.";
                return;
            }

            if (Shell.Current is AppShell appShell)
            {
                await appShell.NavigateToHomeAsync();
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine("========== ERROR HTTP LOGIN ==========");
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine("======================================");

            if (ex.StatusCode == HttpStatusCode.Unauthorized || ex.Message.Contains("ERR006", StringComparison.OrdinalIgnoreCase))
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
            }
            else if (ex.StatusCode == HttpStatusCode.Forbidden)
            {
                ErrorMessage = "No tienes autorización para ingresar.";
            }
            else
            {
                ErrorMessage = "No fue posible conectar con el servidor.";
            }
        }
        catch (TaskCanceledException ex)
        {
            Debug.WriteLine($"TIMEOUT LOGIN: {ex}");

            ErrorMessage = "La solicitud tardó demasiado tiempo.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine("========== ERROR LOGIN ==========");
            Debug.WriteLine(ex.ToString());
            Debug.WriteLine("=================================");

            ErrorMessage = "Ocurrió un error al iniciar sesión.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}