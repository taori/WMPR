namespace WMPR.DataProvider.Json
{
	public class ReportData
	{
		public Fight[] fights { get; set; }
		public string lang { get; set; }
		public Friendly[] friendlies { get; set; }
		public Enemy[] enemies { get; set; }
		public FriendlyPet[] friendlyPets { get; set; }
		public object[] enemyPets { get; set; }
		public object[] abilities { get; set; }
		public Phase[] phases { get; set; }
		public int logVersion { get; set; }
	}
}