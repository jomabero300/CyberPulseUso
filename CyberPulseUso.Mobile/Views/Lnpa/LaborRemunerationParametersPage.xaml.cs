using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class LaborRemunerationParametersPage : ContentPage
{
    private readonly LaborRemunerationParametersViewModel _viewModel;
    public LaborRemunerationParametersPage(LaborRemunerationParametersViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            await DisplayAlertAsync("Remuneraciones", _viewModel.ErrorMessage, "Aceptar");
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await _viewModel.SearchAsync();
        await ShowErrorAsync();
    }

    private async void OnNextPageClicked(object sender, EventArgs e)
    {
        await _viewModel.NextPageAsync();
        await ShowErrorAsync();
    }

    private async void OnPreviousPageClicked(object sender, EventArgs e)
    {
        await _viewModel.PreviousPageAsync();
        await ShowErrorAsync();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        string? name = await DisplayPromptAsync(
            "Nueva remuneración",
            "Ingrese la descripción:",
            "Guardar",
            "Cancelar",
            "Descripción",
            170,
            Keyboard.Text);

        if (string.IsNullOrWhiteSpace(name))
            return;

        await _viewModel.CreateAsync(name);
        await ShowErrorAsync();
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton button ||
            button.CommandParameter is not LaborRemunerationParameters parameter)
            return;

        string? name = await DisplayPromptAsync(
            "Editar remuneración",
            "Ingrese la descripción:",
            "Continuar",
            "Cancelar",
            "Descripción",
            170,
            Keyboard.Text,
            parameter.Name ?? string.Empty);

        if (string.IsNullOrWhiteSpace(name))
            return;

        int? statusId = await SelectStatusAsync(parameter.StatuId);

        if (statusId == null)
            return;

        await _viewModel.UpdateAsync(
            parameter.Id,
            name,
            statusId.Value);

        await ShowErrorAsync();
    }

    private async Task<int?> SelectStatusAsync(int selectedStatusId)
    {
        var statuses = _viewModel.Statuses.ToList();

        if (statuses.Count == 0)
        {
            await DisplayAlertAsync("Remuneraciones", "No se encontraron estados disponibles.", "Aceptar");

            return null;
        }

        string[] options = statuses
            .Select(x => x.Name ?? string.Empty)
            .ToArray();

        string? selected = await DisplayActionSheetAsync(
            "Seleccione el estado",
            "Cancelar",
            null,
            options);

        if (string.IsNullOrWhiteSpace(selected) || selected == "Cancelar")
            return null;

        var status = statuses.FirstOrDefault(
            x => string.Equals(
                x.Name,
                selected,
                StringComparison.OrdinalIgnoreCase));

        return status?.Id;
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not ImageButton button ||
            button.CommandParameter is not LaborRemunerationParameters parameter)
            return;

        bool confirm = await DisplayAlertAsync("Eliminar remuneración", $"¿Está seguro de eliminar '{parameter.Name}'?", "Sí", "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(parameter.Id);
        await ShowErrorAsync();
    }

    private async void OnExportPdfClicked(object sender, EventArgs e)
    {
        var file = await _viewModel.ExportPdfAsync();

        if (file == null)
        {
            await ShowErrorAsync();
            return;
        }

        await SaveAndOpenFileAsync(
            file,
            $"Remuneraciones_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
    }

    private async void OnExportExcelClicked(object sender, EventArgs e)
    {
        var file = await _viewModel.ExportExcelAsync();

        if (file == null)
        {
            await ShowErrorAsync();
            return;
        }

        await SaveAndOpenFileAsync(
            file,
            $"Remuneraciones_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    private async Task SaveAndOpenFileAsync(byte[] file, string fileName)
    {
        try
        {
            string filePath = Path.Combine(
                FileSystem.CacheDirectory,
                fileName);

            await File.WriteAllBytesAsync(filePath, file);

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Remuneraciones", $"No fue posible abrir el archivo: {ex.Message}", "Aceptar");
        }
    }

    private async Task ShowErrorAsync()
    {
        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync("Remuneraciones", _viewModel.ErrorMessage, "Aceptar");
        }
    }
}