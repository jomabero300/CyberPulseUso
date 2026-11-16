using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using System.Globalization;

namespace CyberPulseUso.Mobile.Views.Lnpa.LaborRemunerationParametersValues;

public partial class LaborRemunerationParametersValuesFormPage : ContentPage
{
    private readonly LaborRemunerationParametersValuesViewModel _viewModel;
    private readonly int? _id;
    private bool _initialized;


    public LaborRemunerationParametersValuesFormPage(LaborRemunerationParametersValuesViewModel viewModel, int? id = null)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _id = id;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized) return;

        _initialized = true;

        await LoadFormAsync();
    }

    private async Task LoadFormAsync()
    {
        try
        {
            LblError.IsVisible = false;

            if (_viewModel.Statuses.Count == 0 ||
                _viewModel.Groups.Count == 0 ||
                _viewModel.Parameters.Count == 0)
            {
                await _viewModel.LoadAsync();
            }

            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await ShowErrorAsync();
                return;
            }

            PkrStatus.ItemsSource = _viewModel.Statuses.ToList();

            if (_id.HasValue)
            {
                LblTitle.Text = "Editar valor de remuneración";
                Title = "Editar valor";

                await LoadRecordAsync(_id.Value);
            }
            else
            {
                LblTitle.Text = "Nuevo valor de remuneración";
                Title = "Nuevo valor";

                PkrParameter.ItemsSource = _viewModel.Parameters.ToList();
                PkrGroup.ItemsSource = _viewModel.Groups.ToList();

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

                DtpDate.Date = DateTime.Today;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Error", ex.Message, "Aceptar");
        }
    }

    private async Task LoadRecordAsync(int id)
    {
        var value = await _viewModel.GetByIdAsync(id);

        if (value == null)
        {
            await ShowErrorAsync();

            if (string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync(
                    "Error",
                    "No se encontró el registro solicitado.",
                    "Aceptar");
            }

            return;
        }

        PkrParameter.ItemsSource =
            _viewModel.GetParametersForEdit(value.LRPId);

        PkrGroup.ItemsSource =
            _viewModel.GetGroupsForEdit(value.GroupId);

        PkrParameter.SelectedItem =
            ((List<LaborRemunerationParametersDTO>)PkrParameter.ItemsSource)
            .FirstOrDefault(x => x.Id == value.LRPId);

        PkrGroup.SelectedItem =
            ((List<GroupDTO>)PkrGroup.ItemsSource)
            .FirstOrDefault(x => x.Id == value.GroupId);

        PkrStatus.SelectedItem =
            _viewModel.Statuses.FirstOrDefault(
                x => x.Id == value.StatuId);

        PkrStatus.IsEnabled = true;

        DtpDate.Date = value.Date;
        
        TxtValue.Text = value.Value.ToString(CultureInfo.CurrentCulture);
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            LblError.IsVisible = false;

            if (PkrParameter.SelectedItem is not LaborRemunerationParametersDTO parameter)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Seleccione un parámetro de remuneración.",
                    "Aceptar");
                return;
            }

            if (PkrGroup.SelectedItem is not GroupDTO group)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Seleccione un grupo.",
                    "Aceptar");
                return;
            }

            if (!double.TryParse(
                TxtValue.Text,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture,
                out double numericValue))
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Ingrese un valor numérico válido.",
                    "Aceptar");
                return;
            }

            var status = PkrStatus.SelectedItem as StatuDTO;

            if (status == null)
            {
                await DisplayAlertAsync(
                    "Validación",
                    "Seleccione un estado.",
                    "Aceptar");
                return;
            }

            var value = new LaborRemunerationParametersValueDTO
            {
                Id = _id ?? 0,
                LRPId = parameter.Id,
                LaborRemunerationParameters = parameter,
                GroupId = group.Id,
                Group = group,
                Date = DtpDate.Date ?? DateTime.Today,
                Value = numericValue,
                StatuId = status.Id
            };

            if (_id.HasValue)
            {
                await _viewModel.UpdateAsync(value);
            }
            else
            {
                await _viewModel.CreateAsync(value);
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
            await DisplayAlertAsync("Error", ex.Message, "Aceptar");
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async Task ShowErrorAsync()
    {
        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync("Error", _viewModel.ErrorMessage, "Aceptar");

            _viewModel.ErrorMessage = string.Empty;
        }
    }
}