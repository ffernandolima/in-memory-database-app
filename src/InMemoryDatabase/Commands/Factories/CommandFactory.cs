using InMemoryDatabase.Clients;
using InMemoryDatabase.Commands.Interfaces;
using System.Linq;

namespace InMemoryDatabase.Commands.Factories
{
	public class CommandFactory
	{
		public static ICommand CreateCommand(string[] args)
		{
			if (args == null || !args.Any())
			{
				return null;
			}

			var command = args[0].Trim().ToUpperInvariant();
			var allowedCommands = new[]
			{
				CommandNames.Get,
				CommandNames.Set,
				CommandNames.Incr,
				CommandNames.Del,
				CommandNames.Zadd,
				CommandNames.Zcard,
				CommandNames.Zrank,
				CommandNames.Zrange,
				CommandNames.DbSize
			};

			if (string.IsNullOrWhiteSpace(command) || !allowedCommands.Contains(command))
			{
				return null;
			}

			var client = new InMemoryDatabaseClient();
			switch (command)
			{
				case CommandNames.Get:
					return new GetCommand(client);
				case CommandNames.Set:
					return new SetCommand(client);
				case CommandNames.Incr:
					return new IncrCommand(client);
				case CommandNames.Del:
					return new DelCommand(client);
				case CommandNames.Zadd:
					return new ZaddCommand(client);
				case CommandNames.Zcard:
					return new ZcardCommand(client);
				case CommandNames.Zrank:
					return new ZrankCommand(client);
				case CommandNames.Zrange:
					return new ZrangeCommand(client);
				case CommandNames.DbSize:
					return new DbSizeCommand(client);
			}

			return null;
		}
	}
}
