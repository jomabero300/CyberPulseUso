using CyberPulseUso.Mobile.Configuration;
using CyberPulseUso.Mobile.Services.Authentication;
using CyberPulseUso.Mobile.Services.Implementations;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.ViewModels;
using CyberPulseUso.Mobile.ViewModels.Gene;
using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Mobile.Views;
using CyberPulseUso.Mobile.Views.Gene;
using CyberPulseUso.Mobile.Views.Lnpa;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CyberPulseUso.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<CookieContainer>();

        builder.Services.AddSingleton(sp =>
        {
            var cookieContainer =
                sp.GetRequiredService<CookieContainer>();

            var tokenService =
                sp.GetRequiredService<ITokenService>();

            var innerHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            var jwtHandler = new JwtAuthorizationHandler(
                tokenService,
                sp.GetRequiredService<ITokenRefreshService>())
            {
                InnerHandler = innerHandler
            };

            return new HttpClient(jwtHandler)
            {
                BaseAddress = new Uri(ApiConfiguration.BaseUrl),
                Timeout = TimeSpan.FromSeconds(
                    ApiConfiguration.TimeoutSeconds)
            };


        });
        builder.Services.AddTransient<IApiRepository, ApiRepository>();
        builder.Services.AddSingleton<ITokenService, TokenService>();

        builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

        builder.Services.AddSingleton<ITokenRefreshService, TokenRefreshService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<RecoverPasswordViewModel>();
        builder.Services.AddTransient<RecoverPasswordPage>();

        builder.Services.AddTransient<ResendTokenViewModel>();
        builder.Services.AddTransient<ResendTokenPage>();


        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        builder.Services.AddTransient<UsersViewModel>();
        builder.Services.AddTransient<UsersPage>();

        builder.Services.AddTransient<CategoriesViewModel>();
        builder.Services.AddTransient<CategoriesPage>();

        builder.Services.AddTransient<GroupsViewModel>();
        builder.Services.AddTransient<GroupsPage>();

        builder.Services.AddTransient<ChargesViewModel>();
        builder.Services.AddTransient<ChargesPage>();

        builder.Services.AddTransient<ConventionsViewModel>();
        builder.Services.AddTransient<ConventionsPage>();

        builder.Services.AddTransient<ConventionValuesViewModel>();
        builder.Services.AddTransient<ConventionValuesPage>();

        builder.Services.AddTransient<LaborRemunerationParametersViewModel>();
        builder.Services.AddTransient<LaborRemunerationParametersPage>();

        builder.Services.AddTransient<PayrollSettlementsPage>();
        builder.Services.AddTransient<PayrollSettlementsViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
