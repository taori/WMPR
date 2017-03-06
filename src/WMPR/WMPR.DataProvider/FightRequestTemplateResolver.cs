using System;
using System.Text;

namespace WMPR.DataProvider
{
	public static class FightRequestTemplateResolver
	{
		public static FightRequestTemplateResolution Resolve(FightRequestTemplate template, FightContextData context)
		{
			if (template == null) throw new ArgumentNullException(nameof(template));
			if (context?.Fight == null) throw new ArgumentNullException(nameof(context));

			var result = new FightRequestTemplateResolution();

			result.MappingValues.Clear();
			result.MappingValues.Add(RequestWildcard.ReportId, context.ReportId);
			result.MappingValues.Add(RequestWildcard.FightEnd, context.Fight.end_time.ToString() ?? string.Empty);
			result.MappingValues.Add(RequestWildcard.FightStart, context.Fight.start_time.ToString() ?? string.Empty);
			result.MappingValues.Add(RequestWildcard.FightId, context.Fight.id.ToString() ?? string.Empty);

			var sb = new StringBuilder(template.RequestTemplate);
			foreach (var mapping in result.MappingValues)
			{
				sb.Replace(mapping.Key.Key, mapping.Value);
			}

			result.ResolvedTemplate = sb.ToString();
			return result;
		}
	}
}