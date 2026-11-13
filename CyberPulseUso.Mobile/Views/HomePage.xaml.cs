using CyberPulseUso.Mobile.Services;
using CyberPulseUso.Mobile.Services.Interfaces;

namespace CyberPulseUso.Mobile.Views;

public partial class HomePage : ContentPage
{
    private readonly IAuthenticationService _authenticationService;
    private bool _isLoggingOut;

    public HomePage(IAuthenticationService authenticationService)
    {
        InitializeComponent();

        _authenticationService = authenticationService;
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        if (_isLoggingOut)
            return;

        bool confirm = await DisplayAlertAsync("Cerrar sesión", "¿Está seguro de que desea cerrar sesión?", "Sí, salir", "Cancelar");

        if (!confirm)
            return;

        _isLoggingOut = true;

        try
        {
            await _authenticationService.LogoutAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error al cerrar sesión en el servidor: {ex.Message}");
        }
        finally
        {
            // Regresar a Welcome incluso si falla la petición al backend.
            await Shell.Current.GoToAsync("//Welcome");

            _isLoggingOut = false;
        }
    }
}