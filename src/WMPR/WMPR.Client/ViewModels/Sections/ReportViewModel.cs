using System.Threading.Tasks;
using Caliburn.Micro;
using WMPR.Client.Framework;
using WMPR.DataProvider;
using WMPR.DataProvider.Json;

namespace WMPR.Client.ViewModels.Sections
{
	public class ReportViewModel : PropertyChangedBase
	{
		public string Url { get; set; }

		public string ReportId { get; set; }

		public ReportViewModel()
		{
		}

		public ReportViewModel(string url)
		{
			Url = url;
			var match = WarcraftLogsUrlValidationRule.ValidationRegex.Match(url);
			ReportId = match.Groups["reportId"].Value;
		}

		public async Task LoadAsync()
		{
			var reportProvider = new ReportProvider();
			LoaderText = "Daten werden geladen ...";
			var reportData = await CachingLoader.GetCachedOrLoadAsync(async () => await reportProvider.GetReportDataAsync(ReportId), false, "Cache", "Reports", $"{ReportId}.json");

			ReportData = reportData;
			LoaderText = $"{reportData.fights.Length} Kämpfe enthalten.";
		}

		public ReportData ReportData { get; set; }

		private string _displayText;

		public string DisplayText
		{
			get { return _displayText; }
			set
			{
				if (object.Equals(_displayText, value))
					return;
				_displayText = value;
				NotifyOfPropertyChange(nameof(DisplayText));
			}
		}

		private string _loaderText;

		public string LoaderText
		{
			get { return _loaderText; }
			set
			{
				if (object.Equals(_loaderText, value))
					return;
				_loaderText = value;
				NotifyOfPropertyChange(nameof(LoaderText));
			}
		}
	}
}