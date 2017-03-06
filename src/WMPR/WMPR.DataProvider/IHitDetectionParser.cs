using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public interface IHitDetectionParser
	{
		Task<List<HitDetectionResult>> GetResultsFromContentAsync(string content);
	}
}