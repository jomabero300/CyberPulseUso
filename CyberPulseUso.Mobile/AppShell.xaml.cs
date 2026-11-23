using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.Views;
using CyberPulseUso.Mobile.Views.Gene;
using CyberPulseUso.Mobile.Views.Lnpa;
using CyberPulseUso.Mobile.Views.Lnpa.Categories;
using CyberPulseUso.Mobile.Views.Lnpa.Charges;
using CyberPulseUso.Mobile.Views.Lnpa.Conventions;
using CyberPulseUso.Mobile.Views.Lnpa.ConventionValues;
using CyberPulseUso.Mobile.Views.Lnpa.Groups;
using CyberPulseUso.Mobile.Views.Lnpa.LaborRemunerationParameter;
using CyberPulseUso.Mobile.Views.Lnpa.LaborRemunerationParametersValues;
using CyberPulseUso.Mobile.Views.Lnpa.LaborRemunerationReport;
using CyberPulseUso.Mobile.Views.Lnpa.Normatives;
using CyberPulseUso.Mobile.Views.Lnpa.PayrollConcepts;
using CyberPulseUso.Shared.Resources;
using System.Globalization;

namespace CyberPulseUso.Mobile;

public partial class AppShell : Shell
{
    private readonly ITokenService _tokenService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAuthenticationService _authenticationService;
    private bool _initializationStarted;
    private bool _isLoggingOut;
    private ShellItem? _welcomeShellItem;

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
        _welcomeShellItem = Items.FirstOrDefault();
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

