using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public class WarcraftLogsProviderBase
	{
		private HttpClient CreateClient()
		{
			return new HttpClient()
			{
				BaseAddress = new Uri("https://www.warcraftlogs.com/", UriKind.Absolute)
			};
		}

		protected async Task<string> DownloadStringAsync(string path)
		{
			using (var client = CreateClient())
			{
				Debug.WriteLine($"{nameof(WarcraftLogsProviderBase)}{nameof(DownloadStringAsync)} -> {client.BaseAddress.AbsolutePath}/{path}");
				return await client.GetStringAsync(path).ConfigureAwait(false);
			}
		}
	}
}