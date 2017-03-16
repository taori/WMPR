using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.DTPS)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.DamageTaken)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.PercentageTaken)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.PlayerName)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Abilities)}.{nameof(DamageTakenResultAbility.Name)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Abilities)}.{nameof(DamageTakenResultAbility.Amount)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Abilities)}.{nameof(DamageTakenResultAbility.Percentage)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Sources)}.{nameof(DamageTakenResultSource.Name)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Sources)}.{nameof(DamageTakenResultSource.Amount)}";
			yield return $"{nameof(DamageTakenResult)}.{nameof(DamageTakenResult.Sources)}.{nameof(DamageTakenResultSource.Percentage)}";
		}
	}
}