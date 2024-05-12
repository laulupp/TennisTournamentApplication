namespace Backend.Attributes;

using System;
using System.ComponentModel.DataAnnotations;

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        ErrorMessage = ErrorMessage ?? "Make sure the start date is earlier than the end date.";

        if (value == null || !(value is DateTime))
        {
            return ValidationResult.Success;
        }

        var currentValue = (DateTime)value;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
        if (property == null)
        {
            throw new ArgumentException("Property with this name not found");
        }

        var comparisonValue = property.GetValue(validationContext.ObjectInstance);

        if (comparisonValue == null || !(comparisonValue is DateTime))
        {
            return ValidationResult.Success;
        }

        if (currentValue >= (DateTime)comparisonValue)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}

