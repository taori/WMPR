using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public interface IDamageTakenParser
	{
		Task<List<DamageTakenResult>> GetResultsFromContentAsync(string content);
	}
}