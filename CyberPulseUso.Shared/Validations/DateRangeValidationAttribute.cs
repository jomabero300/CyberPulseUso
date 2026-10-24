using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DateRangeValidationAttribute : ValidationAttribute
{
    private readonly string _startDatePropertyName;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="startDatePropertyName">
    /// El nombre de la propiedad que contiene la fecha de inicio.
    /// </param>
    public DateRangeValidationAttribute(string startDatePropertyName)
    {
        _startDatePropertyName = startDatePropertyName;
    }

    public override string FormatErrorMessage(string name)
    {
        return base.FormatErrorMessage(name);
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        DateTime? endDate = value as DateTime?;

        var startDateProperty = validationContext.ObjectType.GetProperty(
            _startDatePropertyName
        );

        if (startDateProperty == null)
        {
            throw new ArgumentException(
                $"Propiedad con el nombre '{_startDatePropertyName}' no encontrada."
            );
        }

        DateTime? startDate = startDateProperty.GetValue(
            validationContext.ObjectInstance
        ) as DateTime?;

        if (startDate == null || endDate == null)
        {
            return ValidationResult.Success;
        }

        if (endDate < startDate)
        {
            var errorMessage = FormatErrorMessage(
                validationContext.DisplayName
            );

            return new ValidationResult(
                errorMessage,
                new[] { validationContext.MemberName! }
            );
        }

        return ValidationResult.Success;
    }
}
