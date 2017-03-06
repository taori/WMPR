using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace WMPR.Client.ViewModels.Sections
{
	public class WarcraftLogsUrlValidationRule : ValidationRule
	{
		public static readonly Regex ValidationRegex = new Regex("https://www.warcraftlogs.com/reports/(?'reportId'[^/]+)", RegexOptions.Compiled);
		
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (string.IsNullOrEmpty(value.ToString()))
				return new ValidationResult(false, "Eingabe darf nicht leer sein.");
			return ValidationRegex.IsMatch(value.ToString()) ? ValidationResult.ValidResult : new ValidationResult(false, "Die Eingabe ist keine gültige Adresse (https://www.warcraftlogs.com/reports/xxxxxxxxxxx).");
		}
	}
}