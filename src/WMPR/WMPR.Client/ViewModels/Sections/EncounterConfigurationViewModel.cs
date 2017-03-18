using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Amusoft.EventManagement;
using Caliburn.Micro;
using WMPR.Client.Caliburn;
using WMPR.Client.Extensions;
using WMPR.Client.Framework;
using WMPR.Client.Interfaces;
using WMPR.Client.Model;
using WMPR.Client.Utility;
using WMPR.Client.ViewModels.Windows;
using WMPR.DataProvider;

namespace WMPR.Client.ViewModels.Sections
{
	public class EncounterConfigurationViewModel : ScreenValidationBase, IMainTabsControl
	{
		public EncounterConfigurationViewModel()
		{
			SaveCommand = new RelayCommand(SaveExecute);
			NewTemplateCommand = new RelayCommand(NewTemplateExecute);
			AvailableTokenList = $"{RequestWildcard.ReportId.Key} {RequestWildcard.FightId.Key} {RequestWildcard.FightStart.Key} {RequestWildcard.FightEnd.Key}";
		}

		public EncounterConfigurationViewModel(EncounterConfigurationData configurationData) : this()
		{
			this.ConfigurationData = configurationData;
			BossMapping = configurationData.MappingKey;
			Templates = new ObservableCollection<TemplateViewModel>(configurationData.Templates ?? new List<TemplateViewModel>());

			foreach (var template in Templates)
			{
				UpdateParserOptions(template);
			}
		}

		private void UpdateParserOptions(TemplateViewModel template)
		{
			template.ParserTypeChanged -= TemplateOnParserTypeChanged;
			template.ParserTypeChanged += TemplateOnParserTypeChanged;

			if (template.ParserTypeOptions.Count > 0)
				return;

			var parsers = ParserFactory.GetParsers().ToList();
			for (var index = 0; index < parsers.Count; index++)
			{
				var parser = parsers[index];
				template.ParserTypeOptions.Add(new SelectorOption<string>(parser.GetType().FullName, parser.GetDisplayName()));
			}

			var view = CollectionViewSource.GetDefaultView(template.ParserTypeOptions);
			var activeIndex = template.ParserTypeOptions.IndexOf(d => d.Value == template.ParserTypeName);
			if (activeIndex >= 0)
			{
				view.MoveCurrentToPosition(activeIndex);
				UpdateFieldOptions(template, ParserFactory.GetParser(template.ParserTypeName));
			}
		}

		private void TemplateOnParserTypeChanged(object sender, EventArgs eventArgs)
		{
			var template = sender as TemplateViewModel;
			if (template == null)
				return;
			UpdateFieldOptions(template, ParserFactory.GetParser(template.ParserTypeName));
		}

		private void UpdateFieldOptions(TemplateViewModel template, IWarcraftLogsContentParser parser)
		{
			if (template == null)
				return;

			template.FieldOptions.Clear();

			if (parser == null)
				return;

			var keys = parser.GetTemplateKeys().ToList();
			for (var index = 0; index < keys.Count; index++)
			{
				var templateKey = keys[index];
				template.FieldOptions.Add(new SelectorOption<string>(templateKey, templateKey));

				if (template.FieldWildcard == templateKey)
				{
					var changed = CollectionViewSource.GetDefaultView(template.FieldOptions)?.MoveCurrentToPosition(index);
				}
			}

			if (string.IsNullOrEmpty(template.FieldWildcard))
			{
				var changed = CollectionViewSource.GetDefaultView(template.FieldOptions)?.MoveCurrentToFirst();
			}
		}

		private void NewTemplateExecute(object obj)
		{
			var templateViewModel = new TemplateViewModel();
			UpdateParserOptions(templateViewModel);
			Templates.Add(templateViewModel);
		}

		public override string DisplayName => $"Konfig ({BossMapping})";

		public EncounterConfigurationData ConfigurationData { get; set; }

		private string _availableTokenList;

		public string AvailableTokenList
		{
			get { return _availableTokenList; }
			set
			{
				if (object.Equals(_availableTokenList, value))
					return;
				_availableTokenList = value;
				NotifyOfPropertyChange(nameof(AvailableTokenList));
			}
		}

		private void PromptAsEditExecute(object obj)
		{
			ITabsHost host;
			if (DI.TryGetService<ITabsHost>(out host))
			{
				host.Activate(this);
			}
		}

		private async void SaveExecute(object obj)
		{
			IEncounterConfigurationManager host;
			if (DI.TryGetService<IEncounterConfigurationManager>(out host))
			{
				var saveData = new EncounterConfigurationData()
				{
					MappingKey = BossMapping,
					Templates = Templates.ToList()
				};
				await host.SaveAsync(saveData);
			}
		}

