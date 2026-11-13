using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class ConventionsPage : ContentPage
{
    private readonly ConventionsViewModel _viewModel;

    public ConventionsPage(ConventionsViewModel viewModel)
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

        var name = await DisplayPromptAsync(
            "Nuevo convenio",
            "Nombre del convenio:",
            "Continuar",
            "Cancelar",
            maxLength: 80);

        if (string.IsNullOrWhiteSpace(name))
            return;

        await _viewModel.CreateAsync(name);

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Convention convention)
            return;

        var name = await DisplayPromptAsync(
            "Editar convenio",
            "Nombre del convenio:",
            "Continuar",
            "Cancelar",
            initialValue: convention.Name,
            maxLength: 80);

        if (string.IsNullOrWhiteSpace(name))
            return;

        var statusOptions = _viewModel.Statuses
            .Select(x => x.Name)
            .ToArray();

        if (statusOptions.Length == 0)
        {
            await DisplayAlertAsync(
                "Estados",
                "No hay estados disponibles.",
                "Aceptar");
            return;
        }

        var selectedStatus = await DisplayActionSheetAsync(
            "Seleccione el estado",
            "Cancelar",
            null,
            statusOptions);

        if (string.IsNullOrWhiteSpace(selectedStatus) ||
            selectedStatus == "Cancelar")
            return;

        var status = _viewModel.Statuses.FirstOrDefault(
            x => x.Name == selectedStatus);

        if (status == null)
            return;

        await _viewModel.UpdateAsync(
            convention.Id,
            name,
            status.Id);

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Convention convention)
            return;

        var confirm = await DisplayAlertAsync(
            "Eliminar convenio",
            $"¿Desea eliminar el convenio {convention.Name}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(convention.Id);

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
            "Convenios.pdf",
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
            "Convenios.xlsx",
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