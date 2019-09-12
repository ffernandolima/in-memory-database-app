using InMemoryDatabase.Commands.Factories;
using InMemoryDatabase.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace InMemoryDatabase.CLI
{
	internal class ConsoleApplication
	{
		private readonly ILogger<ConsoleApplication> _logger;

		public ConsoleApplication(ILogger<ConsoleApplication> logger)
		{
			_logger = logger;
		}

		public void Run(string[] args)
		{
			try
			{
				if (args == null || !args.Any())
				{
					string input;

					Console.WriteLine("InMemoryDatabase.CLI -> Type 'EXIT' to exit the app");
					Console.WriteLine("> ");

					while ((input = Console.ReadLine()).ToUpperInvariant() != "EXIT")
					{
						args = input.Split(' ').ToArray();

						RunInternal(args);

						Console.WriteLine("> ");
					}
				}
				else
				{
					RunInternal(args);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"InMemoryDatabase.CLI -> Run method -> Internal Error -> {ex.ToString()}");			}
		}

		private static void RunInternal(string[] args)
		{
			var command = CommandFactory.CreateCommand(args);
			if (command != null)
			{
				Exception exception = null;
				Func<object> ExecuteInternal = () =>
				{
					var resultInternal = command.Execute(args);
					return resultInternal;
				};

				var result = ExecuteInternal.SafeExecution(exceptionHandler: (ex) => exception = ex);
				if (exception == null)
				{
					Console.WriteLine(result);
				}
				else
				{
					Console.WriteLine(exception.Message);
				}
			}
			else
			{
				Console.WriteLine("Invalid command");
			}
		}
	}
}
