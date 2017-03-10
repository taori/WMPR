using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public class ReportMetaDataProvider : WarcraftLogsProviderBase
	{
		public ReportMetaDataProvider(string reportId)
		{
			ReportId = reportId;
		}

		public string ReportId { get; set; }

		/**
		 * <div id="report-title-container">
Trillax Mythic
<span id="report-header-owner-text"><span style="font-size:13px">Created by <span class="white">asd</span> on <span id="reportdate">Wed Jan 11 2017</span></span></span>
<script>
var reportDate = new Date(1484168972547)
document.getElementById('reportdate').innerHTML = reportDate.toDateString()
</script>
</div>
		 */

		private static readonly Regex ReportInfoRegex = new Regex("id=\"report-title-container\">(?\'reportName\'[^<]+).+?>(?\'reporterName\'[^<]+)</span.+?Date\\((?\'date\'[\\d]+)\\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		
		public async Task<ReportMetaData> GetMetadataAsync()
		{
			var content = await DownloadStringAsync($"/reports/{ReportId}").ConfigureAwait(false);
			var result = new ReportMetaData();

			var match = ReportInfoRegex.Match(content);
			if (match.Success)
			{
				var date = match.Groups["date"].Value;
				long parsedUnixTime;
				if (long.TryParse(date, out parsedUnixTime))
					result.Date = new DateTime(1970, 1, 1).AddMilliseconds(parsedUnixTime);
				result.Name = match.Groups["reportName"].Value?.Trim();
				result.Reporter = match.Groups["reporterName"].Value;
			}

			return result;
		}
	}

	public class ReportMetaData
	{
		public string Name { get; set; }

		public DateTime Date { get; set; }

		public string Reporter { get; set; }
	}
}