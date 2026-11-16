using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.Views.Lnpa.Categories;

public partial class CategoryFormPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;
    private readonly Category? _category;
    private bool _initialized;

    public CategoryFormPage(CategoriesViewModel viewModel, Category? category)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _category = category;
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

            if (_category != null)
            {
                LblTitle.Text = "Editar categoría";
                Title = "Editar categoría";

                TxtName.Text = _category.Name;

                PkrStatus.SelectedItem =
                    _viewModel.Statuses.FirstOrDefault(
                        x => x.Id == _category.StatuId);

                PkrStatus.IsEnabled = true;
            }
            else
            {
                LblTitle.Text = "Nueva categoría";
                Title = "Nueva categoría";

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
                    "Ingrese el nombre de la categoría.",
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

            BtnSave.IsEnabled = false;
            LayoutSaving.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            if (_category != null)
            {
                await _viewModel.UpdateAsync(
                    _category.Id,
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