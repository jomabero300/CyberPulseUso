
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.Views;
using CyberPulseUso.Mobile.Views.Gene;
using CyberPulseUso.Mobile.Views.Lnpa;

namespace CyberPulseUso.Mobile;

public partial class AppShell : Shell
{
    private readonly ITokenService _tokenService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthenticationService _authenticationService;
    private bool _initializationStarted;

    public AppShell(IAuthenticationService authenticationService, ITokenService tokenService, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _authenticationService = authenticationService;

        Routing.RegisterRoute(
            nameof(LoginPage),
            typeof(LoginPage));

        Routing.RegisterRoute(
            nameof(RegisterPage),
            typeof(RegisterPage));


        Routing.RegisterRoute(
            nameof(RecoverPasswordPage),
            typeof(RecoverPasswordPage));

        Routing.RegisterRoute(
            nameof(ResendTokenPage),
            typeof(ResendTokenPage));

        Routing.RegisterRoute(
            nameof(ProfilePage),
            typeof(ProfilePage));


        Routing.RegisterRoute(
            "users",
            typeof(UsersPage));

        _tokenService = tokenService;
        _serviceProvider = serviceProvider;
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
                await NavigateToHomeAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error al restaurar la sesión: {ex.Message}");
        }
    }



    //private async void OnExitMenuItemClicked(object? sender, EventArgs e)
    //{
    //    bool confirm = await DisplayAlertAsync(
    //        "Salir",
    //        "¿Desea cerrar la aplicación?",
    //        "Sí",
    //        "No");

    //    if (!confirm)
    //        return;

    //    Application.Current?.Quit();
    //}

    private void AddHomeFlyoutItem()
    {
        // Evitar agregar Inicio más de una vez.
        if (Items.Any(x => x.Route == "Home"))
            return;

        var homeContent = new ShellContent
        {
            Title = "Inicio",
            Route = "HomePage",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<HomePage>())
        };

        var homeSection = new ShellSection
        {
            Items = { homeContent }
        };

        var homeItem = new FlyoutItem
        {
            Title = "Inicio",
            Route = "Home"
        };


        homeItem.Items.Add(homeSection);

        Items.Add(homeItem);
        AddProfileFlyoutItem();
        AddUsersFlyoutItem();
        AddCategoriesFlyoutItem();
        AddGroupsFlyoutItem();
        AddChargesFlyoutItem();
        AddConventionsFlyoutItem();
        AddConventionValuesFlyoutItem();
        AddLaborRemunerationParametersFlyoutItem();

        AddPayrollSettlementsFlyoutItem();

    }




    public async Task NavigateToHomeAsync()
    {
        var user = await _tokenService.GetUserSessionAsync();

        //await Shell.Current.DisplayAlertAsync(
        //    "Datos de sesión",
        //    $"Nombre: {user?.FullName}\n" +
        //    $"Email: [{user?.Email}]\n" +
        //    $"Tipo: [{user?.UserType}]\n" +
        //    $"Cargo: [{user?.ChargeName}]",
        //    "Aceptar");


        if (user is not null && !string.IsNullOrWhiteSpace(user.Photo))
        {
            try
            {
                var photoUrl = user.Photo.Trim().Replace('\\', '/');

                System.Diagnostics.Debug.WriteLine(
                    $"URL LIMPIA: [{photoUrl}]");

                if (Uri.TryCreate(photoUrl, UriKind.Absolute, out var photoUri))
                {
                    using var httpClient = new HttpClient();

                    var bytes = await httpClient.GetByteArrayAsync(photoUri);

                    System.Diagnostics.Debug.WriteLine(
                        $"FOTOGRAFÍA DESCARGADA: {bytes.Length} bytes");

                    user.PhotoImage = ImageSource.FromStream(
                        () => new MemoryStream(bytes));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LA URL NO ES VÁLIDA");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"ERROR CARGANDO FOTOGRAFÍA: {ex}");
            }
        }

        FlyoutHeaderView.BindingContext = user;

        AddHomeFlyoutItem();

        await GoToAsync("//Home");
    }
    private void AddProfileFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Profile"))
            return;

        var profileContent = new ShellContent
        {
            Title = "Mi perfil",
            Route = "ProfileContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<ProfilePage>())
        };

        var profileSection = new ShellSection
        {
            Items = { profileContent }
        };

        var profileItem = new FlyoutItem
        {
            Title = "Mi perfil",
            Route = "Profile"
        };

        profileItem.Items.Add(profileSection);

        Items.Add(profileItem);
    }
    private void AddUsersFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Users"))
            return;

        var usersContent = new ShellContent
        {
            Title = "Usuarios",
            Route = "UsersContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<UsersPage>())
        };

        var usersSection = new ShellSection
        {
            Items = { usersContent }
        };

        var usersItem = new FlyoutItem
        {
            Title = "Usuarios",
            Route = "Users"
        };

        usersItem.Items.Add(usersSection);

        Items.Add(usersItem);
    }
    private void AddCategoriesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Categories"))
            return;

        var categoriesContent = new ShellContent
        {
            Title = "Categorías",
            Route = "CategoriesContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<CategoriesPage>())
        };

        var categoriesSection = new ShellSection
        {
            Items = { categoriesContent }
        };

        var categoriesItem = new FlyoutItem
        {
            Title = "Categorías",
            Route = "Categories"
        };

        categoriesItem.Items.Add(categoriesSection);

        Items.Add(categoriesItem);
    }
    private void AddGroupsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Groups"))
            return;

        var groupsContent = new ShellContent
        {
            Title = "Grupos",
            Route = "GroupsContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<GroupsPage>())
        };

        var groupsSection = new ShellSection
        {
            Items = { groupsContent }
        };

        var groupsItem = new FlyoutItem
        {
            Title = "Grupos",
            Route = "Groups"
        };

        groupsItem.Items.Add(groupsSection);

        Items.Add(groupsItem);
    }
    private void AddChargesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Charges"))
            return;

        var chargesContent = new ShellContent
        {
            Title = "Cargos",
            Route = "ChargesContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<ChargesPage>())
        };

        var chargesSection = new ShellSection
        {
            Items = { chargesContent }
        };

        var chargesItem = new FlyoutItem
        {
            Title = "Cargos",
            Route = "Charges"
        };

        chargesItem.Items.Add(chargesSection);
        Items.Add(chargesItem);
    }
    private void AddConventionsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Conventions"))
            return;

        var conventionsContent = new ShellContent
        {
            Title = "Convenios",
            Route = "ConventionsContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<ConventionsPage>())
        };

        var conventionsSection = new ShellSection
        {
            Items = { conventionsContent }
        };

        var conventionsItem = new FlyoutItem
        {
            Title = "Convenios",
            Route = "Conventions"
        };

        conventionsItem.Items.Add(conventionsSection);
        Items.Add(conventionsItem);
    }
    private void AddConventionValuesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "ConventionValues"))
            return;

        var content = new ShellContent
        {
            Title = "Beneficios",
            Route = "ConventionValuesContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<ConventionValuesPage>())
        };

        var section = new ShellSection
        {
            Items = { content }
        };

        var item = new FlyoutItem
        {
            Title = "Valores de convenios",
            Route = "ConventionValues"
        };

        item.Items.Add(section);
        Items.Add(item);
    }
    private void AddLaborRemunerationParametersFlyoutItem()
    {
        if (Items.Any(x => x.Route == "LaborRemunerationParameters"))
            return;

        var content = new ShellContent
        {
            Title = "Remuneraciones",
            Route = "LaborRemunerationParametersContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<LaborRemunerationParametersPage>())
        };

        var section = new ShellSection
        {
            Items = { content }
        };

        var item = new FlyoutItem
        {
            Title = "Remuneraciones",
            Route = "LaborRemunerationParameters",
            Icon = "icon_remunerations.svg"
        };

        item.Items.Add(section);
        Items.Add(item);
    }



    private void AddPayrollSettlementsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "PayrollSettlements"))
            return;

        var payrollSettlementsContent = new ShellContent
        {
            Title = "Liquidación de nómina",
            Route = "PayrollSettlementsContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<PayrollSettlementsPage>())
        };

        var payrollSettlementsSection = new ShellSection
        {
            Items = { payrollSettlementsContent }
        };

        var payrollSettlementsItem = new FlyoutItem
        {
            Title = "Liquidación de nómina",
            Route = "PayrollSettlements"
        };

        payrollSettlementsItem.Items.Add(payrollSettlementsSection);
        Items.Add(payrollSettlementsItem);
    }
    public async Task RefreshFlyoutHeaderAsync()
    {
        var user = await _tokenService.GetUserSessionAsync();

        await Shell.Current.DisplayAlertAsync(
            "Prueba",
            $"Usuario: {user?.Email}\nFoto: {user?.Photo}",
            "Aceptar");

        FlyoutHeaderView.BindingContext = user;
        if (user is not null && !string.IsNullOrWhiteSpace(user.Photo))
        {
            try
            {
                using var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(user.Photo);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Prueba de imagen",
                        $"Imagen descargada correctamente.\nTamaño: {bytes.Length} bytes",
                        "Aceptar");
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error descargando imagen", ex.Message, "Aceptar");
            }
        }
    }
}