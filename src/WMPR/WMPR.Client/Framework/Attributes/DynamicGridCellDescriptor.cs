using System;
using System.ComponentModel;

namespace WMPR.Client.Framework.Attributes
{
	public class DynamicGridCellDescriptor : PropertyDescriptor
	{
		public DynamicGridCellDescriptor(string name) : base(name, null)
		{
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override object GetValue(object component)
		{
			return ((DynamicGridCell) component)[Name];
		}

		public override void ResetValue(object component)
		{
			((DynamicGridCell) component)[Name] = null;
		}

		public override void SetValue(object component, object value)
		{
			((DynamicGridCell) component)[Name] = value;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override Type ComponentType => typeof(DynamicGridCell);
		public override bool IsReadOnly => false;
		public override Type PropertyType => typeof(object);
	}
}