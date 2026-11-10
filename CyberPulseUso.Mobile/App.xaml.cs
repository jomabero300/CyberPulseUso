using System.Globalization;

namespace CyberPulseUso.Mobile;

public partial class App : Application
{
    private readonly AppShell _appShell;

    public App(AppShell appShell)
    {
        ConfigureCulture();

        InitializeComponent();

        _appShell = appShell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_appShell);
    }


    private static void ConfigureCulture()
    {
        var deviceCulture = CultureInfo.CurrentUICulture;

        var language = deviceCulture.TwoLetterISOLanguageName;

        var cultureName = language == "es"
                                        ? deviceCulture.Name
                                        : "en-US";

        var culture = new CultureInfo(cultureName);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
}
