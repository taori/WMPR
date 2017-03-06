using System;

namespace WMPR.DataProvider
{
	public class FightRequestTemplate
	{
		public FightRequestTemplate(string id, string requestTemplate)
		{
			RequestTemplate = requestTemplate;
			Id = id;
			if (string.IsNullOrEmpty(RequestTemplate))
				throw new ArgumentNullException(nameof(RequestTemplate));
		}

		public string RequestTemplate { get; set; }

		public string Id { get; set; }
	}
}