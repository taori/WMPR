using System.Threading.Tasks;

namespace WMPR.DataProvider
{
	public class FightContentProvider : WarcraftLogsProviderBase, IFightContentProvider
	{
		public async Task<string> GetFightContentAsync(FightRequestTemplate requestTemplate, FightContextData context)
		{
			var resolution = FightRequestTemplateResolver.Resolve(requestTemplate, context);
			return await DownloadStringAsync(resolution.ResolvedTemplate).ConfigureAwait(false);
		}
	}
}