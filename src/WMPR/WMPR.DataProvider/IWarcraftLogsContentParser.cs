using System.Collections.Generic;

namespace WMPR.DataProvider
{
	public interface IWarcraftLogsContentParser
	{
		void SetTemplate(FightRequestTemplate template);
		string GetDisplayName();
		IEnumerable<string> GetTemplateKeys();
	}
}