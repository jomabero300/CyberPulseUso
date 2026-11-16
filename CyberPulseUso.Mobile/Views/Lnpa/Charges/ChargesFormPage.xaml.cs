using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.Charges;

public partial class ChargesFormPage : ContentPage
{
    private readonly ChargesViewModel _viewModel;
    private readonly Charge? _charge;
    private bool _initialized;
    public ChargesFormPage(ChargesViewModel viewModel, Charge? charge)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _charge = charge;
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
            if (_viewModel.Statuses.Count == 0 ||
                _viewModel.Groups.Count == 0)
            {
                await _viewModel.LoadAsync();
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await ShowErrorAsync();
                return;
            }

            PkrStatus.ItemsSource = _viewModel.Statuses.ToList();

            if (_charge != null)
            {
                LblTitle.Text = "Editar cargo";
                Title = "Editar cargo";

                TxtName.Text = _charge.Name;

                var groups = _viewModel.GetGroupsForEdit(
                    _charge.GroupId);

                PkrGroup.ItemsSource = groups;

                PkrGroup.SelectedItem = groups.FirstOrDefault(
                    x => x.Id == _charge.GroupId);

                PkrStatus.SelectedItem =
                    _viewModel.Statuses.FirstOrDefault(
                        x => x.Id == _charge.StatuId);

                PkrGroup.IsEnabled = true;
                PkrStatus.IsEnabled = true;
            }
            else
            {
                LblTitle.Text = "Nuevo cargo";
                Title = "Nuevo cargo";

                PkrGroup.ItemsSource =
                    _viewModel.Groups.ToList();

                PkrGroup.SelectedItem = null;
                PkrGroup.Title = "-- Seleccionar un grupo --";

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
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Ingrese el nombre del cargo.",
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

            if (PkrGroup.SelectedItem is not Group group)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Seleccione un grupo.",
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

            BtnSave.IsEnabled = false;
            LayoutSaving.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            if (_charge != null)
            {
                await _viewModel.UpdateAsync(
                    _charge.Id,
                    TxtName.Text,
                    group.Id,
                    _charge.GroupId,
                    status.Id);
            }
            else
            {
                await _viewModel.CreateAsync(
                    TxtName.Text,
                    group.Id);
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
        finally
        {
            BtnSave.IsEnabled = true;
            LayoutSaving.IsVisible = false;
            LoadingIndicator.IsRunning = false;
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