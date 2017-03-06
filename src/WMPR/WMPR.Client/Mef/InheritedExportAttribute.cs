using System;

namespace WMPR.Client.Mef
{
	public class InheritedExportAttribute : Attribute
	{
		public Type ContractType { get; set; }

		public string ContactName { get; set; }

		public InheritedExportAttribute(Type contractType)
		{
			ContractType = contractType;
		}

		public InheritedExportAttribute(Type contractType, string contactName)
		{
			ContactName = contactName;
			ContractType = contractType;
		}
	}
}