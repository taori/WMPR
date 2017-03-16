using WMPR.Client.Caliburn;
using WMPR.Client.Interfaces;

namespace WMPR.Client.ViewModels.Sections
{
	public class EncounterConfigurationOverviewViewModel : ScreenValidationBase, IMainTabsControl
	{
		public EncounterConfigurationOverviewViewModel()
		{
			DisplayName = "Auswertungseinstellungen";
			IsCloseable = false;
		}

		public int Order { get; }
	}
}