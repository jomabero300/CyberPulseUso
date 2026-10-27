using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.AuthenticationProviders;
using CyberPulseUso.Frontend.Components;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("es-CO")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
localizationOptions.RequestCultureProviders.Insert(0,
    new CustomRequestCultureProvider(context =>
    {
        var languages = context.Request.Headers.AcceptLanguage.ToString();

        var preferredLanguage = languages
            .Split(',')
            .Select(x => x.Split(';')[0].Trim())
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        var culture = preferredLanguage?.StartsWith(
            "es", StringComparison.OrdinalIgnoreCase) == true
                ? "es-CO"
                : "en-US";

        return Task.FromResult<ProviderCultureResult?>(
            new ProviderCultureResult(culture, culture));
    }));


// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddLocalization();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7093") });


builder.Services.AddScoped<IRepository, Repository>();

builder.Services.AddSweetAlert2();

builder.Services.AddAuthentication("CustomJwtScheme").AddCookie("CustomJwtScheme");


//builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderTest>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ISqlInjValRepository, SqlInjValRepository>();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();