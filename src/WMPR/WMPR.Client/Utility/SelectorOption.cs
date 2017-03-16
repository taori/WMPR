using System.Collections.Generic;
using System.Diagnostics;

namespace WMPR.Client.Utility
{
	[DebuggerDisplay("SelectorOption {Value}")]
	public class SelectorOption<T>
	{
		public T Value { get; set; }

		public string DisplayText { get; set; }

		public static implicit operator T(SelectorOption<T> value)
		{
			return value.Value;
		}

		public SelectorOption(T value, string displayText)
		{
			Value = value;
			DisplayText = displayText;
		}

		public SelectorOption()
		{
			
		}

		protected bool Equals(SelectorOption<T> other)
		{
			return EqualityComparer<T>.Default.Equals(Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SelectorOption<T>) obj);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(Value);
		}
	}
}