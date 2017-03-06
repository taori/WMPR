using WMPR.Client.Caliburn;
using WMPR.Client.Interfaces;

namespace WMPR.Client.ViewModels.Sections
{
	public class ReportEvaluationViewModel : ScreenValidationBase, IMainTabsControl
	{
		public ReportViewModel Report { get; set; }

		public ReportEvaluationViewModel(ReportViewModel report)
		{
			Report = report;
		}

		public int Order { get; }
	}
}