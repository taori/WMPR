using System.Threading.Tasks;
using WMPR.DataProvider.Json;

namespace WMPR.DataProvider
{
	public interface IReportProvider
	{
		Task<ReportData> GetReportDataAsync(string reportId);
	}
}