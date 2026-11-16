using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.ConventionValues;

public partial class ConventionValuesFormPage : ContentPage
{
    private readonly ConventionValuesViewModel _viewModel;
    private readonly ConventionValue? _conventionValue;

    public ConventionValuesFormPage(ConventionValuesViewModel viewModel, ConventionValue? conventionValue)
    {
        InitializeComponent();

        _viewModel = viewModel;

        _conventionValue = conventionValue;

        BindingContext = _viewModel;

        LoadForm();
    }
    private void LoadForm()
    {
        PkrConvention.ItemsSource = _viewModel.Conventions;
        PkrStatus.ItemsSource = _viewModel.Statuses;

        if (_conventionValue != null)
        {
            LblTitle.Text = "Editar valor del convenio";

            PkrConvention.SelectedItem = _viewModel.Conventions
                .FirstOrDefault(x => x.Id == _conventionValue.ConventionId);

            PkrStatus.SelectedItem = _viewModel.Statuses
                .FirstOrDefault(x => x.Id == _conventionValue.StatuId);

            TxtPrince.Text = _conventionValue.Prince.ToString(
                System.Globalization.CultureInfo.InvariantCulture);

            DtpDate.Date = _conventionValue.Date;

            PkrStatus.IsEnabled = true;
        }
        else
        {
            LblTitle.Text = "Nuevo valor del convenio";

            var activeStatus = _viewModel.Statuses.FirstOrDefault(
                x => string.Equals(
                    x.Name,
                    "Activo",
                    StringComparison.OrdinalIgnoreCase));

            PkrStatus.SelectedItem = activeStatus;
            PkrStatus.IsEnabled = false;

            DtpDate.Date = DateTime.Today;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        LblError.IsVisible = false;

        if (PkrConvention.SelectedItem is not Convention convention)
        {
            await ShowErrorAsync("Seleccione un convenio.");
            return;
        }

        if (PkrStatus.SelectedItem is not Statu status)
        {
            await ShowErrorAsync("Seleccione un estado.");
            return;
        }

        if (!double.TryParse(
            TxtPrince.Text?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var prince) || prince <= 0)
        {
            await ShowErrorAsync("Ingrese un valor numérico mayor que cero.");
            return;
        }

        if (_conventionValue == null)
        {
            await _viewModel.CreateAsync(
                convention.Id,
                DtpDate.Date ?? DateTime.Today,
                prince);
        }
        else
        {
            await _viewModel.UpdateAsync(
                _conventionValue.Id,
                convention.Id,
                DtpDate.Date ?? DateTime.Today,
                prince,
                status.Id);
        }

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await ShowErrorAsync(_viewModel.ErrorMessage);
            return;
        }

        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        await Navigation.PopAsync();
    }

    private async Task ShowErrorAsync(string message)
    {
        LblError.Text = message;
        LblError.IsVisible = true;
        await DisplayAlertAsync("Convenios", message, "Aceptar");
    }
}