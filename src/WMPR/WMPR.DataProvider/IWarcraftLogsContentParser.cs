using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public interface IWarcraftLogsContentParser
	{
		void SetTemplate(FightRequestTemplate template);
		string GetDisplayName();
		IEnumerable<string> GetTemplateKeys();
		Task ApplyResultMappingAsync(CancellationToken cancellationToken, FightContextData fightContext, Dictionary<string, object> requestResults, string templateKey);
	}
}