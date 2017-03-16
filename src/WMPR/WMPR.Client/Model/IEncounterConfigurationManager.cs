using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMPR.Client.Mef;

namespace WMPR.Client.Model
{
	[PartCreationPolicy(true)]
	[InheritedExport(typeof(IEncounterConfigurationManager))]
	public interface IEncounterConfigurationManager
	{
		Task<List<EncounterConfigurationData>> GetAllAsync();
		Task SaveAsync(List<EncounterConfigurationData> data);
		Task SaveAsync(EncounterConfigurationData data);
	}
}