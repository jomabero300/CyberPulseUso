using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.LaborRemunerationParameter;

public partial class LaborRemunerationParametersFormPage : ContentPage
{
    private readonly LaborRemunerationParametersViewModel _viewModel;
    private readonly LaborRemunerationParameters? _parameter;

    public LaborRemunerationParametersFormPage(LaborRemunerationParametersViewModel viewModel, LaborRemunerationParameters? parameter)
    {
        InitializeComponent();

        _viewModel = viewModel;

        _parameter = parameter;

        PkrStatus.ItemsSource = _viewModel.Statuses;

        if (_parameter == null)
        {
            LblTitle.Text = "Crear remuneración";

            var activeStatus = _viewModel.Statuses
                .FirstOrDefault(s => s.Name == "Activo");

            PkrStatus.SelectedItem = activeStatus;
            PkrStatus.IsEnabled = false;
        }
        else
        {
            LblTitle.Text = "Editar remuneración";

            TxtName.Text = _parameter.Name;

            PkrStatus.SelectedItem = _viewModel.Statuses
                .FirstOrDefault(s => s.Id == _parameter.StatuId);

            PkrStatus.IsEnabled = true;
        }
    }
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        LblError.IsVisible = false;

        var name = TxtName.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("Ingrese la descripción.");
            return;
        }

        if (PkrStatus.SelectedItem is not Statu status)
        {
            ShowError("Seleccione un estado.");
            return;
        }

        if (_parameter == null)
        {
            await _viewModel.CreateAsync(name);
        }
        else
        {
            await _viewModel.UpdateAsync(
                _parameter.Id,
                name,
                status.Id);
        }

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            ShowError(_viewModel.ErrorMessage);
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

    private void ShowError(string message)
    {
        LblError.Text = message;
        LblError.IsVisible = true;
    }
}