using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialManagementSystem.Models.Helpers;

public static class Validations
{
    public static bool ValidateFields(object instance)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(instance);
        return Validator.TryValidateObject(instance, validationContext, validationResults, true);
    }
    
    public static bool ValidateFields(object instance, Type type, List<string> attributesToValidate)
    {
        foreach (var attributeName in attributesToValidate)
        {
            var validationContext = new ValidationContext(instance) { MemberName = attributeName };
            if (!Validator.TryValidateProperty(type.GetProperty(attributeName).GetValue(instance), validationContext, null))
            {
                return false;
            }
        }
        
        return true;
    }
}