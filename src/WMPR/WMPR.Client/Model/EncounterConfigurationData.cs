using System.Collections.Generic;
using WMPR.Client.ViewModels.Sections;

namespace WMPR.Client.Model
{
	public class EncounterConfigurationData
	{
		public string MappingKey { get; set; }

		public List<TemplateViewModel> Templates { get; set; }
	}
}