using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class PayrollSettlementEditPage : ContentPage
{
    public ObservableCollection<PayrollSettlementItemViewModel> PayrollSettlements { get; }
    private readonly List<PayrollSettlementDTO> _settlements;
    public double TotalAccruedValue { get; private set; }
    public double TotalDeductedValue { get; private set; }
    public double TotalToPay { get; private set; }
    private readonly PayrollSettlementsViewModel _viewModel;
    private readonly Dictionary<int, Entry> _amountEntries = new();
    public PayrollSettlementEditPage(PayrollSettlement settlement, List<PayrollSettlementDTO> payrollSettlements, PayrollSettlementsViewModel viewModel)
    {
        InitializeComponent();

        _settlements = payrollSettlements;

        // Convertimos los DTO en elementos que notifican cambios a la interfaz.
        PayrollSettlements = new ObservableCollection<PayrollSettlementItemViewModel>(
            payrollSettlements.Select(x =>
                new PayrollSettlementItemViewModel(x, Recalculate)));

        BindingContext = this;

        DateValueLabel.Text = settlement.Date.ToString("dd/MM/yyyy");
        WorkerNameValueLabel.Text = settlement.User?.FullName ?? "No disponible";
        ChargeNameValueLabel.Text = settlement.Charge?.Name ?? "No disponible";

        Recalculate();
        _viewModel = viewModel;
    }

    private void Recalculate()
    {
        // 1. Calcular el devengado de cada concepto.
        foreach (var item in PayrollSettlements)
        {
            item.AccruedValue = item.Amount * item.UnitValue;
        }

        // 2. Obtener el total devengado.
        double totalAccruedValue = PayrollSettlements.Sum(x => x.AccruedValue);

        // 3. Retención: concepto 16.
        var withholdingConcept = PayrollSettlements
            .FirstOrDefault(x => x.PayrollConceptId == 16);

        if (withholdingConcept is not null)
        {
            withholdingConcept.DeductedValue =
                totalAccruedValue * withholdingConcept.Amount / 100d;
        }

        // 4. Salud: concepto 17.
        var healthConcept = PayrollSettlements
            .FirstOrDefault(x => x.PayrollConceptId == 17);

        if (healthConcept is not null)
        {
            healthConcept.DeductedValue =
                totalAccruedValue * healthConcept.Amount / 100d;
        }

        // 5. Pensión: concepto 18.
        var pensionConcept = PayrollSettlements
            .FirstOrDefault(x => x.PayrollConceptId == 18);

        if (pensionConcept is not null)
        {
            pensionConcept.DeductedValue =
                totalAccruedValue * pensionConcept.Amount / 100d;
        }

        // 6. Salario básico diario: concepto 1.
        var currentDailyBaseSalary = PayrollSettlements
            .FirstOrDefault(x => x.PayrollConceptId == 1);

        // 7. Descuento sindical: concepto 19.
        var unionDuesDeduction = PayrollSettlements
            .FirstOrDefault(x => x.PayrollConceptId == 19);

        if (unionDuesDeduction is not null &&
            currentDailyBaseSalary is not null)
        {
            unionDuesDeduction.DeductedValue =
                (currentDailyBaseSalary.Amount *
                 currentDailyBaseSalary.UnitValue *
                 unionDuesDeduction.Amount) / 100d;
        }

        // 8. Descuento de solidaridad: concepto 20.
        var solidarityDiscount = PayrollSettlements
            .FirstOrDefault(x => x.PayrollConceptId == 20);

        if (solidarityDiscount is not null &&
            currentDailyBaseSalary is not null)
        {
            solidarityDiscount.DeductedValue =
                (currentDailyBaseSalary.Amount *
                 currentDailyBaseSalary.UnitValue *
                 solidarityDiscount.Amount) / 100d;
        }

        // Totales generales.
        TotalAccruedValue = PayrollSettlements.Sum(x => x.AccruedValue);

        TotalDeductedValue = PayrollSettlements.Sum(x => x.DeductedValue);

        TotalToPay = TotalAccruedValue - TotalDeductedValue;

        // Notificar actualización de la interfaz.
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

            var response = await _viewModel.UpdateFullAsync(_settlements);

            if (response is not null)
            {
                await DisplayAlertAsync(
                    "Liquidación",
                    "La liquidación se guardó correctamente.",
                    "Aceptar");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlertAsync(
                    "Error",
                    "El servidor no devolvió los datos de la liquidación.",
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

public class PayrollSettlementItemViewModel : INotifyPropertyChanged
{
    private readonly PayrollSettlementDTO _settlement;
    private readonly Action _onAmountChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    public PayrollSettlementItemViewModel(
        PayrollSettlementDTO settlement,
        Action onAmountChanged)
    {
        _settlement = settlement;
        _onAmountChanged = onAmountChanged;
    }

    public int PayrollConceptId => _settlement.PayrollConceptId;

    public string? Name => _settlement.Name;

    public int Amount
    {
        get => _settlement.Amount;
        set
        {
            if (_settlement.Amount == value)
                return;

            _settlement.Amount = value;

            OnPropertyChanged();
            _onAmountChanged();
        }
    }

    public double UnitValue => _settlement.UnitValue;

    public double AccruedValue
    {
        get => _settlement.AccruedValue;
        set
        {
            if (_settlement.AccruedValue == value)
                return;

            _settlement.AccruedValue = value;
            OnPropertyChanged();
        }
    }

    public double DeductedValue
    {
        get => _settlement.DeductedValue;
        set
        {
            if (_settlement.DeductedValue == value)
                return;

            _settlement.DeductedValue = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}