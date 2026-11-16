using CyberPulseUso.Mobile.ViewModels.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class LaborRemunerationParametersOtherValuesPage : ContentPage
{
    private readonly LaborRemunerationParametersOtherValuesViewModel _viewModel;
    public LaborRemunerationParametersOtherValuesPage(LaborRemunerationParametersOtherValuesViewModel viewModel)
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

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        var confirm = await DisplayAlertAsync(
            "Guardar valores",
            "¿Desea guardar los valores de los parámetros de remuneración?",
            "Sí",
            "No");

        if (!confirm)
            return;

        var success = await _viewModel.SaveAsync();

        if (success)
        {
            await DisplayAlertAsync(
                "Guardado",
                "Los valores se guardaron correctamente.",
                "Aceptar");
        }
        else if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }
}