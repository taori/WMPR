using System.Collections.Generic;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public interface IHitDetectionParser : IWarcraftLogsContentParser
	{
		Task<List<HitDetectionResult>> GetResultsFromContentAsync(string content);
	}
}