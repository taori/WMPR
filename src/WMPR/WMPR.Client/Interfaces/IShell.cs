using WMPR.Client.Mef;

namespace WMPR.Client.Interfaces
{
	[PartCreationPolicy(true)]
	[InheritedExport(typeof(IShell))]
	public interface IShell
	{

	}
}