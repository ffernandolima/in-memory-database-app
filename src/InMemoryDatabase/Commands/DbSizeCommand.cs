using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Commands.Interfaces;
using System;

namespace InMemoryDatabase.Commands
{
	public class DbSizeCommand : ICommand
	{
		private IInMemoryDatabaseClient _client;

		public DbSizeCommand(IInMemoryDatabaseClient client)
		{
			_client = client;
		}

		public object Execute(string[] args)
		{
			var result = _client.DbSize();

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
