using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using System;

namespace InMemoryDatabase.Commands
{
	public class ZcardCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public ZcardCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			if (args.Length < 2)
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			var key = args[1];
			var result = _client.Zcard(key);

			return result;
		}
	}
}
