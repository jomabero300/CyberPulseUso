using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using System.Collections.ObjectModel;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class PayrollSettlementCreatePage : ContentPage
{
    public ObservableCollection<PayrollSettlementItemViewModel> PayrollSettlements { get; }

    private readonly List<PayrollSettlementDTO> _settlements;
    private readonly PayrollSettlementsViewModel _viewModel;

    public double TotalAccruedValue { get; private set; }
    public double TotalDeductedValue { get; private set; }
    public double TotalToPay { get; private set; }
    private readonly ITokenService _tokenService;
    private readonly Dictionary<int, Entry> _amountEntries = new();
    public PayrollSettlementCreatePage(List<PayrollSettlementDTO> payrollSettlements, PayrollSettlementsViewModel viewModel, ITokenService tokenService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _settlements = payrollSettlements;
        _tokenService = tokenService;

        PayrollSettlements = new ObservableCollection<PayrollSettlementItemViewModel>(
            _settlements.Select(x =>
                new PayrollSettlementItemViewModel(x, Recalculate)));

        BindingContext = this;

        Recalculate();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var session = await _tokenService.GetUserSessionAsync();

        if (session is null)
            return;

        DateValueLabel.Text = DateTime.Now.ToString("dd/MM/yyyy");
        WorkerNameValueLabel.Text = session.FullName;
        ChargeNameValueLabel.Text = session.ChargeName;
    }

    private void Recalculate()
    {
        foreach (var item in PayrollSettlements)
        {
            item.AccruedValue = item.Amount * item.UnitValue;
        }

        double totalAccruedValue = PayrollSettlements.Sum(x => x.AccruedValue);

        var withholdingConcept = PayrollSettlements.FirstOrDefault(x => x.PayrollConceptId == 16);
        if (withholdingConcept is not null)
            withholdingConcept.DeductedValue = totalAccruedValue * withholdingConcept.Amount / 100d;

        var healthConcept = PayrollSettlements.FirstOrDefault(x => x.PayrollConceptId == 17);
        if (healthConcept is not null)
            healthConcept.DeductedValue = totalAccruedValue * healthConcept.Amount / 100d;

        var pensionConcept = PayrollSettlements.FirstOrDefault(x => x.PayrollConceptId == 18);
        if (pensionConcept is not null)
            pensionConcept.DeductedValue = totalAccruedValue * pensionConcept.Amount / 100d;

        var currentDailyBaseSalary = PayrollSettlements.FirstOrDefault(x => x.PayrollConceptId == 1);

        var unionDuesDeduction = PayrollSettlements.FirstOrDefault(x => x.PayrollConceptId == 19);
        if (unionDuesDeduction is not null && currentDailyBaseSalary is not null)
        {
            unionDuesDeduction.DeductedValue =
                currentDailyBaseSalary.Amount *
                currentDailyBaseSalary.UnitValue *
                unionDuesDeduction.Amount / 100d;
        }

        var solidarityDiscount = PayrollSettlements.FirstOrDefault(x => x.PayrollConceptId == 20);
        if (solidarityDiscount is not null && currentDailyBaseSalary is not null)
        {
            solidarityDiscount.DeductedValue =
                currentDailyBaseSalary.Amount *
                currentDailyBaseSalary.UnitValue *
                solidarityDiscount.Amount / 100d;
        }

        TotalAccruedValue = PayrollSettlements.Sum(x => x.AccruedValue);
        TotalDeductedValue = PayrollSettlements.Sum(x => x.DeductedValue);
        TotalToPay = TotalAccruedValue - TotalDeductedValue;

        OnPropertyChanged(nameof(TotalAccruedValue));
        OnPropertyChanged(nameof(TotalDeductedValue));
        OnPropertyChanged(nameof(TotalToPay));
    }
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (!await ValidateRequiredDeductions())
                return;

            var response = await _viewModel.CreateFullAsync(_settlements);

            if (response is not null)
            {
                await DisplayAlertAsync(
                    "Liquidación",
                    "La liquidación se creó correctamente.",
                    "Aceptar");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync(
                    "Error",
                    string.IsNullOrWhiteSpace(_viewModel.ErrorMessage)
                        ? "No fue posible crear la liquidación."
                        : _viewModel.ErrorMessage,
                    "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "Aceptar");
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        await SaveAsync();
    }

    private async Task<bool> ValidateRequiredDeductions()
    {
        int[] requiredConcepts = { 16, 17, 18, 19, 20 };

        var invalidDeductions = requiredConcepts
            .Select(id => _settlements.FirstOrDefault(
                x => x.PayrollConceptId == id))
            .Where(x => x is null || x.DeductedValue <= 0)
            .ToList();

        if (invalidDeductions.Count == 0)
            return true;

        var names = invalidDeductions
            .Select(x => x?.Name ?? "Concepto no disponible")
            .ToList();

        await DisplayAlertAsync(
            "Validación de liquidación",
            "Los siguientes conceptos deben tener una deducción superior a cero:\n\n" +
            string.Join("\n", names),
            "Aceptar");

        // Buscar el primer concepto pendiente.
        var firstInvalid = invalidDeductions[0];

        if (firstInvalid is not null &&
            _amountEntries.TryGetValue(
                firstInvalid.PayrollConceptId, out var entry))
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await PayrollScrollView.ScrollToAsync(
                    entry,
                    ScrollToPosition.MakeVisible,
                    true);

                entry.Focus();
            });
        }

        return false;
    }

    private void OnAmountEntryLoaded(object? sender, EventArgs e)
    {
        if (sender is Entry entry &&
            entry.BindingContext is PayrollSettlementItemViewModel item)
        {
            _amountEntries[item.PayrollConceptId] = entry;
        }
    }
}