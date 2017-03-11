using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WMPR.DataProvider.Test
{
	[TestFixture]
	public class ReportProviderTests
	{
		static string[] _sampleReports = new string[]
		{
			"txQpLXckya2qNDVb",
			"RQ4FPmWatXw1f9Aq",
			"ha6AMNZcYjBr3tbW"
		};

		[Test, TestCaseSource(nameof(_sampleReports))]
		public async Task VerifyTestDownloadAndSerialization(string reportId)
		{
			var provider = new ReportDataProvider();
			var result = await provider.GetReportDataAsync(reportId);
			Assert.That(result, Is.Not.Null, "Report data sollte nicht null sein.");
		}
	}
}
