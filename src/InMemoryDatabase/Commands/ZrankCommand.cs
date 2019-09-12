using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using System;

namespace InMemoryDatabase.Commands
{
	public class ZrankCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public ZrankCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			if (args.Length < 3)
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			var key = args[1];
			var member = args[2];

			var result = _client.Zrank(key, member);

			return result;
		}
	}
}
