using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ValidareEndTimeAttribute : ValidationAttribute
{
    private readonly string _startTimePropertyName;
    private readonly bool _MorningAfternoonPropertyName;

    // MorningAfternoon
    public ValidareEndTimeAttribute(
        string startTimePropertyName,
        bool morningAfternoonPropertyName)
    {
        _startTimePropertyName = startTimePropertyName;
        _MorningAfternoonPropertyName = morningAfternoonPropertyName;
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        // Obtener la propiedad de hora de inicio.
        var horaInicioProp = validationContext.ObjectType.GetProperty(
            _startTimePropertyName
        );

        if (horaInicioProp == null)
        {
            return new ValidationResult(
                $"Propiedad {_startTimePropertyName} no encontrada"
            );
        }

        // Obtener valores.
        var horaInicio = (TimeSpan?)horaInicioProp.GetValue(
            validationContext.ObjectInstance
        );

        var horaFinal = (TimeSpan?)value;

        // Si no hay hora de inicio, no se valida.
        if (!horaInicio.HasValue)
        {
            return ValidationResult.Success;
        }

        if (horaInicio == TimeSpan.Zero && horaFinal == TimeSpan.Zero)
        {
            return ValidationResult.Success;
        }

        // Validar que la hora final tenga valor.
        if (!horaFinal.HasValue)
        {
            return new ValidationResult("La hora final es obligatoria");
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

        // Validar rango de hora final.
        TimeSpan minHora = new TimeSpan(min, 0, 0);
        TimeSpan maxHora = new TimeSpan(max, 0, 0);

        if (horaFinal < minHora || horaFinal >= maxHora)
        {
            return new ValidationResult(errMessage);
        }

        // Validar que la hora final sea posterior a la hora de inicio.
        if (horaFinal <= horaInicio)
        {
            return new ValidationResult(
                "La hora final debe ser posterior a la hora inicial"
            );
        }

        return ValidationResult.Success;
    }
}
