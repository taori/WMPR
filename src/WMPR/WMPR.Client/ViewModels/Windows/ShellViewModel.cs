using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using WMPR.Client.Framework;
using WMPR.Client.Interfaces;
using WMPR.Client.Mef;
using WMPR.Client.Resources;
using WMPR.Client.ViewModels.Sections;

namespace WMPR.Client.ViewModels.Windows
{
	public class ShellViewModel : Conductor<IMainTabsControl>.Collection.OneActive, IShell, ITabsHost
	{
		public ShellViewModel()
		{
		}

		public override string DisplayName
		{
			get { return "Warcraft Mechanics Performance Reporter"; }
			set { }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			var item = new ReportListViewModel();
			this.Items.Add(item);
			this.Items.Add(new EncounterConfigurationOverviewViewModel());

			ActivateItem(item);
		}

		public bool Add(IMainTabsControl tab)
		{
			var evaluation = tab as ReportEvaluationViewModel;
			if (evaluation != null)
			{
				var openTab = Items.Where(d => d is ReportEvaluationViewModel).Cast<ReportEvaluationViewModel>().FirstOrDefault(d => d.Report.ReportId == evaluation.Report.ReportId);
				if (openTab != null)
				{
					Activate(openTab);
					openTab.ReloadDataCommand?.Execute(null);
					return false;
				}
				else
				{
					Items.Add(tab);
					return true;
				}
			}
			else
			{
				Items.Add(tab);
			}

			return true;
		}
		
		public void Activate(IMainTabsControl tab)
		{
			ActivateItem(tab);
		}

		public void CloseQuery(IMainTabsControl tab)
		{
			if (Items.Count > 2)
			{
				var secondLastTab = Items.Reverse().Skip(1).FirstOrDefault();
				ActivateItem(secondLastTab);
			}
			Items.Remove(tab);
		}
	}

	[PartCreationPolicy(true)]
	[InheritedExport(typeof(ITabsHost))]
	public interface ITabsHost
	{
		bool Add(IMainTabsControl tab);
		void Activate(IMainTabsControl tab);
	}
}