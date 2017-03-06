using System.Collections.Generic;

namespace WMPR.DataProvider
{
	public class FightRequestFactory
	{
		public static class FightRequestTemplateType
		{
			public const string ScorpyronHitDetection = "defaultScorpyronHitDetection";
		}

		private static List<FightRequestTemplate> _default;
		public static List<FightRequestTemplate> CreateDefault()
		{
			if (_default == null)
			{
				_default = new List<FightRequestTemplate>();
				_default.Add(new FightRequestTemplate(FightRequestTemplateType.ScorpyronHitDetection, "reports/table/damage-taken/" + RequestWildcard.ReportId.Key + "/" + RequestWildcard.FightId.Key + "/" + RequestWildcard.FightStart.Key + "/" + RequestWildcard.FightEnd.Key + "/source/0/0/0/0/0/210074/-1.0.-1/0/Any/Any/2/1849?pins=2%24Off%24%23244F4B%24auras-gained%240%240.0.0.Any%240.0.0.Any%24true%240.0.0.Any%24true%24204284%24false%24false%2464&"));
			}

			return _default;
		}
	}
}