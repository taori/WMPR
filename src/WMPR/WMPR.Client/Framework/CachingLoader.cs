using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WMPR.Client.Framework
{
	public static class CachingLoader
	{
		public static async Task<TData> GetCachedOrLoadAsync<TData>(Func<Task<TData>> load, bool forceLoad, params string[] cacheFileDestination)
		{
			if (forceLoad)
				goto remoteLoad;

			var localContent = await RoamingHelper.TryLoadContentAsync(cacheFileDestination).ConfigureAwait(false);
			if (string.IsNullOrEmpty(localContent))
			{
				goto remoteLoad;
			}

			return await Task.Run(() => JsonConvert.DeserializeObject<TData>(localContent)).ConfigureAwait(false);

			remoteLoad:
			var contentObject = await load().ConfigureAwait(false);

			var serialized = await Task.Run(() => JsonConvert.SerializeObject(contentObject)).ConfigureAwait(false);

			await RoamingHelper.TrySaveContentAsync(serialized, cacheFileDestination).ConfigureAwait(false);

			return contentObject;
		}
	}
}