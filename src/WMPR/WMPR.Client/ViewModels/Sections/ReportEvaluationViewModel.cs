using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WMPR.Client.Caliburn;
using WMPR.Client.Extensions;
using WMPR.Client.Framework;
using WMPR.Client.Framework.Attributes;
using WMPR.Client.Interfaces;
using WMPR.Client.Model;
using WMPR.Client.Utility;
using WMPR.DataProvider;
using WMPR.DataProvider.Json;
using Action = System.Action;

namespace WMPR.Client.ViewModels.Sections
{
	public class SimpleListItemViewModel : PropertyChangedBase, IProgress<string>
	{
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

		public void Report(string value)
		{
			DisplayText = value;
		}
	}

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
			NextEncounterCommand = new RelayCommand(NextEncounterExecute);
			PreviousEncounterCommand = new RelayCommand(PreviousEncounterExecute);
			AnalyzeDataCommand = new RelayCommand(AnalyzeDataExecute);
		}

		private async void AnalyzeDataExecute(object obj)
		{
			var casted = obj as EncounterGroupViewModel;
			var notification = new SimpleListItemViewModel() {DisplayText = DateTime.Now.ToString("F")};
			AnalyzerNotifications.Add(notification);
			var collectionView = CollectionViewSource.GetDefaultView(Encounters);
			collectionView.MoveCurrentTo(casted);
			await RunAnalysisAsync(casted, notification);
			AnalyzerNotifications.Remove(notification);
		}

		private async Task RunAnalysisAsync(EncounterGroupViewModel encounterGroupViewModel, SimpleListItemViewModel notification)
		{
			await encounterGroupViewModel.LoadAsync(notification);
		}

		private void NextEncounterExecute(object obj)
		{
			if (Encounters == null)
				return;
			var view = CollectionViewSource.GetDefaultView(Encounters);
			if (view == null)
				return;
			if (!view.MoveCurrentToNext())
				view.MoveCurrentToPrevious();
		}

		private void PreviousEncounterExecute(object obj)
		{
			if (Encounters == null)
				return;
			var view = CollectionViewSource.GetDefaultView(Encounters);
			if (view == null)
				return;
			if (!view.MoveCurrentToPrevious())
				view.MoveCurrentToNext();
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

		private ObservableCollection<SimpleListItemViewModel> _runningAnalyzers;

		public ObservableCollection<SimpleListItemViewModel> AnalyzerNotifications
		{
			get { return _runningAnalyzers ?? (_runningAnalyzers = new ObservableCollection<SimpleListItemViewModel>()); }
			set
			{
				if (object.Equals(_runningAnalyzers, value))
					return;
				_runningAnalyzers = value;
				NotifyOfPropertyChange(nameof(AnalyzerNotifications));
			}
		}

		private ICommand _nextEncounterCommand;

		public ICommand NextEncounterCommand
		{
			get { return _nextEncounterCommand; }
			set
			{
				if (Equals(value, _nextEncounterCommand)) return;
				_nextEncounterCommand = value;
				NotifyOfPropertyChange();
			}
		}

		private ICommand _previousEncounterCommand;

		public ICommand PreviousEncounterCommand
		{
			get { return _previousEncounterCommand; }
			set
			{
				if (Equals(value, _previousEncounterCommand)) return;
				_previousEncounterCommand = value;
				NotifyOfPropertyChange();
			}
		}

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

		private ICommand _analyzeDataCommand;

		public ICommand AnalyzeDataCommand
		{
			get { return _analyzeDataCommand; }
			set
			{
				if (object.Equals(_analyzeDataCommand, value))
					return;
				_analyzeDataCommand = value;
				NotifyOfPropertyChange(nameof(AnalyzeDataCommand));
			}
		}

		private bool _isLoading;

		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				if (object.Equals(_isLoading, value))
					return;
				_isLoading = value;
				NotifyOfPropertyChange(nameof(IsLoading));
			}
		}

		bool _previouslyActivated;

		protected override async void OnActivate()
		{
			base.OnActivate();
			
			if (_previouslyActivated)
				return;
			_previouslyActivated = true;

			try
			{
				await LoadDataAsync();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private async Task LoadDataAsync()
		{
			IsLoading = true;

			DisplayName = "Lade Daten ...";
			Encounters.Clear();

			await ExtractReportMetaInfoAsync();
			var reportData = await GetReportDataAsync();

			var encounters = await BuildEncounters(reportData);

			Encounters.AddRange(encounters);

			IsLoading = false;
		}

		private async Task<List<EncounterGroupViewModel>> BuildEncounters(ReportData reportData)
		{
			var result = new List<EncounterGroupViewModel>();
			var bossNames = new HashSet<string>(reportData.fights.Select(s => s.name));
			foreach (var bossName in bossNames)
			{
				var encounterGroup = new EncounterGroupViewModel();
				encounterGroup.BossName = bossName;
				encounterGroup.ReportId = Report.ReportId;
				result.Add(encounterGroup);
			}

			IEncounterConfigurationManager encounterConfigurationManager;
			List<EncounterConfigurationData> configurations = new List<EncounterConfigurationData>();

			if (DI.TryGetService<IEncounterConfigurationManager>(out encounterConfigurationManager))
				configurations = await encounterConfigurationManager.GetAllAsync();
			var configurationViewModels = configurations.Select(config => new EncounterConfigurationViewModel(config)).ToList();

			foreach (var group in result)
			{
				var configurationMatch = configurationViewModels.FirstOrDefault(d => d.IsValidForBoss(group.BossName));
				group.Configuration = configurationMatch ?? new EncounterConfigurationViewModel() { BossMapping = group.BossName };
			}

			var view = CollectionViewSource.GetDefaultView(result);
			if (result.Count > 0)
				view?.MoveCurrentToFirst();

			return result;
		}

		private async Task<ReportData> GetReportDataAsync()
		{
			var provider = new ReportDataProvider();
			var data = await provider.GetReportDataAsync(Report.ReportId);
			return data;
		}

		private async Task ExtractReportMetaInfoAsync()
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
		private CancellationTokenSource _cts;
		public async Task LoadAsync(IProgress<string> progress)
		{
			try
			{
				if (_cts != null)
				{
					progress.Report($"Laufende Analyse wurde abgebrochen.");
					await Task.Delay(2000);
					_cts?.Dispose();
				}
				_cts = new CancellationTokenSource();
				await Task.Run(async () => await LoadImplAsync(progress, _cts.Token), _cts.Token);
			}
			catch (TaskCanceledException e)
			{
				await Task.CompletedTask;
			}
		}

		private async Task LoadImplAsync(IProgress<string> progress, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await Task.Delay(2000);

			cancellationToken.ThrowIfCancellationRequested();

			var provider = new ReportDataProvider();
			progress.Report($"Kampfdaten für {BossName} werden geladen.");
			var reportData = await provider.GetReportDataAsync(ReportId);
			var matchingFights = reportData.fights.Where(d => d.name.ToUpper() == BossName.ToUpper()).ToList();
			progress.Report($"{matchingFights.Count} Kämpfe gefunden.");
			await Task.Delay(1000);

			cancellationToken.ThrowIfCancellationRequested();

			var templates = Configuration.Templates.ToList();
			progress.Report($"{templates.Count} Auswertungsfelder gefunden.");
			await Task.Delay(1000);
			
			Dictionary<string, object> tokenValueByPlayer = new Dictionary<string, object>();
			Dictionary<string, DynamicGridCell> cellByPlayer = new Dictionary<string, DynamicGridCell>();

			var players = new HashSet<string>(reportData.friendlies.Select(s => s.name));
			foreach (var player in players)
			{
				var playerCell = new DynamicGridCell();
				playerCell["Spieler"] = player;
				cellByPlayer.Add(player, playerCell);
			}

			foreach (var template in templates)
			{
				tokenValueByPlayer.Clear();

				cancellationToken.ThrowIfCancellationRequested();

				var parser = ParserFactory.GetParser(template.ParserTypeName);
				parser.SetTemplate(template.Template);
//				var warcraftLogsContentParser = new GenericDamageTakenParser();
//				await warcraftLogsContentParser.GetResultsAsync(new FightContextData());
//				parser = warcraftLogsContentParser;
				foreach (var fight in matchingFights)
				{
					cancellationToken.ThrowIfCancellationRequested();
					
					var fightContext = new FightContextData() { Fight = fight, ReportId = ReportId};
					await parser.ApplyResultMappingAsync(cancellationToken, fightContext, tokenValueByPlayer, template.FieldWildcard);
				}

				foreach (var playerWithTokenValue in tokenValueByPlayer)
				{
					cancellationToken.ThrowIfCancellationRequested();

					DynamicGridCell cell;
					if (cellByPlayer.TryGetValue(playerWithTokenValue.Key, out cell))
					{
						cell[template.FieldWildcard] = playerWithTokenValue.Value;
					}
				}
				
			}

			var cells = new List<KeyValuePair<string, DynamicGridCell>>();
			foreach (var cell in cellByPlayer)
			{
				cells.Add(cell);
			}

			await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
				 ResultData.AddRange(cells.Select(s => s.Value))));
		}

		private EncounterConfigurationViewModel _configuration;
		public EncounterConfigurationViewModel Configuration
		{
			get { return _configuration; }
			set
			{
				if (Equals(value, _configuration)) return;
				_configuration = value;
				NotifyOfPropertyChange();
			}
		}

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

		public string ReportId { get; set; }
	}
}