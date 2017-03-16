using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public class GenericHitDetectionParser : FightContentProvider, IHitDetectionParser
	{
		public GenericHitDetectionParser()
		{
		}

		public GenericHitDetectionParser(FightRequestTemplate template)
		{
			Template = template;
		}

		public FightRequestTemplate Template { get; set; }

		public static readonly Regex ParsingRegex = new Regex("<tr[^>]+main-table-row[^>]*>.+?<a[^>]+>(?<name>[^<]+).+?<span[^>]+>(?<dmgTaken>\\d+)\\$.+?<td class=\"num\" >.+?(?<casts>\\d+).+?<td class=\"num tooltip main-table-hits\">.+?(?<hits>\\d+).+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public async Task<List<HitDetectionResult>> GetResultsFromContentAsync(string content)
		{
			var result = new List<HitDetectionResult>();
			var matches = await Task.Run(() => ParsingRegex.Matches(content));
			foreach (Match match in matches)
			{
				var total = int.Parse(match.Groups["dmgTaken"].Value);
				var casts = int.Parse(match.Groups["casts"].Value);
				var hits = int.Parse(match.Groups["hits"].Value);
				var playerName = match.Groups["name"].Value;
				var average = total / hits;

				result.Add(new HitDetectionResult()
				{
					TotalDamage = total,
					AverageHitDamage = average,
					Casts = casts,
					Hits = hits,
					PlayerName = playerName,
				});
			}
			return result;
		}

		public async Task<List<HitDetectionResult>> GetResultsAsync(FightContextData context)
		{
			var content = await this.GetFightContentAsync(Template, context).ConfigureAwait(false);
			return await GetResultsFromContentAsync(content).ConfigureAwait(false);
		}

		public void SetTemplate(FightRequestTemplate template)
		{
			Template = template;
		}

		public string GetDisplayName()
		{
			return "Trefferparser";
		}

		public IEnumerable<string> GetTemplateKeys()
		{
			yield return $"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.AverageHitDamage)}";
			yield return $"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.Casts)}";
			yield return $"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.Hits)}";
			yield return $"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.PlayerName)}";
			yield return $"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.AverageHitDamage)}";
		}
	}
}