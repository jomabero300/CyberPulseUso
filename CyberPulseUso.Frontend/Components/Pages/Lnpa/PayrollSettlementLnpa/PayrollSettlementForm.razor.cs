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
    //[Parameter] public double WithholdingPercentage { get; set; } 
    //[Parameter] public double HealthAndPensionDeductions { get; set; }

    protected override void OnInitialized()
    {
        editContext = new(PayrollSettlementDTO);
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

        //otros pagos
        var OtherPaymentsConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId==14);

        if(OtherPaymentsConcept is not null)
        {
            OtherPaymentsConcept.AccruedValue = OtherPaymentsConcept.Amount;
        }
        //otros descuentos
        var OtherDeductionsConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 15);

        if (OtherDeductionsConcept is not null)
        {
            OtherDeductionsConcept.DeductedValue = OtherDeductionsConcept.Amount;
        }

        // Buscar el concepto de retención en la fuente
        var withholdingConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 16);

        if (withholdingConcept is not null)
        {
            // Calcular retención en la fuente
            withholdingConcept.DeductedValue = (totalAccruedValue * withholdingConcept.Amount) / 100;
        }

        //aportes a dalud y pension
        var HealthConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 17);

        if (HealthConcept is not null)
        {
            //Salu y pension
            HealthConcept!.DeductedValue = totalAccruedValue * HealthConcept.Amount / 100d;
        }

        var PensionConcept = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 18);

        if (PensionConcept is not null)
        {
            //Salu y pension
            PensionConcept!.DeductedValue = totalAccruedValue * PensionConcept.Amount / 100d;
        }



        //DESCUENTO CUOTA SINDICAL 2%
        var UnionDuesDeduction = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 19);
        //SALARIO BÁSICO DIARIO ACTUAL (Dia)
        var CurrentDayliBbaseSalary = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 1);

        UnionDuesDeduction!.DeductedValue = ((CurrentDayliBbaseSalary!.Amount * CurrentDayliBbaseSalary.UnitValue) * UnionDuesDeduction.Amount) / 100;

        //DESCUENTO SOLIDARIDAD 2%
        var SolidarityDiscount = PayrollSettlementDTO!.FirstOrDefault(x => x.PayrollConceptId == 20);
        SolidarityDiscount!.DeductedValue = ((CurrentDayliBbaseSalary!.Amount * CurrentDayliBbaseSalary.UnitValue) * SolidarityDiscount.Amount) / 100;
        // Total deducido
        totalDeductedValue = PayrollSettlementDTO?.Sum(x => x.DeductedValue) ?? 0d;
    }
}