    private void AddHomeFlyoutItem(string userType)
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
            Route = "Home",
            Icon = "icon_home.svg"
        };

        homeItem.Items.Add(homeSection);
        Items.Add(homeItem);

        // El perfil siempre está disponible.
        AddProfileFlyoutItem();
        AddNormativesFlyoutItem();
        // Normalizar el perfil recibido del JWT.
        var role = userType?.Trim();

        bool isAdmin =
            string.Equals(role, "SAdm", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role, "Admi", StringComparison.OrdinalIgnoreCase);

        bool isWorker =
            string.Equals(role, "work", StringComparison.OrdinalIgnoreCase);

        // Módulos exclusivos de administradores.
        if (isAdmin)
        {
            AddUsersFlyoutItem();
            AddCategoriesFlyoutItem();
            AddGroupsFlyoutItem();
            AddChargesFlyoutItem();
            AddConventionsFlyoutItem();
            AddConventionValuesFlyoutItem();
            AddLaborRemunerationParametersFlyoutItem();
            AddLaborRemunerationParametersValuesFlyoutItem();
            AddPayrollConceptsFlyoutItem();
            AddLaborRemunerationParametersOtherValuesFlyoutItem();
            AddLaborRemunerationReportFlyoutItem();
            // Consulta de documentos normativos para todos los perfiles.
        }

        // Administradores y trabajadores pueden consultar liquidaciones.
        if (isAdmin || isWorker)
        {
            AddPayrollSettlementsFlyoutItem();
        }
    }

    private void AddLaborRemunerationParametersOtherValuesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "LaborRemunerationParametersOtherValues"))
            return;

        var title = GetTitle("laborremunerationparametersothervalues", "Paramtros1");

        var content = new ShellContent
        {
            Title = title,
            Route = "LaborRemunerationParametersOtherValuesContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<LaborRemunerationParametersOtherValuesPage>())
        };
        var section = new ShellSection
        {
            Items = { content }
        };
        var item = new FlyoutItem
        {
            Title = title,
            Route = "LaborRemunerationParametersOtherValues",
            Icon = "icon_remunerations.svg"
        };
        item.Items.Add(section);
        Items.Add(item);
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

        AddHomeFlyoutItem(user?.UserType ?? string.Empty);

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
            Route = "Profile",
            Icon = "icon_profile.svg"
        };

        profileItem.Items.Add(profileSection);

        Items.Add(profileItem);
    }
    private void AddUsersFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Users"))
            return;

        var title = GetTitle("Users", "Usuarios1");

        var usersContent = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "Users",
            Icon = "icon_users.svg"
        };

        usersItem.Items.Add(usersSection);

        Items.Add(usersItem);
    }
    private void AddCategoriesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Categories"))
            return;

        var title = GetTitle("Categories", "Categorías1");

        var categoriesContent = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "Categories",
            Icon = "icon_categories.svg"
        };

        categoriesItem.Items.Add(categoriesSection);

        Items.Add(categoriesItem);
    }
    private void AddGroupsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Groups"))
            return;

        var title = GetTitle("Groups", "Grupos1");

        var groupsContent = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "Groups",
            Icon = "icon_groups.svg"
        };

        groupsItem.Items.Add(groupsSection);

        Items.Add(groupsItem);
    }
    private void AddChargesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Charges"))
            return;

        var title = GetTitle("Charges", "Cargos1");

        var chargesContent = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "Charges",
            Icon = "icon_charges.svg"
        };

        chargesItem.Items.Add(chargesSection);
        Items.Add(chargesItem);
    }
    private void AddConventionsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Conventions"))
            return;

        var title = GetTitle("Conventions", "Convenciones1");

        var conventionsContent = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "Conventions",
            Icon = "icon_conventions.svg"
        };

        conventionsItem.Items.Add(conventionsSection);
        Items.Add(conventionsItem);
    }
    private void AddConventionValuesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "ConventionValues"))
            return;

        var title = GetTitle("ConventionValues", "Beneficios1");

        var content = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "ConventionValues",
            Icon = "icon_benefits.svg"
        };

        item.Items.Add(section);
        Items.Add(item);
    }
    private void AddLaborRemunerationParametersFlyoutItem()
    {
        if (Items.Any(x => x.Route == "LaborRemunerationParameters"))
            return;

        var title = GetTitle("LaborRemunerationParameters", "Remuneraciones1");

        var content = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "LaborRemunerationParameters",
            Icon = "icon_remunerations.svg"
        };

        item.Items.Add(section);
        Items.Add(item);
    }
    private void AddLaborRemunerationParametersValuesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "LaborRemunerationParametersValues"))
            return;

        var title = GetTitle("LaborRemunerationParametersValues", "Valores de remuneraciones1");

        var content = new ShellContent
        {
            Title = title,
            Route = "LaborRemunerationParametersValuesContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<LaborRemunerationParametersValuesPage>())
        };

        var section = new ShellSection
        {
            Items = { content }
        };

        var item = new FlyoutItem
        {
            Title = title,
            Route = "LaborRemunerationParametersValues",
            Icon = "icon_remunerations.svg"
        };

        item.Items.Add(section);
        Items.Add(item);
    }
    private void AddPayrollSettlementsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "PayrollSettlements"))
            return;

        var title = GetTitle("Payrollsettlements", "Liquidación de nómina1");
                              
        var payrollSettlementsContent = new ShellContent
        {
            Title = title,
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
            Title = title,
            Route = "PayrollSettlements",
            Icon = "icon_payroll.svg"
        };

        payrollSettlementsItem.Items.Add(payrollSettlementsSection);
        Items.Add(payrollSettlementsItem);
    }
    private void AddPayrollConceptsFlyoutItem()
    {
        if (Items.Any(x => x.Route == "PayrollConcepts"))
            return;

        var title = GetTitle("PayrollConcepts", "Conceptos de nómina1");

        var payrollConceptsContent = new ShellContent
        {
            Title = title,
            Route = "PayrollConceptsContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<PayrollConceptsPage>())
        };
        var payrollConceptsSection = new ShellSection
        {
            Items = { payrollConceptsContent }
        };
        var payrollConceptsItem = new FlyoutItem
        {
            Title = title,
            Route = "PayrollConcepts",
            Icon = "icon_payroll.svg"
        };
        payrollConceptsItem.Items.Add(payrollConceptsSection);
        Items.Add(payrollConceptsItem);
    }
    private void AddLaborRemunerationReportFlyoutItem()
    {
        if (Items.Any(x => x.Route == "LaborRemunerationReport"))
            return;

        var title = GetTitle(
            "LaborRemunerationReport",
            "Remuneración laboral");

        var content = new ShellContent
        {
            Title = title,
            Route = "LaborRemunerationReportContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<LaborRemunerationReportPage>())
        };

        var section = new ShellSection
        {
            Items = { content }
        };

        var item = new FlyoutItem
        {
            Title = title,
            Route = "LaborRemunerationReport",
            Icon = "icon_remunerations.svg"
        };

        item.Items.Add(section);
        Items.Add(item);
    }
    private void AddNormativesFlyoutItem()
    {
        if (Items.Any(x => x.Route == "Normatives"))
            return;

        var content = new ShellContent
        {
            Title = "Documentos normativos",
            Route = "NormativesContent",
            ContentTemplate = new DataTemplate(
                () => _serviceProvider.GetRequiredService<NormativesPage>())
        };

        var section = new ShellSection
        {
            Items = { content }
        };

        var item = new FlyoutItem
        {
            Title = "Documentos normativos",
            Route = "Normatives"
        };

        item.Items.Add(section);
        Items.Add(item);
    }

    private async void OnLogoutMenuItemClicked(object? sender, EventArgs e)
    {
        if (_isLoggingOut)
            return;
        
        FlyoutIsPresented = false;

        bool confirm = await DisplayAlertAsync(
            "Cerrar sesión",
            "¿Está seguro de que desea cerrar sesión?",
            "Sí, salir",
            "Cancelar");

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
            FlyoutIsPresented = false;

            ClearUserMenu();

            await GoToAsync("//Welcome");

            _isLoggingOut = false;
        }
    }

    public async Task RefreshFlyoutHeaderAsync()
    {
        var user = await _tokenService.GetUserSessionAsync();

        //await Shell.Current.DisplayAlertAsync(
        //    "Prueba",
        //    $"Usuario: {user?.Email}\nFoto: {user?.Photo}",
        //    "Aceptar");

        FlyoutHeaderView.BindingContext = user;
        if (user is not null && !string.IsNullOrWhiteSpace(user.Photo))
        {
            try
            {
                using var httpClient = new HttpClient();

                user.Photo = user.Photo.Replace('\\', '/');

                var bytes = await httpClient.GetByteArrayAsync(user.Photo);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error descargando imagen", ex.Message, "Aceptar");
            }
        }
    }

    private void ClearUserMenu()
    {
        foreach (var item in Items.ToList())
        {
            if (!ReferenceEquals(item, _welcomeShellItem))
            {
                Items.Remove(item);
            }
        }

        FlyoutHeaderView.BindingContext = null;
    }
    private static string GetTitle(string resourceKey, string defaultValue)
    {
        return Literals.ResourceManager.GetString(resourceKey, CultureInfo.CurrentUICulture) ?? defaultValue;
    }
}