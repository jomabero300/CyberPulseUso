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

    private async Task OpenSocialNetwork(string url)
    {
        await Launcher.Default.OpenAsync(url);
    }

    private async void Facebook_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://www.facebook.com/usosubarauca/");

    private async void Instagram_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://www.instagram.com/");

    private async void TikTok_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://www.tiktok.com/");

    private async void YouTube_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://www.youtube.com/");

    private async void X_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://x.com/");

    private async void WhatsApp_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://www.whatsapp.com/");

    private async void Telegram_Clicked(object sender, EventArgs e)
        => await OpenSocialNetwork("https://telegram.org/");

}