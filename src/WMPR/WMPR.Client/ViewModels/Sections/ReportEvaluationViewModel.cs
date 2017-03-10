using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using WMPR.Client.Caliburn;
using WMPR.Client.Framework;
using WMPR.Client.Interfaces;
using WMPR.DataProvider;

namespace WMPR.Client.ViewModels.Sections
{
	public class ReportEvaluationViewModel : ScreenValidationBase, IMainTabsControl
	{
		public ReportViewModel Report { get; set; }

		public ReportEvaluationViewModel(ReportViewModel report)
		{
			Report = report;
			HyperlinkText = $"Auswertung von {report.Url}";
			HyperlinkReference = report.Url;
			HyperlinkOpenCommand = new RelayCommand(HyperlinkOpenExecute);
			ReloadDataCommand = new RelayCommand(ReloadDataExecute);
		}

		private async void ReloadDataExecute(object obj)
		{
			await LoadDataAsync();
		}

		private void HyperlinkOpenExecute(object o)
		{
			Process.Start(new ProcessStartInfo(HyperlinkReference));
		}

		public int Order { get; }

		private string _reportDate;

		public string ReportDate
		{
			get { return _reportDate; }
			set
			{
				if (value == _reportDate) return;
				_reportDate = value;
				NotifyOfPropertyChange();
			}
		}

		private string _reportName;

		public string ReportName
		{
			get { return _reportName; }
			set
			{
				if (value == _reportName) return;
				_reportName = value;
				NotifyOfPropertyChange();
			}
		}

		private string _hyperlinkText;

		public string HyperlinkText
		{
			get { return _hyperlinkText; }
			set
			{
				if (object.Equals(_hyperlinkText, value))
					return;
				_hyperlinkText = value;
				NotifyOfPropertyChange(nameof(HyperlinkText));
			}
		}

		private string _hyperlinkReference;

		public string HyperlinkReference
		{
			get { return _hyperlinkReference; }
			set
			{
				if (object.Equals(_hyperlinkReference, value))
					return;
				_hyperlinkReference = value;
				NotifyOfPropertyChange(nameof(HyperlinkReference));
			}
		}

		private ObservableCollection<EncounterGroupViewModel> _encounters;

		public ObservableCollection<EncounterGroupViewModel> Encounters
		{
			get { return _encounters ?? (_encounters = new ObservableCollection<EncounterGroupViewModel>()); }
			set
			{
				if (object.Equals(_encounters, value))
					return;
				_encounters = value;
				NotifyOfPropertyChange(nameof(Encounters));
			}
		}

		private ICommand _reloadDataCommand;

		public ICommand ReloadDataCommand
		{
			get { return _reloadDataCommand; }
			set
			{
				if (object.Equals(_reloadDataCommand, value))
					return;
				_reloadDataCommand = value;
				NotifyOfPropertyChange(nameof(ReloadDataCommand));
			}
		}

		private ICommand _hyperlinkOpenCommand;

		public ICommand HyperlinkOpenCommand
		{
			get { return _hyperlinkOpenCommand; }
			set
			{
				if (object.Equals(_hyperlinkOpenCommand, value))
					return;
				_hyperlinkOpenCommand = value;
				NotifyOfPropertyChange(nameof(HyperlinkOpenCommand));
			}
		}

		bool _previouslyActivated;
		protected override async void OnActivate()
		{
			base.OnActivate();
			
			if (_previouslyActivated)
				return;
			_previouslyActivated = true;

			await LoadDataAsync();
		}

		private async Task LoadDataAsync()
		{
			DisplayName = "Lade Daten ...";
			Encounters.Clear();

			await ExtractReportInfoAsync();

			var encounter = new EncounterGroupViewModel();
			encounter.BossName = "Boss1";
			encounter.ResultData.Add(new {Hi = "what", Hi2 = "what3"});
			encounter.ResultData.Add(new {Hi = "what2", Hi2 = "what4"});
			Encounters.Add(encounter);

			encounter = new EncounterGroupViewModel();
			encounter.BossName = "Boss2";
			encounter.ResultData.Add(new {Hi = "what", Hi2 = "what3"});
			encounter.ResultData.Add(new {Hi = "what2", Hi2 = "what4"});
			Encounters.Add(encounter);
		}

		private async Task ExtractReportInfoAsync()
		{
			var provider = new ReportMetaDataProvider(Report.ReportId);
			var result = await provider.GetMetadataAsync();
			ReportName = result.Name;
			ReportDate = result.Date.ToString("D");
			
			DisplayName = ReportName;
			this.HyperlinkText = $"{result.Name} {result.Date} - {result.Reporter}";
		}
	}

	public class EncounterGroupViewModel : PropertyChangedBase
	{
		private string _bossName;

		public string BossName
		{
			get { return _bossName; }
			set
			{
				if (object.Equals(_bossName, value))
					return;
				_bossName = value;
				NotifyOfPropertyChange(nameof(BossName));
			}
		}

		private ObservableCollection<object> _resultData;

		public ObservableCollection<object> ResultData
		{
			get { return _resultData ?? (_resultData = new ObservableCollection<object>()); }
			set
			{
				if (object.Equals(_resultData, value))
					return;
				_resultData = value;
				NotifyOfPropertyChange(nameof(ResultData));
			}
		}
	}
}