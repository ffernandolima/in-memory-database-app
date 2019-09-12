using System;

namespace InMemoryDatabase.Interfaces
{
	internal interface IInMemoryDatabaseImpl
	{
		long Count { get; }
		T Get<T>(string key) where T : class;
		bool Exists(string key);
		void Add<T>(string key, T value, ExpirationType expirationType, TimeSpan? expirationInterval = null);
		void Add<T>(string key, Func<T> loader, ExpirationType expirationType, TimeSpan? expirationInterval = null);
		void Remove(string key);
	}
}
