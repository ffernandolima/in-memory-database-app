using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using System;
using System.Linq;

namespace InMemoryDatabase.Commands
{
	public class ZaddCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public ZaddCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			if (args.Length < 4)
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			object result;

			var key = args[1];
			var option = args[2];

			if (option.ToUpperInvariant() == "XX")
			{
				var values = args.Skip(3).ToArray();
				result = _client.ZaddXx(key, values);
			}
			else if (option.ToUpperInvariant() == "NX")
			{
				var values = args.Skip(3).ToArray();
				result = _client.ZaddNx(key, values);
			}
			else if (option.ToUpperInvariant() == "CH")
			{
				var values = args.Skip(3).ToArray();
				result = _client.ZaddCh(key, values);
			}
			else if (option.ToUpperInvariant() == "INCR")
			{
				var values = args.Skip(3).ToArray();
				if (values.Length != 2)
				{
					throw new ArgumentException("(error) wrong number of arguments");
				}
				result = _client.ZaddIncr(key, values);
			}
			else
			{
				var values = args.Skip(2).ToArray();
				result = _client.Zadd(key, values);
			}

			return result;
		}
	}
}
