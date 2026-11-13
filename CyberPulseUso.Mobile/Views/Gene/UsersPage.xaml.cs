using CyberPulseUso.Mobile.ViewModels.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CyberPulseUso.Mobile.Views.Gene;

public partial class UsersPage : ContentPage
{
    private readonly UsersViewModel _viewModel;

    public UsersPage(UsersViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadAsync();
    }
    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        await _viewModel.SearchAsync();
    }
    private async void OnPreviousPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.PreviousPageAsync();
    }
    private async void OnNextPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.NextPageAsync();
    }
    private async void OnChangeRoleClicked(object? sender, EventArgs e)
    {
        if (sender is not ImageButton button ||
            button.BindingContext is not UserMobileDTO user)
            return;

        if (_viewModel.IsBusy)
            return;

        var currentRole = user.UserType;

        var availableRoles = Enum.GetValues<UserType>()
            .Where(role => role != currentRole)
            .Select(role =>
            {
                var displayName = typeof(UserType)
                    .GetField(role.ToString())?
                    .GetCustomAttribute<DisplayAttribute>()?
                    .GetName() ?? role.ToString();

                return new
                {
                    Value = role,
                    Name = displayName
                };
            })
            .ToList();

        if (availableRoles.Count == 0)
            return;

        var selectedRole = await DisplayActionSheetAsync(
            $"Cambiar rol de {user.FirstName} {user.LastName}",
            "Cancelar",
            null,
            availableRoles.Select(x => x.Name).ToArray());

        if (string.IsNullOrWhiteSpace(selectedRole) ||
            selectedRole == "Cancelar")
            return;

        var selected = availableRoles
            .FirstOrDefault(x => x.Name == selectedRole);

        if (selected == null)
            return;

        bool confirm = await DisplayAlertAsync(
            "Confirmar cambio",
            $"¿Deseas cambiar el rol de {user.FirstName} {user.LastName}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.UpdateUserTypeAsync(user.Id, selected.Value);
    }
    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportPdfAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "Usuarios.pdf",
            "application/pdf");
    }
    private async void OnExportExcelClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportExcelAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "Usuarios.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
    private async Task SaveAndOpenFileAsync(byte[] fileBytes,string fileName,string contentType)
    {
        try
        {
            var filePath = Path.Combine(
                FileSystem.CacheDirectory,
                fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath, contentType)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Archivo",$"No fue posible abrir el archivo: {ex.Message}","Aceptar");
        }
    }
}