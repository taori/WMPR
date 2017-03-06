using System.Collections.Generic;

namespace WMPR.DataProvider
{
	public class DamageTakenResult
	{
		public string PlayerName { get; set; }

		public int PlayerId { get; set; }

		public int DamageTaken { get; set; }

		// ReSharper disable once InconsistentNaming
		public double DTPS { get; set; }

		public List<DamageTakenResultSource> Sources { get; set; } = new List<DamageTakenResultSource>();

		public List<DamageTakenResultAbility> Abilities { get; set; } = new List<DamageTakenResultAbility>();

		public float PercentageTaken { get; set; }
	}
}