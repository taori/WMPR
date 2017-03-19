using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public class GenericDamageTakenParser : FightContentProvider, IDamageTakenParser
	{
		public GenericDamageTakenParser()
		{
		}

		public GenericDamageTakenParser(FightRequestTemplate template)
		{
			Template = template;
		}

		public FightRequestTemplate Template { get; set; }

		public static readonly Regex RawDataParseRegex = new Regex("<tr[^>]+main-table-row-(?<playerId>[\\d]+)-0[^>]+>.+?<a[^>]+>(?<playerName>[^<]+)</a>.+?(?<dmgTaken>[\\d]+)\\$.+?class=\"report-amount-percent\">(?<percent>[^\\<]+).+?main-per-second-amount\">(?<DTPS>[\\d,.]+).+?</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		
		public static readonly Regex AbilitySectionParseRegex = new Regex("<table[^>]+table-ability-(?<playerId>[\\d]+)-0[^>]+>\r\n\t.+?\r\n</table>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		
		public static readonly Regex AbilityRowParseRegex = new Regex("<tr[^>]+>\r\n.+?\r\n>(?\'name\'[^<]+)</span>\r\n.+?\r\n(?\'damage\'[\\d]+)\\$\r\n.+?\r\namount-percent\">(?\'percent\'[\\d.]+)%<\r\n.+?\r\n</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		
		public static readonly Regex SourceSectionRegex = new Regex("<table[^>]+table-source-(?<playerId>[\\d]+)-0[^>]+>\r\n\t.+?\r\n</table>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		
		public static readonly Regex SourceRowParseRegex = new Regex("<tr[^>]+>\r\n.+?\r\n>(?\'name\'[^<]+)<td\r\n.+?\r\n(?\'damage\'[\\d]+)\\$\r\n.+?\r\namount-percent\">(?\'percent\'[\\d.]+)%<\r\n.+?\r\n</tr>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		public async Task<List<DamageTakenResult>> GetResultsFromContentAsync(string content)
		{
			var result = new List<DamageTakenResult>();
			var rawMatchesTask = Task.Run(() => RawDataParseRegex.Matches(content));
			var abilitySectionTask = Task.Run(() => AbilitySectionParseRegex.Matches(content));
			var sourceMatchesTask = Task.Run(() => SourceSectionRegex.Matches(content));

			await Task.WhenAll(rawMatchesTask, abilitySectionTask, sourceMatchesTask);

			var rawMatches = await rawMatchesTask;
			var abilitySectionMatches = await abilitySectionTask;
			var sourceSectionMatches = await sourceMatchesTask;

			var abilitySectionByPlayerId = abilitySectionMatches.Cast<Match>().ToDictionary(d => int.Parse(d.Groups["playerId"].Value));
			var sourceSectionByPlayerId = sourceSectionMatches.Cast<Match>().ToDictionary(d => int.Parse(d.Groups["playerId"].Value));

			if (rawMatches.Count == 0)
				return result;

			foreach (Match match in rawMatches)
			{
				var playerId = int.Parse(match.Groups["playerId"].Value);
				var playerName = match.Groups["playerName"].Value;
				var damageTaken = int.Parse(match.Groups["dmgTaken"].Value);
				double dtps;
				double.TryParse(match.Groups["DTPS"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out dtps);
				float percentageTaken;
				float.TryParse(match.Groups["percent"].Value.Replace("%", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out percentageTaken);

				var resultRow = new DamageTakenResult()
				{
					PlayerName = playerName,
					DamageTaken = damageTaken,
					PlayerId = playerId,
					DTPS = dtps,
					PercentageTaken = percentageTaken,
				};

				Match value;
				if (abilitySectionByPlayerId.TryGetValue(playerId, out value))
				{
					if (value.Value != null)
					{
						var rowMatches = AbilityRowParseRegex.Matches(value.Value);
						foreach (Match rowMatch in rowMatches)
						{
							var ability = rowMatch.Groups["name"].Value;
							var damage = int.Parse(rowMatch.Groups["damage"].Value);
							var percent = float.Parse(rowMatch.Groups["percent"].Value, CultureInfo.InvariantCulture);

							resultRow.Abilities.Add(new DamageTakenResultAbility()
							{
								Amount = damage,
								Percentage = percent,
								Name = ability,
							});
						}
					}
				}
				
				if (sourceSectionByPlayerId.TryGetValue(playerId, out value))
				{
					if (value.Value != null)
					{
						var rowMatches = SourceRowParseRegex.Matches(value.Value);
						foreach (Match rowMatch in rowMatches)
						{
							var ability = rowMatch.Groups["name"].Value?.Trim();
							var damage = int.Parse(rowMatch.Groups["damage"].Value);
							var percent = float.Parse(rowMatch.Groups["percent"].Value, CultureInfo.InvariantCulture);

							resultRow.Sources.Add(new DamageTakenResultSource()
							{
								Amount = damage,
								Percentage = percent,
								Name = ability,
							});
						}
					}
				}


				result.Add(resultRow);
			}
			return result;
		}

		public async Task<List<DamageTakenResult>> GetResultsAsync(FightContextData context)
		{
			var content = await this.GetFightContentAsync(Template, context).ConfigureAwait(false);
			return await GetResultsFromContentAsync(content).ConfigureAwait(false);
		}

		public void SetTemplate(FightRequestTemplate template)
		{
			this.Template = template;
		}

		public string GetDisplayName()
		{
			return "Schadensparser";
		}

		public IEnumerable<string> GetTemplateKeys()
		{
			return KeyValueMappings.Mappings.Keys;
		}

		private class KeyValueMappings
		{
			public static Dictionary<string, Func<DamageTakenResult, KeyValuePair<string, object>>> Mappings = new Dictionary<string, Func<DamageTakenResult, KeyValuePair<string, object>>>()
			{
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.DTPS)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.DTPS) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.DamageTaken)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.DamageTaken) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.PercentageTaken)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.PercentageTaken) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.PlayerName)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.PlayerName) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Abilities)}.{nameof(DamageTakenResultAbility.Name)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Abilities.FirstOrDefault().Name) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Abilities)}.{nameof(DamageTakenResultAbility.Amount)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Abilities.FirstOrDefault().Amount) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Abilities)}.{nameof(DamageTakenResultAbility.Percentage)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Abilities.FirstOrDefault().Percentage) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Sources)}.{nameof(DamageTakenResultSource.Name)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Sources.FirstOrDefault().Name) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Sources)}.{nameof(DamageTakenResultSource.Amount)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Sources.FirstOrDefault().Amount) },
				{$"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Sources)}.{nameof(DamageTakenResultSource.Percentage)}", (result) => new KeyValuePair<string, object>(result.PlayerName, result.Sources.FirstOrDefault().Percentage) },
			};
		}

		public async Task ApplyResultMappingAsync(CancellationToken cancellationToken, FightContextData fightContext, Dictionary<string, object> requestResults, string templateKey)
		{
			var results = await GetResultsAsync(fightContext).ConfigureAwait(false);
			Func<DamageTakenResult, KeyValuePair<string, object>> extractor;
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