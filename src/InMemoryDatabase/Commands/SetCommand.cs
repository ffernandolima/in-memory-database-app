using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using InMemoryDatabase.Extensions;
using System;

namespace InMemoryDatabase.Commands
{
	public class SetCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public SetCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			if (args.Length < 3)
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			object result;

			var key = args[1];
			var value = args[2];

			if (args.Length == 3)
			{
				result = _client.Set(key, value);
			}
			else if (args.Length == 5)
			{
				var ex = args[3];
				if (ex.ToUpperInvariant() != "EX")
				{
					throw new ArgumentException("(error) ERR Syntax error");
				}

				var seconds = args[4];
				if (string.IsNullOrWhiteSpace(seconds))
				{
					throw new ArgumentException("(error) ERR Syntax error");
				}

				Func<string, int?> ToInt = (string input) => Convert.ToInt32(input);

				var secondsResult = ToInt.SafeExecution(args[4]);
				if (!secondsResult.HasValue)
				{
					throw new ArgumentException("(error) ERR value is not an integer or out of range");
				}

				result = _client.SetEx(key, value, secondsResult.Value);
			}
			else
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			return result;
		}

		#region IDisposable Members

		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_client != null)
					{
						_client.Dispose();
						_client = null;
					}
				}
			}

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Members
	}
}
