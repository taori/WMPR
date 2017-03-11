using System.Threading.Tasks;
using Newtonsoft.Json;
using WMPR.DataProvider.Json;

namespace WMPR.DataProvider
{
	public class ReportDataProvider : WarcraftLogsProviderBase, IReportProvider
	{
		public async Task<ReportData> GetReportDataAsync(string reportId)
		{
			var content = await DownloadStringAsync($"reports/fights_and_participants/{reportId}/0");
			if (string.IsNullOrEmpty(content))
			{
				return null;
			}

			var result = JsonConvert.DeserializeObject<ReportData>(content);
			return result;
		}
	}
}