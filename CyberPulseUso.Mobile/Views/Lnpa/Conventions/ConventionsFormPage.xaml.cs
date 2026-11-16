using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.Conventions;

public partial class ConventionsFormPage : ContentPage
{
    private readonly ConventionsViewModel _viewModel;
    private readonly Convention? _convention;
    private bool _initialized;
    public ConventionsFormPage(ConventionsViewModel viewModel, Convention? convention)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _convention = convention;
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

            if (_convention != null)
            {
                LblTitle.Text = "Editar convenio";
                Title = "Editar convenio";

                TxtName.Text = _convention.Name;

                PkrStatus.SelectedItem =
                    _viewModel.Statuses.FirstOrDefault(
                        x => x.Id == _convention.StatuId);

                PkrStatus.IsEnabled = true;
            }
            else
            {
                LblTitle.Text = "Nuevo convenio";
                Title = "Nuevo convenio";

                var activeStatus = _viewModel.Statuses.FirstOrDefault(
                    x => string.Equals(
                        x.Name,
                        "Activo",
                        StringComparison.OrdinalIgnoreCase));

                if (activeStatus == null)
                    throw new InvalidOperationException(
                        "No se encontró el estado Activo.");

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
        if (_viewModel.IsBusy)
            return;

        try
        {
            LblError.IsVisible = false;

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Ingrese el nombre del convenio.",
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

            if (_convention != null)
            {
                await _viewModel.UpdateAsync(
                    _convention.Id,
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
        if (_viewModel.IsBusy)
            return;

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