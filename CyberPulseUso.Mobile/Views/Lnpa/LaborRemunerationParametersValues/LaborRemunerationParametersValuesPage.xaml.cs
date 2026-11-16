using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.LaborRemunerationParametersValues;

public partial class LaborRemunerationParametersValuesPage : ContentPage
{
    private readonly LaborRemunerationParametersValuesViewModel _viewModel;

    public LaborRemunerationParametersValuesPage(LaborRemunerationParametersValuesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LaborRemunerationParametersValues.Count == 0)
        {
            await _viewModel.LoadAsync();
            await ShowErrorAsync();
        }
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        await _viewModel.SearchAsync();
        await ShowErrorAsync();
    }

    private async void OnPreviousPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.PreviousPageAsync();
        await ShowErrorAsync();
    }

    private async void OnNextPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.NextPageAsync();
        await ShowErrorAsync();
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(
            new LaborRemunerationParametersValuesFormPage(_viewModel));
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not LaborRemunerationParametersValueDTO value)
            return;

        await Navigation.PushAsync(
            new LaborRemunerationParametersValuesFormPage(
                _viewModel, value.Id));
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.CommandParameter is not LaborRemunerationParametersValueDTO value)
            return;

        bool confirm = await DisplayAlertAsync(
            "Confirmar",
            "¿Está seguro de eliminar este registro?",
            "Sí",
            "No");

        if (!confirm) return;

        await _viewModel.DeleteAsync(value.Id);
        await ShowErrorAsync();
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        var file = await _viewModel.ExportPdfAsync();

        if (file == null || file.Length == 0)
        {
            await ShowErrorAsync();
            return;
        }

        await SaveAndOpenFileAsync(file, "ValoresRemuneraciones.pdf");
    }

    private async void OnExportExcelClicked(object? sender, EventArgs e)
    {
        var file = await _viewModel.ExportExcelAsync();

        if (file == null || file.Length == 0)
        {
            await ShowErrorAsync();
            return;
        }

        await SaveAndOpenFileAsync(file, "ValoresRemuneraciones.xlsx");
    }

    private async Task SaveAndOpenFileAsync(byte[] file, string fileName)
    {
        try
        {
            string filePath = Path.Combine(
                FileSystem.CacheDirectory, fileName);

            await File.WriteAllBytesAsync(filePath, file);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error", ex.Message, "Aceptar");
        }
    }

    private async Task ShowErrorAsync()
    {
        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");

            _viewModel.ErrorMessage = string.Empty;
        }
    }
}