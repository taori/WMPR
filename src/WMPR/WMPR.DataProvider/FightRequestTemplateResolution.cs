using System.Collections.Generic;

namespace WMPR.DataProvider
{
	public class FightRequestTemplateResolution
	{
		public Dictionary<RequestWildcard, string> MappingValues { get; } = new Dictionary<RequestWildcard, string>();

		public string ResolvedTemplate { get; set; }
	}
}