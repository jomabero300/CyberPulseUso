using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.PayrollSettlementLnpa;

public partial class PayrollSettlementForm
{
    private EditContext editContext = null!;

    private StatuDTO selectedStatu = new();
    private List<StatuDTO>? status;
    private bool loading;
    private bool _disable;


    //private const double WithholdingPercentage = 10d;

    private double totalAccruedValue;
    private double totalDeductedValue;
    private double totalToPay => totalAccruedValue - totalDeductedValue;

    [EditorRequired, Parameter] public List<PayrollSettlementDTO> PayrollSettlementDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    public bool FormPostedSuccessfully { get; set; } = false;
    [Parameter] public double WithholdingPercentage { get; set; } 
    [Parameter] public double HealthAndPensionDeductions { get; set; }


    protected override void OnInitialized()
    {
        editContext = new(PayrollSettlementDTO);
    }

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        await LoadPayrollSettlementsAsync();

        loading = false;
    }
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formwasEditad = editContext.IsModified();

        if (!formwasEditad || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
        });

        var confirm = !string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private async Task LoadPayrollSettlementsAsync()
    {
        var responseHttp = await Repository.GetAsync<List<PayrollSettlementDTO>>("/api/PayrollSettlements/Sett");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        PayrollSettlementDTO = responseHttp.Response!;
        CalculateTotals();
    }


    private void AmountChanged(PayrollSettlementDTO item, int value)
    {
        item.Amount = value;

        // Recalcular el valor devengado del registro
        item.AccruedValue = item.Amount * item.UnitValue;

        //// Notificar el cambio al EditContext
        //editContext.NotifyFieldChanged(
        //    new FieldIdentifier(item, nameof(item.Amount)));
        CalculateTotals();
        StateHasChanged();
    }

    private void CalculateTotals()
    {
        if (PayrollSettlementDTO is null)
            return;

        // Total devengado
        totalAccruedValue = PayrollSettlementDTO?.Sum(x => x.AccruedValue) ?? 0d;

        // Buscar el concepto de retención en la fuente
        var withholdingConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 16);

        if (withholdingConcept is not null)
        {
            // Calcular retención en la fuente
            withholdingConcept.DeductedValue =
                (totalAccruedValue * WithholdingPercentage) / 100;
        }

        //aportes a dalud y pension
        var HealthConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 17);
        var PensionConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 18);

        if (HealthConcept is not null)
        {
            // Calcular retención en la fuente
            HealthConcept!.DeductedValue =
                totalAccruedValue * HealthAndPensionDeductions / 100d;
            PensionConcept!.DeductedValue =
                totalAccruedValue * HealthAndPensionDeductions / 100d;
        }

        //DESCUENTO CUOTA SINDICAL 2%
        var UnionDuesDeduction = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 19);
        //SALARIO BÁSICO DIARIO ACTUAL (Dia)
        var CurrentDayliBbaseSalary = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 1);

        UnionDuesDeduction!.DeductedValue = ((CurrentDayliBbaseSalary!.Amount * CurrentDayliBbaseSalary.UnitValue) * 1) / 100;

        //DESCUENTO SOLIDARIDAD 2%
        var SolidarityDiscount = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 20);
        SolidarityDiscount!.DeductedValue = ((CurrentDayliBbaseSalary!.Amount * CurrentDayliBbaseSalary.UnitValue) * 1) / 100;
        // Total deducido
        totalDeductedValue = PayrollSettlementDTO?.Sum(x => x.DeductedValue) ?? 0d;
    }
}