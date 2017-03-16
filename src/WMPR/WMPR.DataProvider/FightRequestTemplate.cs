using System;
using System.Runtime.Serialization;

namespace WMPR.DataProvider
{
	public class FightRequestTemplate
	{
		public FightRequestTemplate(string id, string requestTemplate)
		{
			RequestTemplate = requestTemplate;
			Id = id;
		}
		
		public string RequestTemplate { get; set; }
		
		public string Id { get; set; }
	}
}