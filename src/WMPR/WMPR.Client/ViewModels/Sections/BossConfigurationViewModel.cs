using WMPR.Client.Caliburn;
using WMPR.Client.Interfaces;

namespace WMPR.Client.ViewModels.Sections
{
	public class BossConfigurationViewModel : ScreenValidationBase, IMainTabsControl
	{
		public BossConfigurationViewModel()
		{
			DisplayName = "Auswertungseinstellungen";
			IsCloseable = false;
		}

		public int Order { get; }
	}
}