using System;
using System.Diagnostics;

namespace WMPR.DataProvider.Json
{
	[DebuggerDisplay("{name} {kill} diff:{difficulty} health:{bossPercentage} start:{start_time} end:{end_time}")]
	public class Fight
	{
		public int id { get; set; }
		public UInt64 start_time { get; set; }
		public UInt64 end_time { get; set; }
		public int boss { get; set; }
		public string name { get; set; }
		public int size { get; set; }
		public int difficulty { get; set; }
		public bool kill { get; set; }
		public int partial { get; set; }
		public int bossPercentage { get; set; }
		public int fightPercentage { get; set; }
		public int lastPhaseForPercentageDisplay { get; set; }
	}
}