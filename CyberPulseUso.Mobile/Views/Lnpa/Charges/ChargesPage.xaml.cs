using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.Charges;

public partial class ChargesPage : ContentPage
{
    private readonly ChargesViewModel _viewModel;
    public ChargesPage(ChargesViewModel viewModel)
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
        => await _viewModel.SearchAsync();

    private async void OnNextPageClicked(object? sender, EventArgs e)
        => await _viewModel.NextPageAsync();

    private async void OnPreviousPageClicked(object? sender, EventArgs e)
        => await _viewModel.PreviousPageAsync();

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        await Navigation.PushAsync(
            new ChargesFormPage(_viewModel, null));
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Charge charge)
            return;

        await Navigation.PushAsync(new ChargesFormPage(_viewModel, charge));
    }
    private async Task<Group?> SelectGroupAsync()
        => await SelectGroupAsync(_viewModel.Groups.ToList());

    private async Task<Group?> SelectGroupAsync(List<Group> groups)
    {
        if (groups.Count == 0)
        {
            await DisplayAlertAsync(
                "Grupos",
                "No hay grupos disponibles.",
                "Aceptar");
            return null;
        }

        var groupOptions = groups
            .Select(x => x.Name ?? string.Empty)
            .ToArray();

        var selectedGroup = await DisplayActionSheetAsync(
            "Seleccione el grupo",
            "Cancelar",
            null,
            groupOptions);

        if (string.IsNullOrWhiteSpace(selectedGroup) ||
            selectedGroup == "Cancelar")
            return null;

        return groups.FirstOrDefault(
            x => x.Name == selectedGroup);
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Charge charge)
            return;

        var confirm = await DisplayAlertAsync(
            "Eliminar cargo",
            $"¿Desea eliminar el cargo {charge.Name}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(charge.Id);

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportPdfAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");
            }

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "Cargos.pdf",
            "application/pdf");
    }

    private async void OnExportExcelClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportExcelAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");
            }

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "Cargos.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    private async Task SaveAndOpenFileAsync(
        byte[] fileBytes,
        string fileName,
        string contentType)
    {
        try
        {
            var filePath = Path.Combine(
                FileSystem.CacheDirectory,
                fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath, contentType)
                });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Archivo",
                $"No fue posible abrir el archivo: {ex.Message}",
                "Aceptar");
        }
    }
}