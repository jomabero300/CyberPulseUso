
using CyberPulseUso.Mobile.ViewModels;

namespace CyberPulseUso.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private async void OnRegisterTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
    private async void OnRecoverPasswordTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RecoverPasswordPage));
    }

    private async void OnResendConfirmationTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ResendTokenPage));
    }

    private async void OnResendTokenTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ResendTokenPage));
    }
}