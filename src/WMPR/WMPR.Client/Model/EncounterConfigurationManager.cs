using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WMPR.Client.Extensions;
using WMPR.Client.Framework;

namespace WMPR.Client.Model
{
	public class EncounterConfigurationManager : IEncounterConfigurationManager
	{
		public async Task<List<EncounterConfigurationData>> GetAllAsync()
		{
			var content = await RoamingHelper.TryLoadContentAsync("configurations.json").ConfigureAwait(false);
			if (string.IsNullOrEmpty(content))
				return new List<EncounterConfigurationData>();
			var result = await Task.Run(() => JsonConvert.DeserializeObject<List<EncounterConfigurationData>>(content)).ConfigureAwait(false);
			return result;
		}

		public async Task SaveAsync(List<EncounterConfigurationData> data)
		{
			await RoamingHelper.TrySaveContentAsync(JsonConvert.SerializeObject(data), "configurations.json").ConfigureAwait(false);
		}

		public async Task SaveAsync(EncounterConfigurationData data)
		{
			var previousData = await GetAllAsync().ConfigureAwait(false);
			var indexOf = previousData.IndexOf(s => s.MappingKey.ToUpper() == data.MappingKey.ToUpper());
			if (indexOf >= 0)
			{
				previousData[indexOf] = data;
			}
			else
			{
				previousData.Add(data);
			}

			await SaveAsync(previousData).ConfigureAwait(false);
		}
	}
}