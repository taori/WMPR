using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;
using System.Threading.Tasks;
using WMPR.DataProvider.Json;

namespace WMPR.DataProvider.Test
{
	[TestFixture]
	public class GenericHitDetectionReportProviderTests
	{
		[Test]
		public async Task VerifySkorpyronHits()
		{
			var requestTemplate = new FightRequestTemplate("test", "reports/table/damage-taken/"+RequestWildcard.ReportId.Key + "/"+ RequestWildcard.FightId.Key + "/" + RequestWildcard.FightStart.Key + "/" + RequestWildcard.FightEnd.Key + "/source/0/0/0/0/0/210074/-1.0.-1/0/Any/Any/2/1849?pins=2%24Off%24%23244F4B%24auras-gained%240%240.0.0.Any%240.0.0.Any%24true%240.0.0.Any%24true%24204284%24false%24false%2464&");
			var provider = new GenericHitDetectionParser(requestTemplate);
			var context = new FightContextData();
			context.ReportId = "ha6AMNZcYjBr3tbW";
			context.Fight = new Fight()
			{
				id = 2,
				start_time = 250751,
				end_time = 642520,
			};
			

			var result = await provider.GetResultsAsync(context);

			Assert.That(result.Count, Is.EqualTo(5));

			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nullidan"), Is.Not.Null);
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nullidan")?.TotalDamage, Is.EqualTo(2230363));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nullidan")?.AverageHitDamage, Is.EqualTo(1115181).Within(1));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nullidan")?.Casts, Is.EqualTo(6));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nullidan")?.Hits, Is.EqualTo(2));

			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Abatuu"), Is.Not.Null);
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Abatuu")?.TotalDamage, Is.EqualTo(2164916));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Abatuu")?.AverageHitDamage, Is.EqualTo(1082458).Within(1));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Abatuu")?.Casts, Is.EqualTo(6));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Abatuu")?.Hits, Is.EqualTo(2));

			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Syrolyn"), Is.Not.Null);
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Syrolyn")?.TotalDamage, Is.EqualTo(1224738));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Syrolyn")?.AverageHitDamage, Is.EqualTo(1224738).Within(1));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Syrolyn")?.Casts, Is.EqualTo(6));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Syrolyn")?.Hits, Is.EqualTo(1));

			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nubbadin"), Is.Not.Null);
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nubbadin")?.TotalDamage, Is.EqualTo(1106572));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nubbadin")?.AverageHitDamage, Is.EqualTo(1106572).Within(1));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nubbadin")?.Casts, Is.EqualTo(6));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Nubbadin")?.Hits, Is.EqualTo(1));

			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Gorkow"), Is.Not.Null);
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Gorkow")?.TotalDamage, Is.EqualTo(975301));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Gorkow")?.AverageHitDamage, Is.EqualTo(975301).Within(1));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Gorkow")?.Casts, Is.EqualTo(6));
			Assert.That(result.FirstOrDefault(d => d.PlayerName == "Gorkow")?.Hits, Is.EqualTo(1));
		}
	}
}
