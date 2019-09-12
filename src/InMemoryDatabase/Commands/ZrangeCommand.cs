using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using InMemoryDatabase.Extensions;
using System;

namespace InMemoryDatabase.Commands
{
	public class ZrangeCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public ZrangeCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			if (args.Length < 4)
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			var key = args[1];

			Func<string, int?> ToInt = (string input) => Convert.ToInt32(input);

			var start = ToInt.SafeExecution(args[2]);
			var stop = ToInt.SafeExecution(args[3]);

			if (!start.HasValue || !stop.HasValue)
			{
				throw new ArgumentException("(error) ERR value is not an integer or out of range");
			}

			object result;

			if (args.Length == 4)
			{
				result = _client.Zrange(key, start.Value, stop.Value);
			}
			else if (args.Length == 5)
			{
				var withScores = args[4];
				if (withScores.ToUpperInvariant() != "WITHSCORES")
				{
					throw new ArgumentException("(error) ERR Syntax error");
				}

				result = _client.ZrangeWithScores(key, start.Value, stop.Value);
			}
			else
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			return result;
		}
	}
}
