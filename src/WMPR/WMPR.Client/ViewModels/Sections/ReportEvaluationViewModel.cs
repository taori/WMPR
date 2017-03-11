﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WMPR.Client.Caliburn;
using WMPR.Client.Extensions;
using WMPR.Client.Framework;
using WMPR.Client.Framework.Attributes;
using WMPR.Client.Interfaces;
using WMPR.DataProvider;
using WMPR.DataProvider.Json;

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

			await ExtractReportMetaInfoAsync();
			var reportData = await GetReportDataAsync();

			var encounters = BuildEncounters(reportData);

			Encounters.AddRange(encounters);
		}

		private List<EncounterGroupViewModel> BuildEncounters(ReportData reportData)
		{
			var result = new List<EncounterGroupViewModel>();

//			var encounter = new EncounterGroupViewModel();
//			encounter.BossName = "Boss1";
//			encounter.ResultData.Add(new { Hi = "what", Hi2 = "what3" });
//			encounter.ResultData.Add(new { Hi = "what2", Hi2 = "what4" });
//			result.Add(encounter);
//
//			encounter = new EncounterGroupViewModel();
//			encounter.BossName = "Boss2";
//			encounter.ResultData.Add(new { Hi = "what", Hi2 = "what3" });
//			encounter.ResultData.Add(new { Hi = "what2", Hi2 = "what4" });
//			result.Add(encounter);

//			var cell = new DynamicGridCell();

			var fightByBoss = new Dictionary<string, HashSet<int>>();
			var fightById = new Dictionary<int, Fight>();

			foreach (var fight in reportData.fights)
			{
				HashSet<int> set;
				if (!fightByBoss.TryGetValue(fight.name, out set))
				{
					fightByBoss.Add(fight.name, set = new HashSet<int>());
				}

				fightById.Add(fight.id, fight);
				set.Add(fight.id);
			}

			foreach (var bossFightList in fightByBoss)
			{
				var encounter = CreateEncounterGroup(bossFightList, fightByBoss, fightById);
				result.Add(encounter);
			}

//			var encounter = new EncounterGroupViewModel();
//			encounter.BossName = "Boss1";
//			encounter.ResultData.Add(CreateRow("what1", "what2"));
//			encounter.ResultData.Add(CreateRow("what3", "what4"));
//			result.Add(encounter);
//
//			encounter = new EncounterGroupViewModel();
//			encounter.BossName = "Boss2";
//			encounter.ResultData.Add(CreateRow("what5", "what6"));
//			encounter.ResultData.Add(CreateRow("what7", "what8"));
//			result.Add(encounter);

			return result;
		}

		private EncounterGroupViewModel CreateEncounterGroup(KeyValuePair<string, HashSet<int>> bossFightList, Dictionary<string, HashSet<int>> fightByBoss, Dictionary<int, Fight> fightById)
		{
			var encounter = new EncounterGroupViewModel();
			encounter.BossName = bossFightList.Key;

			var cell = new DynamicGridCell();
			cell["Spieler"] = Guid.NewGuid().ToString();
			cell["Spieler2"] = Guid.NewGuid().ToString();
			encounter.ResultData.Add(cell);

			return encounter;
		}

		private static DynamicGridCell CreateRow(string value1, string value2)
		{
//			var expandoObject = new ExpandoObject();
//			var casted = expandoObject as IDictionary<string, object>;
			var expandoObject = new DynamicGridCell();
			var casted = expandoObject as IDictionary<string, object>;
			casted["hiasdasd"] = value1;
			casted["hiasdasfasdasdd"] = value2;
			
			return expandoObject;
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

		private ObservableCollection<DynamicGridCell> _resultData;

		public ObservableCollection<DynamicGridCell> ResultData
		{
			get { return _resultData ?? (_resultData = new ObservableCollection<DynamicGridCell>()); }
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