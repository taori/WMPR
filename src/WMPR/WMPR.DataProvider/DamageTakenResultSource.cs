using System.Diagnostics;

namespace WMPR.DataProvider
{
	[DebuggerDisplay("{Name} {Percentage} {Amount}")]
	public class DamageTakenResultSource
	{
		public string Name { get; set; }

		public float Percentage { get; set; }

		public int Amount { get; set; }
	}
}