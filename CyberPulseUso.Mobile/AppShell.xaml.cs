using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.Views;

namespace CyberPulseUso.Mobile;

public partial class AppShell : Shell
{
    private readonly IAuthenticationService _authenticationService;
    private bool _initializationStarted;

    public AppShell(IAuthenticationService authenticationService)
    {
        InitializeComponent();

        _authenticationService = authenticationService;

        Routing.RegisterRoute(
            nameof(LoginPage),
            typeof(LoginPage));

        Routing.RegisterRoute(
            nameof(HomePage),
            typeof(HomePage));

        Routing.RegisterRoute(
            nameof(RegisterPage),
            typeof(RegisterPage));


        Routing.RegisterRoute(
            nameof(RecoverPasswordPage),
            typeof(RecoverPasswordPage));

        Routing.RegisterRoute(
            nameof(ResendTokenPage),
            typeof(ResendTokenPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initializationStarted)
            return;

        _initializationStarted = true;

        try
        {
            var isAuthenticated =
                await _authenticationService.IsAuthenticatedAsync();

            if (isAuthenticated)
            {
                await GoToAsync(nameof(HomePage));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error al restaurar la sesión: {ex.Message}");
        }
    }
}