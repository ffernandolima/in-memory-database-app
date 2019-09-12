using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using System;
using System.Linq;

namespace InMemoryDatabase.Commands
{
	public class DelCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public DelCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			if (args.Length < 2)
			{
				throw new ArgumentException("(error) wrong number of arguments");
			}

			var keys = args.Skip(1).ToArray();
			var result = _client.Del(keys);

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
