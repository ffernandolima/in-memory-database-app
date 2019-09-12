using InMemoryDatabase.Interfaces;
using System;
using System.Runtime.Caching;
using System.Threading;

namespace InMemoryDatabase
{
	internal class InMemoryDatabaseImpl : IInMemoryDatabaseImpl
	{
		private static readonly Lazy<InMemoryDatabaseImpl> _instance = new Lazy<InMemoryDatabaseImpl>(() => new InMemoryDatabaseImpl(), LazyThreadSafetyMode.PublicationOnly);
		public static InMemoryDatabaseImpl Instance => _instance.Value;

		private readonly MemoryCache _memoryCache;

		public InMemoryDatabaseImpl()
			: this(MemoryCache.Default)
		{ }

		public InMemoryDatabaseImpl(MemoryCache memoryCache)
		{
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		}

		public long Count => _memoryCache.GetCount();

		public T Get<T>(string key) where T : class
		{
			if (_memoryCache.Get(key) is CacheItem cachedItem)
			{
				return cachedItem.Value as T;
			}

			return null;
		}

		public bool Exists(string key)
		{
			var exists = _memoryCache.Contains(key);

			return exists;
		}

		public void Add<T>(string key, T value, ExpirationType expirationType, TimeSpan? expirationInterval = null)
		{
			Add(key, () => value, expirationType, expirationInterval);
		}

		public void Add<T>(string key, Func<T> loader, ExpirationType expirationType, TimeSpan? expirationInterval = null)
		{
			var policy = new CacheItemPolicy();

			switch (expirationType)
			{
				case ExpirationType.Infinite:
					{
						policy.AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
					}
					break;
				case ExpirationType.Absolute:
					{
						if (!expirationInterval.HasValue)
						{
							throw new ArgumentNullException(nameof(expirationInterval), "Expiration interval cannot be null due to the exipration type is absolute.");
						}

						policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now + expirationInterval.Value);
					}
					break;
				case ExpirationType.Sliding:
					{
						if (!expirationInterval.HasValue)
						{
							throw new ArgumentNullException(nameof(expirationInterval), "Expiration interval cannot be null due to the exipration type is sliding.");
						}

						policy.SlidingExpiration = expirationInterval.Value;
					}
					break;
				case ExpirationType.None:
				default:
					{
						throw new NotSupportedException("Expiration type none is not supported.");
					}
			}

			var data = loader();

			var cacheItem = new CacheItem(key, data);
			_memoryCache.Set(key, cacheItem, policy);
		}

		public void Remove(string key)
		{
			_memoryCache.Remove(key);
		}
	}
}
