using System;

namespace WMPR.Client.Framework.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class IsModifiedTrackingAttribute : Attribute
	{
	}
}