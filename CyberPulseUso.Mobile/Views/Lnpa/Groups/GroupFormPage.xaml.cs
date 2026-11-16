using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.Groups;

public partial class GroupFormPage : ContentPage
{
    private readonly GroupsViewModel _viewModel;
    private readonly Group? _group;
    private bool _initialized;
    public GroupFormPage(GroupsViewModel viewModel, Group? group)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _group = group;
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

            if (_viewModel.Statuses.Count == 0 ||
                _viewModel.Categories.Count == 0)
            {
                await _viewModel.LoadAsync();
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await ShowErrorAsync();
                return;
            }

            PkrStatus.ItemsSource = _viewModel.Statuses.ToList();

            if (_group != null)
            {
                LblTitle.Text = "Editar grupo";
                Title = "Editar grupo";

                TxtName.Text = _group.Name;

                var categories = _viewModel.GetCategoriesForEdit(
                    _group.CategoryId);

                PkrCategory.ItemsSource = categories;

                PkrCategory.SelectedItem =
                    categories.FirstOrDefault(
                        x => x.Id == _group.CategoryId);

                PkrStatus.SelectedItem =
                    _viewModel.Statuses.FirstOrDefault(
                        x => x.Id == _group.StatuId);

                PkrCategory.IsEnabled = true;
                PkrStatus.IsEnabled = true;
            }
            else
            {
                LblTitle.Text = "Nuevo grupo";
                Title = "Nuevo grupo";

                PkrCategory.ItemsSource =
                    _viewModel.Categories.ToList();

                PkrCategory.SelectedItem = null;
                PkrCategory.Title = "-- Seleccionar Categoria --";

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
        if (_viewModel.IsBusy)
            return;

        try
        {
            LblError.IsVisible = false;

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Ingrese el nombre del grupo.",
                    "Aceptar");
                return;
            }

            if (TxtName.Text.Trim().Length > 100)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "El nombre no puede superar los 100 caracteres.",
                    "Aceptar");
                return;
            }

            if (PkrCategory.SelectedItem is not Category category)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Seleccione una categoría.",
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

            if (_group != null)
            {
                await _viewModel.UpdateAsync(
                    _group.Id,
                    TxtName.Text,
                    category.Id,
                    _group.CategoryId,
                    status.Id);
            }
            else
            {
                await _viewModel.CreateAsync(
                    TxtName.Text,
                    category.Id);
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