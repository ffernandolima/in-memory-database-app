using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace InMemoryDatabase.CLI.DependencyResolution
{
	internal class IoC
	{
		private static readonly Lazy<IoC> _instance = new Lazy<IoC>(() => new IoC(), LazyThreadSafetyMode.PublicationOnly);

		private bool _startupComplete;
		private readonly object _startupLocker;

		private IServiceCollection _services;
		public IServiceCollection Services { get { PerformStartup(); return _services; } }

		public static IoC Instance => _instance.Value;

		private IoC()
		{
			_startupComplete = false;
			_startupLocker = new object();

			_services = null;
		}

		public void PerformStartup()
		{
			if (!_startupComplete)
			{
				lock (_startupLocker)
				{
					if (!_startupComplete)
					{
						ConfigureServices();

						_startupComplete = true;
					}
				}
			}
		}

		private void ConfigureServices()
		{
			if (_services == null)
			{
				var services = new ServiceCollection();

				services.AddLogging(builder =>
				{
					builder.AddDebug();
					builder.AddConsole();
				});

				services.AddTransient<ConsoleApplication>();

				_services = services;
			}
		}
	}
}
