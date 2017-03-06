namespace WMPR.Client.Framework.Converters
{
	public class BooleanConverterInverted : BooleanConverter<bool>
	{
		public BooleanConverterInverted() : base(false, true)
		{
		}
	}
}