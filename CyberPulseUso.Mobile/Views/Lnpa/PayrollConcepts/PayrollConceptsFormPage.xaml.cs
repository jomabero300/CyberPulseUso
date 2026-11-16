using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.PayrollConcepts;

public partial class PayrollConceptsFormPage : ContentPage
{
    private readonly PayrollConceptsViewModel _viewModel;
    private readonly PayrollConcept? _concept;
    private bool _initialized;

    public PayrollConceptsFormPage(PayrollConceptsViewModel viewModel, PayrollConcept? concept)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _concept = concept;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized)
            return;

        _initialized = true;

        await LoadFormAsync();
    }

    private async Task LoadFormAsync()
    {
        try
        {
            LblError.IsVisible = false;

            if (_viewModel.Statuses.Count == 0)
                await _viewModel.LoadAsync();

            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await ShowErrorAsync();
                return;
            }

            PkrStatus.ItemsSource = _viewModel.Statuses.ToList();

            if (_concept != null)
            {
                LblTitle.Text = "Editar concepto de nómina";
                Title = "Editar concepto";

                TxtName.Text = _concept.Name;

                PkrStatus.SelectedItem =
                    _viewModel.Statuses.FirstOrDefault(
                        x => x.Id == _concept.StatuId);

                PkrStatus.IsEnabled = true;
            }
            else
            {
                LblTitle.Text = "Nuevo concepto de nómina";
                Title = "Nuevo concepto";

                var activeStatus = _viewModel.Statuses.FirstOrDefault(
                    x => string.Equals(
                        x.Name,
                        "Activo",
                        StringComparison.OrdinalIgnoreCase));

                if (activeStatus == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró el estado Activo.");
                }

                PkrStatus.SelectedItem = activeStatus;
                PkrStatus.IsEnabled = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                ex.Message,
                "Aceptar");
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            LblError.IsVisible = false;

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Ingrese el nombre del concepto.",
                    "Aceptar");
                return;
            }

            if (TxtName.Text.Trim().Length > 80)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "El nombre no puede superar los 80 caracteres.",
                    "Aceptar");
                return;
            }

            if (PkrStatus.SelectedItem is not Statu status)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Seleccione un estado.",
                    "Aceptar");
                return;
            }

            if (_concept != null)
            {
                await _viewModel.UpdateAsync(
                    _concept.Id,
                    TxtName.Text,
                    status.Id);
            }
            else
            {
                await _viewModel.CreateAsync(TxtName.Text);
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await ShowErrorAsync();
                return;
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error",
                ex.Message,
                "Aceptar");
        }
    }
    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
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