using InMemoryDatabase.CLI.DependencyResolution;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading;

namespace InMemoryDatabase.CLI
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			var serviceProvider = IoC.Instance.Services.BuildServiceProvider();

			serviceProvider.GetService<ConsoleApplication>().Run(args);
		}
	}
}
