using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using WMPR.Client.Caliburn;
using WMPR.Client.Framework;
using WMPR.Client.Interfaces;
using WMPR.Client.ViewModels.Windows;

namespace WMPR.Client.ViewModels.Sections
{
	public class ReportListViewModel : ScreenValidationBase, IMainTabsControl
	{
		public int Order { get; }

		public ReportListViewModel()
		{
			IsCloseable = false;
			DisplayName = "Logübersicht";
			CreateNewReportCommand = new RelayCommand<object>(o => CreateNewReport());
			ClearCacheCommand = new RelayCommand(o => ClearCacheExecute());
			OpenReportCommand = new RelayCommand(OpenReportExecute);
			// https://www.warcraftlogs.com/reports/qQR1mBMT72ztxpkA
			// https://www.warcraftlogs.com/reports/XqkGMZ8BRFW6ryN4/
		}

		private void OpenReportExecute(object o)
		{
			var reportViewModel = o as ReportViewModel;
			var newTab = new ReportEvaluationViewModel(reportViewModel);
			if (reportViewModel == null)
				return;

			newTab.DisplayName = reportViewModel.ReportId;
			ITabsHost host;
			if (DI.TryGetService<ITabsHost>(out host))
				host.Add(newTab);
		}

		private string _newReportUrl;

		public string NewReportUrl
		{
			get { return _newReportUrl; }
			set { SetValue(ref _newReportUrl, value, nameof(NewReportUrl)); }
		}

		public ICommand CreateNewReportCommand { get; set; }

		public async void CreateNewReport()
		{
			var rule = new WarcraftLogsUrlValidationRule();
			var validationResult = rule.Validate(NewReportUrl, CultureInfo.CurrentUICulture);
			if (!validationResult.IsValid)
			{
				MessageBox.Show(validationResult.ErrorContent.ToString());
				return;
			}

			if (Reports.Any(d => d.LoaderText.Equals(NewReportUrl)))
			{
				MessageBox.Show("Dieser Bericht existiert bereits.");
				return;
			}

			var newUrl = NewReportUrl;
			NewReportUrl = null;

			await CreateReportViewModelAsync(newUrl);
		}

		private async Task CreateReportViewModelAsync(string url)
		{
			var reportViewModel = new ReportViewModel(url)
			{
				LoaderText = url,
				DisplayText = url,
			};


			Reports.Add(reportViewModel);

			await reportViewModel.LoadAsync();
		}

		private void ClearCacheExecute()
		{
			var reportsCache = RoamingHelper.GetRoamedPath("Cache", "Reports");
			if (Directory.Exists(reportsCache))
				Directory.Delete(reportsCache, true);

			Reports.Clear();
		}

		protected override async void OnInitialize()
		{
			base.OnInitialize();
			var path = RoamingHelper.GetRoamedPath("Cache", "Reports");

			if (!Directory.Exists(path))
				return;

			var files = Directory.GetFiles(path);
			foreach (var file in files)
			{
				var url = $"https://www.warcraftlogs.com/reports/{Path.GetFileNameWithoutExtension(file)}";
				await CreateReportViewModelAsync(url);
			}
		}

		private BindableCollection<ReportViewModel> _reports;

		public BindableCollection<ReportViewModel> Reports
		{
			get { return _reports ?? (_reports = new BindableCollection<ReportViewModel>()); }
			set { SetValue(ref _reports, value, nameof(Reports)); }
		}

		public ICommand ClearCacheCommand { get; set; }

		public ICommand OpenReportCommand { get; set; }
	}
}