using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace RSA_Demo
{
    public class BigIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? str = value as string;
            if (str == null)
            {
                return new ValidationResult(false, "Field is null");
            }

            if (str.Length == 0)
            {
                return new ValidationResult(false, "Field is empty");
            }

            if (str.All(char.IsDigit))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Field contains non-numeric characters");
        }
    }
}
