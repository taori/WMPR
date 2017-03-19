using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

		public static readonly Regex ParsingRegex = new Regex("<tr[^>]+main-table-row[^>]*>.+?<a[^>]+>(?<name>[^<]+).+?<span[^>]+>(?<dmgTaken>\\d+)\\$.+?<td class=\"num\" >.+?(?<casts>\\d+).+?<td class=\"num tooltip main-table-hits\">.+?(?<hits>\\d+).+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromSeconds(30));

		public async Task<List<HitDetectionResult>> GetResultsFromContentAsync(string content)
		{
			var result = new List<HitDetectionResult>();
			if (!ParsingRegex.IsMatch(content))
				return result;

			var matches = await Task.Run(() => ParsingRegex.Matches(content));
			if (matches == null || matches.Count == 0)
				return result;

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

		private class KeyValueMappings
		{
			public static Dictionary<string, Func<HitDetectionResult, KeyValuePair<string, object>>> Mappings = new Dictionary<string, Func<HitDetectionResult, KeyValuePair<string, object>>>()
			{
				{$"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.AverageHitDamage)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.AverageHitDamage) },
				{$"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.Casts)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Casts) },
				{$"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.Hits)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Hits) },
				{$"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.PlayerName)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.PlayerName) },
				{$"{nameof(HitDetectionResult)}.{nameof(HitDetectionResult.AverageHitDamage)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.AverageHitDamage) },
			};
		}

		public async Task ApplyResultMappingAsync(CancellationToken cancellationToken, FightContextData fightContext, Dictionary<string, object> requestResults, string templateKey)
		{
			var results = await GetResultsAsync(fightContext).ConfigureAwait(false);
			Func<HitDetectionResult, KeyValuePair<string, object>> extractor;
			if (KeyValueMappings.Mappings.TryGetValue(templateKey, out extractor))
			{
				foreach (var parserResult in results)
				{
					var output = extractor(parserResult);
					object value;
					if (requestResults.TryGetValue(output.Key, out value))
					{
						requestResults[output.Key] = output.Value;
					}
					else
					{
						requestResults.Add(output.Key, output.Value);
					}
				}
			}
		}
	}
}