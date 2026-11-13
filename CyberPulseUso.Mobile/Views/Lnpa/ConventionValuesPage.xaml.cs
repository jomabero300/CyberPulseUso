
using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class ConventionValuesPage : ContentPage
{
    private readonly ConventionValuesViewModel _viewModel;

    public ConventionValuesPage(ConventionValuesViewModel viewModel)
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

        if (_viewModel.Conventions.Count == 0)
        {
            await DisplayAlertAsync(
                "Convenios",
                "No hay convenios disponibles.",
                "Aceptar");
            return;
        }

        var convention = await SelectConventionAsync();

        if (convention == null)
            return;

        var date = await SelectDateAsync(DateTime.Today);

        if (date == null)
            return;

        var prince = await GetPrinceAsync();

        if (prince == null)
            return;

        await _viewModel.CreateAsync(
            convention.Id,
            date.Value,
            prince.Value);

        await ShowErrorAsync();
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not ConventionValue value)
            return;

        var convention = await SelectConventionAsync(value.ConventionId);

        if (convention == null)
            return;

        var date = await SelectDateAsync(value.Date);

        if (date == null)
            return;

        var prince = await GetPrinceAsync(value.Prince);

        if (prince == null)
            return;

        if (_viewModel.Statuses.Count == 0)
        {
            await DisplayAlertAsync(
                "Estados",
                "No hay estados disponibles.",
                "Aceptar");
            return;
        }

        var statusOptions = _viewModel.Statuses
            .Select(x => x.Name ?? string.Empty)
            .ToArray();

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
            value.Id,
            convention.Id,
            date.Value,
            prince.Value,
            status.Id);

        await ShowErrorAsync();
    }

    private async Task<Convention?> SelectConventionAsync(
        int? selectedId = null)
    {
        if (_viewModel.Conventions.Count == 0)
            return null;

        var options = _viewModel.Conventions
            .Select(x => x.Name ?? string.Empty)
            .ToArray();

        var selected = await DisplayActionSheetAsync(
            "Seleccione el convenio",
            "Cancelar",
            null,
            options);

        if (string.IsNullOrWhiteSpace(selected) ||
            selected == "Cancelar")
            return null;

        return _viewModel.Conventions.FirstOrDefault(
            x => x.Name == selected);
    }

    private async Task<DateTime?> SelectDateAsync(DateTime initialDate)
    {
        var result = await DisplayPromptAsync(
            "Fecha del valor",
            "Ingrese la fecha (dd/MM/yyyy):",
            "Aceptar",
            "Cancelar",
            initialValue: initialDate.ToString("dd/MM/yyyy"),
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(result))
            return null;

        if (DateTime.TryParseExact(
            result,
            "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out var date))
        {
            return date;
        }

        await DisplayAlertAsync(
            "Fecha inválida",
            "Ingrese una fecha válida con formato dd/MM/yyyy.",
            "Aceptar");

        return null;
    }

    private async Task<double?> GetPrinceAsync(double? initialValue = null)
    {
        var result = await DisplayPromptAsync(
            "Precio",
            "Ingrese el precio:",
            "Aceptar",
            "Cancelar",
            initialValue: initialValue?.ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(result))
            return null;

        if (double.TryParse(
            result.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var prince) && prince > 0)
        {
            return prince;
        }

        await DisplayAlertAsync(
            "Precio inválido",
            "Ingrese un precio numérico mayor que cero.",
            "Aceptar");

        return null;
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not ConventionValue value)
            return;

        var confirm = await DisplayAlertAsync(
            "Eliminar valor",
            $"¿Desea eliminar el valor del convenio {value.Convention?.Name}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(value.Id);
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
            "ValoresConvenios.pdf",
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
            "ValoresConvenios.xlsx",
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
        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync("Error",_viewModel.ErrorMessage,"Aceptar");
        }
    }
}