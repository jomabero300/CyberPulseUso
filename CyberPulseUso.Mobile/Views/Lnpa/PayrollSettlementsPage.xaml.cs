using CyberPulseUso.Mobile.Models;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class PayrollSettlementsPage : ContentPage
{
    private readonly PayrollSettlementsViewModel _viewModel;
    private readonly ITokenService _tokenService;
    private UserSession? _userSession;


    public PayrollSettlementsPage(PayrollSettlementsViewModel viewModel, ITokenService tokenService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _tokenService = tokenService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _userSession = await _tokenService.GetUserSessionAsync();

        _viewModel.IsAdministrator =
            _userSession?.UserType == "Admi" ||
            _userSession?.UserType == "SAdm";

        _viewModel.IsWorker = _userSession?.UserType == "work";

        await _viewModel.LoadAsync();

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        await _viewModel.SearchAsync();
    }

    private async void OnPreviousPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.PreviousPageAsync();
    }

    private async void OnNextPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.NextPageAsync();
    }

    private async void OnPdfClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not PayrollSettlement settlement)
            return;

        if (string.IsNullOrWhiteSpace(settlement.UserId))
        {
            await DisplayAlertAsync(
                "PDF",
                "No se encontró el identificador del trabajador.",
                "Aceptar");
            return;
        }

        var fileBytes = await _viewModel.GeneratePdfAsync(
            settlement.Date,
            settlement.ChargeId,
            settlement.UserId);

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");
            }

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            $"Liquidacion_{settlement.Date:yyyyMMdd_HHmm}.pdf",
            "application/pdf");
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

    private void OnTotalToPayBindingContextChanged(object? sender, EventArgs e)
    {
        if (sender is Label label &&
            label.BindingContext is PayrollSettlement settlement)
        {
            label.Text = (settlement.AccruedValue - settlement.DeductedValue)
                .ToString("N2");
        }
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not PayrollSettlement settlement)
            return;

        try
        {
            _viewModel.IsBusy = true;

            var payrollSettlements = await _viewModel.GetForEditAsync(
                settlement.Date,
                settlement.ChargeId,
                settlement.UserId!);

            if (payrollSettlements == null || payrollSettlements.Count == 0)
            {
                await DisplayAlertAsync(
                    "Editar liquidación",
                    !string.IsNullOrWhiteSpace(_viewModel.ErrorMessage)
                        ? _viewModel.ErrorMessage
                        : "No se encontraron conceptos para esta liquidación.",
                    "Aceptar");

                return;
            }

            var editPage = new PayrollSettlementEditPage(
                settlement,
                payrollSettlements,
                _viewModel);

            await Navigation.PushAsync(editPage);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "Aceptar");
        }
        finally
        {
            _viewModel.IsBusy = false;
        }
    }
    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        try
        {
            _viewModel.IsBusy = true;

            var settlements = await _viewModel.GetForCreateAsync();

            if (settlements == null || settlements.Count == 0)
            {
                await DisplayAlertAsync(
                    "Crear liquidación",
                    !string.IsNullOrWhiteSpace(_viewModel.ErrorMessage)
                        ? _viewModel.ErrorMessage
                        : "No se encontraron conceptos para crear la liquidación.",
                    "Aceptar");

                return;
            }

            var createPage = new PayrollSettlementCreatePage(
                settlements,
                _viewModel,
                _tokenService);

            await Navigation.PushAsync(createPage);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "Aceptar");
        }
        finally
        {
            _viewModel.IsBusy = false;
        }
    }
}