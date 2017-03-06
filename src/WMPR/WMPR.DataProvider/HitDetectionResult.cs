namespace WMPR.DataProvider
{
	public class HitDetectionResult
	{
		public string PlayerName { get; set; }

		public int Casts { get; set; }

		public int Hits { get; set; }

		public int TotalDamage { get; set; }

		public double AverageHitDamage { get; set; }
	}
}