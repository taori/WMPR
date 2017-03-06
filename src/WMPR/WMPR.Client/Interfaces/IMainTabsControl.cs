using Caliburn.Micro;
using WMPR.Client.Mef;

namespace WMPR.Client.Interfaces
{
	[PartCreationPolicy(true)]
	[InheritedExport(typeof(IMainTabsControl))]
	public interface IMainTabsControl : IScreen
	{
		int Order { get; }
	}
}