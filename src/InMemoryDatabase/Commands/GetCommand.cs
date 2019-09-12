using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using System;

namespace InMemoryDatabase.Commands
{
	public class GetCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public GetCommand(IInMemoryDatabaseClient client)
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
			var result = _client.Get(key);

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
