using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Linq;
using System.Reflection;
using WMPR.Client.Interfaces;
using WMPR.Client.ViewModels.Sections;
using WMPR.Client.ViewModels.Windows;
using WMPR.Client.Views.Sections;

namespace WMPR.Client.Mef
{
	public static class MefExtensions
	{
		public static void ImportFromAttributes(this ConventionBuilder convention, Assembly[] assemblies)
		{
			var exportedTypes = assemblies.SelectMany(d => d.ExportedTypes).ToList();
			var serviceTypes = exportedTypes.Where(d => d.IsDefined(typeof(InheritedExportAttribute))).Select(s => new {type = s, attr = s.GetCustomAttribute<InheritedExportAttribute>() }).ToList();
			var creationPolicyByServiceType = serviceTypes.ToDictionary(key => key.type, val => new { creationPolicyAttr = val.type.GetCustomAttribute<PartCreationPolicyAttribute>(), exportAttr = val.attr });

			PartConventionBuilder partBuilder;
			foreach (var implementationType in exportedTypes)
			{
				partBuilder = null;
				if (implementationType.IsAbstract || implementationType.IsInterface || !implementationType.IsClass)
					continue;
					
				PartCreationPolicyAttribute sharePolicy = null;
				foreach (var serviceType in creationPolicyByServiceType)
				{
					if (!serviceType.Key.IsAssignableFrom(implementationType))
						continue;

					partBuilder = partBuilder ?? convention.ForType(implementationType);
					if (string.IsNullOrEmpty(serviceType.Value.exportAttr.ContactName))
					{
						partBuilder.Export(config => config.AsContractType(serviceType.Value.exportAttr.ContractType));
					}
					else
					{
						partBuilder.Export(config => config.AsContractType(serviceType.Value.exportAttr.ContractType).AsContractName(serviceType.Value.exportAttr.ContactName));
					}

					sharePolicy = serviceType.Value.creationPolicyAttr;
				}


				if (sharePolicy != null)
				{
					if (!string.IsNullOrEmpty(sharePolicy.SharingBoundary) && sharePolicy.SharingBoundary != PartCreationPolicyAttribute.DefaultShared)
					{
						partBuilder.Shared(sharePolicy.SharingBoundary);
						continue;
					}
					if (sharePolicy.SharingBoundary == PartCreationPolicyAttribute.DefaultShared)
					{
						partBuilder.Shared();
						continue;
					}
				}
			}
		}
	}
}