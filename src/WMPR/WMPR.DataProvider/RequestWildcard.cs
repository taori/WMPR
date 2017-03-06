namespace WMPR.DataProvider
{
	public class RequestWildcard
	{
		public string Key { get; }

		private RequestWildcard(string key)
		{
			Key = key;
		}

		private RequestWildcard() { }

		public static RequestWildcard FightStart = new RequestWildcard("{fightStart}");
		public static RequestWildcard FightEnd = new RequestWildcard("{fightEnd}");
		public static RequestWildcard ReportId = new RequestWildcard("{reportId}");
		public static RequestWildcard FightId = new RequestWildcard("{fightId}");

		protected bool Equals(RequestWildcard other)
		{
			return string.Equals(Key, other.Key);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RequestWildcard) obj);
		}

		public override int GetHashCode()
		{
			return (Key != null ? Key.GetHashCode() : 0);
		}
	}
}