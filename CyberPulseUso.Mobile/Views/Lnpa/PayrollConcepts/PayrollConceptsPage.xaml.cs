using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.PayrollConcepts;

public partial class PayrollConceptsPage : ContentPage
{
    private readonly PayrollConceptsViewModel _viewModel;
    public PayrollConceptsPage(PayrollConceptsViewModel viewModel)
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

        await Navigation.PushAsync(new PayrollConceptsFormPage(_viewModel,null));
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not PayrollConcept concept)
            return;

        await Navigation.PushAsync(
            new PayrollConceptsFormPage(_viewModel, concept));
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not PayrollConcept concept)
            return;

        var confirm = await DisplayAlertAsync(
            "Eliminar concepto",
            $"¿Desea eliminar el concepto {concept.Name}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(concept.Id);
        await ShowErrorAsync();
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportPdfAsync();

        if (fileBytes == null)
        {
            await ShowErrorAsync();
            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "ConceptosNomina.pdf",
            "application/pdf");
    }

    private async void OnExportExcelClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportExcelAsync();

        if (fileBytes == null)
        {
            await ShowErrorAsync();
            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "ConceptosNomina.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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

    private async Task ShowErrorAsync()
    {
        if (string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            return;

        await DisplayAlertAsync(
            "Error",
            _viewModel.ErrorMessage,
            "Aceptar");

        _viewModel.ErrorMessage = string.Empty;
    }

}