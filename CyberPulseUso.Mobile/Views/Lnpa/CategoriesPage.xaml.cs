using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;

    public CategoriesPage(CategoriesViewModel viewModel)
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

    private async void OnNextPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.NextPageAsync();
    }

    private async void OnPreviousPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.PreviousPageAsync();
    }

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        var name = await DisplayPromptAsync(
            "Nueva categoría",
            "Nombre de la categoría:",
            "Guardar",
            "Cancelar",
            maxLength: 80);

        if (string.IsNullOrWhiteSpace(name))
            return;

        await _viewModel.CreateAsync(name);
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Category category)
            return;

        var name = await DisplayPromptAsync(
            "Editar categoría",
            "Nombre de la categoría:",
            "Guardar",
            "Cancelar",
            initialValue: category.Name,
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
            category.Id,
            name,
            status.Id);
    }
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Category category)
            return;

        var confirm = await DisplayAlertAsync(
            "Eliminar categoría",
            $"¿Desea eliminar la categoría {category.Name}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(category.Id);
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportPdfAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync("Error", _viewModel.ErrorMessage, "Aceptar");
            }

            return;
        }

        await SaveAndOpenFileAsync(fileBytes,"Categorias.pdf","application/pdf");
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

        await SaveAndOpenFileAsync(fileBytes, "Categorias.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }


    private async Task SaveAndOpenFileAsync(byte[] fileBytes, string fileName, string contentType)
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