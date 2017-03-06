using System;
using System.Composition.Convention;
using System.Linq;
using System.Reflection;
using WMPR.Client.Interfaces;
using WMPR.Client.ViewModels.Windows;

namespace WMPR.Client.Mef
{
	public static class MefExtensions
	{
		private static void ApplySharePolicy(PartConventionBuilder export, PartCreationPolicyAttribute partCreationAttribute)
		{
			if (export != null && partCreationAttribute != null)
			{
				if (partCreationAttribute.SharingBoundary == PartCreationPolicyAttribute.DefaultShared)
				{
					export.Shared();
				}
				else if (partCreationAttribute.SharingBoundary == PartCreationPolicyAttribute.DefaultNoShare)
				{
					// nothing to do here.
				}
				else
				{
					export.Shared(partCreationAttribute.SharingBoundary);
				}
			}
		}

		public static void ImportFromAttributes(this ConventionBuilder convention, Assembly[] assemblies)
		{
			var allTypes = assemblies.SelectMany(d => d.ExportedTypes);
			var done = false;
			foreach (var type in allTypes)
			{
				var inheritedExportAttribute = type.GetCustomAttribute<InheritedExportAttribute>(false);
				if (inheritedExportAttribute != null)
				{
					var partCreationAttribute = type.GetCustomAttribute<PartCreationPolicyAttribute>(false);
					foreach (var derivedType in allTypes.Where(d => !d.IsAbstract && !d.IsInterface && type.IsAssignableFrom(d)))
					{
						partCreationAttribute = partCreationAttribute ?? derivedType.GetCustomAttribute<PartCreationPolicyAttribute>(false);
						if (derivedType == typeof(ShellViewModel))
						{
							if (done)
								continue;
							var builder = convention.ForType<ShellViewModel>();
							builder.Export(config => config.AsContractType<ITabsHost>()).Export(config => config.AsContractType<IShell>()).Shared();
							done = true;
						}
						else
						{
							if (string.IsNullOrEmpty(inheritedExportAttribute.ContactName))
							{
								var export = convention.ForType(derivedType).Export(config => config.AsContractType(inheritedExportAttribute.ContractType));
								ApplySharePolicy(export, partCreationAttribute);
							}
							else
							{
								var export = convention.ForType(derivedType).Export(config => config.AsContractType(inheritedExportAttribute.ContractType).AsContractName(inheritedExportAttribute.ContactName));
								ApplySharePolicy(export, partCreationAttribute);
							}
						}
					}
				}
			}
		}
	}
}