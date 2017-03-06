using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using WMPR.DataProvider.Json;

namespace WMPR.DataProvider.Test
{
	[TestFixture]
	public class GenericDamageTakenReportProviderTests
	{
		[Test]
		public async Task GetScorpyronReportResults()
		{
			// https://www.warcraftlogs.com/reports/table/damage-taken/ha6AMNZcYjBr3tbW/2/250751/642520/source/0/0/0/0/0/0/-1.0.-1/0/Any/Any/2/1849?pins=2%24Off%24%23244F4B%24auras-gained%240%240.0.0.Any%240.0.0.Any%24true%240.0.0.Any%24true%24204284%24false%24false%2464&
			var template = new FightRequestTemplate("test", $"reports/table/damage-taken/{RequestWildcard.ReportId.Key}/{RequestWildcard.FightId.Key}/{RequestWildcard.FightStart.Key}/{RequestWildcard.FightEnd.Key}/source/0/0/0/0/0/0/-1.0.-1/0/Any/Any/2/1849?pins=2%24Off%24%23244F4B%24auras-gained%240%240.0.0.Any%240.0.0.Any%24true%240.0.0.Any%24true%24204284%24false%24false%2464&");
			
			var provider = new GenericDamageTakenParser(template);
			var context = new FightContextData();
			context.ReportId = "ha6AMNZcYjBr3tbW";
			context.Fight = new Fight()
			{
				id = 2,
				start_time = 250751,
				end_time = 642520,
			};
			var results = await provider.GetResultsAsync(context);


			Assert.That(results, Is.Not.Null);
			Assert.That(results.Count, Is.EqualTo(20));

			var example = results.FirstOrDefault(d => d.PlayerName == "Tremendous");
			Assert.That(example, Is.Not.Null);
			Assert.That(example.DTPS, Is.EqualTo(484996.7).Within(0.1));
			Assert.That(example.PlayerId, Is.EqualTo(7));
			Assert.That(example.PercentageTaken, Is.EqualTo(14.85).Within(0.01));

			Assert.That(example.Abilities.Count, Is.EqualTo(5));

			Assert.That(example.Abilities.FirstOrDefault(d =>d.Name == "Arcanoslash")?.Amount, Is.EqualTo(41670000).Within(10000));

			Assert.That(example.Abilities.FirstOrDefault(d =>d.Name == "Melee")?.Percentage, Is.EqualTo(43.14).Within(0.01));
			Assert.That(example.Abilities.FirstOrDefault(d =>d.Name == "Arcanoslash")?.Percentage, Is.EqualTo(21.93).Within(0.01));
			Assert.That(example.Abilities.FirstOrDefault(d =>d.Name == "Arcane Tether")?.Percentage, Is.EqualTo(9.96).Within(0.01));
			Assert.That(example.Abilities.FirstOrDefault(d =>d.Name == "Acidic Fragments")?.Percentage, Is.EqualTo(8.85).Within(0.01));
			Assert.That(example.Abilities.FirstOrDefault(d =>d.Name == "Toxic Chitin")?.Percentage, Is.EqualTo(4.49).Within(0.01));


			Assert.That(example.Sources.Count, Is.EqualTo(5));

			Assert.That(example.Sources.FirstOrDefault(d => d.Name == "Environment")?.Amount, Is.EqualTo(27460000).Within(10000));

			Assert.That(example.Sources.FirstOrDefault(d => d.Name == "Skorpyron")?.Percentage, Is.EqualTo(67.59).Within(0.01));
			Assert.That(example.Sources.FirstOrDefault(d => d.Name == "Environment")?.Percentage, Is.EqualTo(14.45).Within(0.01));
			Assert.That(example.Sources.FirstOrDefault(d => d.Name == "Crystalline Scorpid")?.Percentage, Is.EqualTo(7.21).Within(0.01));
			Assert.That(example.Sources.FirstOrDefault(d => d.Name == "Volatile Scorpid")?.Percentage, Is.EqualTo(3.59).Within(0.01));
			Assert.That(example.Sources.FirstOrDefault(d => d.Name == "Acidmaw Scorpid")?.Percentage, Is.EqualTo(2.98).Within(0.01));

		}
	}
}
