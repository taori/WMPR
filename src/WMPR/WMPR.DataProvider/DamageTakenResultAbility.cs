using System.Diagnostics;

namespace WMPR.DataProvider
{
	[DebuggerDisplay("{Name} {Percentage} {Amount}")]
	public class DamageTakenResultAbility
	{
		public string Name { get; set; }

		public float Percentage { get; set; }

		public int Amount { get; set; }
	}
}