using System.Linq;
using System.Threading.Tasks;
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
			this.Items.Add(new BossConfigurationViewModel());

			ActivateItem(item);
		}

		public void Add(IMainTabsControl tab)
		{
			Items.Add(tab);
		}

		public void Activate(IMainTabsControl tab)
		{
			ActivateItem(tab);
		}
	}

	[PartCreationPolicy(true)]
	[InheritedExport(typeof(ITabsHost))]
	public interface ITabsHost
	{
		void Add(IMainTabsControl tab);
		void Activate(IMainTabsControl tab);
	}
}