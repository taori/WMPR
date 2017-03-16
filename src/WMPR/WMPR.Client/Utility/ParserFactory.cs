using System;
using System.Collections.Generic;
using System.Linq;
using WMPR.DataProvider;

namespace WMPR.Client.Utility
{
	public static class ParserFactory
	{	
		private static readonly Dictionary<string, Func<IWarcraftLogsContentParser>> Parsers = new Dictionary<string, Func<IWarcraftLogsContentParser>>()
		{
			{ typeof(GenericDamageTakenParser).FullName, () => new GenericDamageTakenParser()},
			{ typeof(GenericHitDetectionParser).FullName, () => new GenericHitDetectionParser()}
		};

		public static IEnumerable<Type> GetParserTypes()
		{
			return GetParsers().Select(s => s.GetType());
		}

		public static IEnumerable<IWarcraftLogsContentParser> GetParsers()
		{
			return Parsers.Values.Select(s => s());
		}

		public static IWarcraftLogsContentParser GetParser(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
				return null;

			Func<IWarcraftLogsContentParser> func;
			if (Parsers.TryGetValue(typeName, out func))
				return func();
			return null;
		}
	}
}