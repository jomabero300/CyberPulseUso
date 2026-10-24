using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidarRangoHoraAttribute : ValidationAttribute
{
    private readonly bool _MorningAfternoonPropertyName;

    public ValidarRangoHoraAttribute(bool morningAfternoonPropertyName)
    {
        _MorningAfternoonPropertyName = morningAfternoonPropertyName;
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        var hora = (TimeSpan?)value;

        // Si no hay valor, no se valida.
        if (!hora.HasValue)
        {
            return ValidationResult.Success;
        }

        if (hora == TimeSpan.Zero)
        {
            return ValidationResult.Success;
        }

        int min = 12;
        int max = 24;

        string errMessage = "La hora debe estar entre 12:00 y 24:00";

        if (_MorningAfternoonPropertyName)
        {
            min = 0;
            max = 12;
            errMessage = "La hora debe estar entre 00:00 y 12:00";
        }

        // Validar rango de hora.
        TimeSpan minHora = new TimeSpan(min, 0, 0);
        TimeSpan maxHora = new TimeSpan(max, 0, 0);

        if (hora < minHora || hora >= maxHora)
        {
            return new ValidationResult(errMessage);
        }

        return ValidationResult.Success;
    }
}
