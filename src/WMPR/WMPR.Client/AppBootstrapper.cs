using System.Composition.Hosting;
using System.Reflection;
using WMPR.Client.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using Caliburn.Micro;
using WMPR.Client.Mef;
using WMPR.Client.ViewModels.Windows;

namespace WMPR.Client
{
	public static class DI
	{
		public static CompositionHost Container;

		public static bool TryGetService<TService>(out TService instance, string contractName = null)
		{ 
			if (contractName != null)
				return Container.TryGetExport(contractName, out instance);

			return Container.TryGetExport(out instance);
		}
	}

	public class AppBootstrapper : BootstrapperBase
	{
		SimpleContainer _container;

		public CompositionHost MefContainer { get; set; }

		public AppBootstrapper()
		{
			Initialize();
		}

		protected override IEnumerable<Assembly> SelectAssemblies()
		{
			return GetAllAssemblies();
		}

		protected override void Configure()
		{
			var convention = new ConventionBuilder();

			var allAssemblies = GetAllAssemblies();

			MefExtensions.ImportFromAttributes(convention, allAssemblies);

			convention.ForType<WindowManager>().Export(d => d.AsContractType(typeof(IWindowManager))).Shared();
			convention.ForType<EventAggregator>().Export(d => d.AsContractType(typeof(IEventAggregator))).Shared();

			var configuration = new ContainerConfiguration();
			configuration.WithAssemblies(allAssemblies, convention);
			DI.Container = MefContainer = configuration.CreateContainer();

			_container = new SimpleContainer();
		}

		private static Assembly[] GetAllAssemblies()
		{
			return new[] { Assembly.GetExecutingAssembly(), typeof(IWindowManager).Assembly };
		}

		protected override object GetInstance(Type service, string key)
		{
			var instance = _container.GetInstance(service, key);
			if (instance != null)
				return instance;

			if (MefContainer.TryGetExport(service, key, out instance))
			{
				return instance;
			}

			throw new InvalidOperationException("Could not locate any instances.");
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			foreach (var instance in _container.GetAllInstances(service))
			{
				yield return instance;
			}
			foreach (var instance in MefContainer.GetExports(service))
			{
				yield return instance;
			}
		}

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			DisplayRootViewFor<IShell>();
		}
	}
}