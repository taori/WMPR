using System.Windows;

namespace WMPR.Client.Framework.Converters
{
	public class BooleanToVisibilityConverterInverted : BooleanConverter<Visibility>
	{
		public BooleanToVisibilityConverterInverted() : base(Visibility.Collapsed, Visibility.Visible)
		{
		}
	}
}