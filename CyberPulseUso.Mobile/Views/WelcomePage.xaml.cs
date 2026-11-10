namespace CyberPulseUso.Mobile.Views;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}
    private async void OnNextClicked(object? sender,EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}