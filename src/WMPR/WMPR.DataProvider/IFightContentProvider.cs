using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public interface IFightContentProvider
	{
		Task<string> GetFightContentAsync(FightRequestTemplate requestTemplate, FightContextData context);
	}
}