		private ObservableCollection<TemplateViewModel> _templates;

		public ObservableCollection<TemplateViewModel> Templates
		{
			get { return _templates ?? (_templates = new ObservableCollection<TemplateViewModel>()); }
			set
			{
				if (object.Equals(_templates, value))
					return;
				_templates = value;
				NotifyOfPropertyChange(nameof(Templates));
			}
		}

		private string _bossMapping;

		public string BossMapping
		{
			get { return _bossMapping; }
			set
			{
				if (object.Equals(_bossMapping, value))
					return;
				_bossMapping = value;
				NotifyOfPropertyChange(nameof(BossMapping));
				NotifyOfPropertyChange(nameof(DisplayName));
			}
		}

		public bool IsValidForBoss(string bossName)
		{
			if (string.IsNullOrEmpty(BossMapping))
				return false;

			bossName = bossName.ToUpper();

			return BossMapping.ToUpper().Split('|',',').Contains(bossName);
		}

		private ICommand _newTemplateCommand;

		public ICommand NewTemplateCommand
		{
			get { return _newTemplateCommand; }
			set
			{
				if (object.Equals(_newTemplateCommand, value))
					return;
				_newTemplateCommand = value;
				NotifyOfPropertyChange(nameof(NewTemplateCommand));
			}
		}

		private ICommand _saveCommand;

		public ICommand SaveCommand
		{
			get { return _saveCommand; }
			set
			{
				if (object.Equals(_saveCommand, value))
					return;
				_saveCommand = value;
				NotifyOfPropertyChange(nameof(SaveCommand));
			}
		}

		private ICommand _promptAsEditCommand;

		public ICommand PromptAsEditCommand
		{
			get { return _promptAsEditCommand ?? (_promptAsEditCommand = new RelayCommand(PromptAsEditExecute)); }
			set
			{
				if (object.Equals(_promptAsEditCommand, value))
					return;
				_promptAsEditCommand = value;
				NotifyOfPropertyChange(nameof(PromptAsEditCommand));
			}
		}

		public int Order { get; }
	}

	public class TemplateViewModel : PropertyChangedBase
	{
		private FightRequestTemplate _template;
		[DataMember]
		public FightRequestTemplate Template
		{
			get { return _template ?? (_template = new FightRequestTemplate(Guid.NewGuid().ToString(), String.Empty)); }
			set
			{
				if (object.Equals(_template, value))
					return;
				_template = value;
				NotifyOfPropertyChange(nameof(Template));
			}
		}

		private string _parserTypeName;
		[DataMember]
		public string ParserTypeName
		{
			get { return _parserTypeName; }
			set
			{
				if (object.Equals(_parserTypeName, value))
					return;
				_parserTypeName = value;
				NotifyOfPropertyChange(nameof(ParserTypeName));
				_parserTypeChanged?.Raise(this, EventArgs.Empty);
			}
		}

		private string _fieldWildcard;
		[DataMember]
		public string FieldWildcard
		{
			get { return _fieldWildcard; }
			set
			{
				if (object.Equals(_fieldWildcard, value))
					return;
				_fieldWildcard = value;
				NotifyOfPropertyChange(nameof(FieldWildcard));
			}
		}

		private readonly WeakEvent _parserTypeChanged = new WeakEvent();

		public event EventHandler ParserTypeChanged
		{
			add { _parserTypeChanged.Add(value); }
			remove { _parserTypeChanged.Remove(value); }
		}

		private ObservableCollection<SelectorOption<string>> _parserTypeOptions;

		public ObservableCollection<SelectorOption<string>> ParserTypeOptions
		{
			get { return _parserTypeOptions ?? (_parserTypeOptions = new ObservableCollection<SelectorOption<string>>()); }
			set
			{
				if (object.Equals(_parserTypeOptions, value))
					return;
				_parserTypeOptions = value;
				NotifyOfPropertyChange(nameof(ParserTypeOptions));
			}
		}

		private ObservableCollection<SelectorOption<string>> _fieldOptions;

		public ObservableCollection<SelectorOption<string>> FieldOptions
		{
			get { return _fieldOptions ?? (_fieldOptions = new ObservableCollection<SelectorOption<string>>()); }
			set
			{
				if (object.Equals(_fieldOptions, value))
					return;
				_fieldOptions = value;
				NotifyOfPropertyChange(nameof(FieldOptions));
			}
		}
	}
}