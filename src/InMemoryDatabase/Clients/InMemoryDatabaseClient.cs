using InMemoryDatabase.Clients.Interfaces;
using InMemoryDatabase.Exceptions;
using InMemoryDatabase.Extensions;
using InMemoryDatabase.Interfaces;
using InMemoryDatabase.Types;
using System;
using System.Collections.Generic;

namespace InMemoryDatabase.Clients
{
	public class InMemoryDatabaseClient : IInMemoryDatabaseClient
	{
		private static readonly Dictionary<string, object> Locks = new Dictionary<string, object>();

		private IInMemoryDatabaseImpl _databaseImpl;
		private enum ZaddOption { None, Xx, Nx, Ch, Incr };

		public InMemoryDatabaseClient()
			: this(InMemoryDatabaseImpl.Instance)
		{ }

		internal InMemoryDatabaseClient(IInMemoryDatabaseImpl databaseImpl)
		{
			_databaseImpl = databaseImpl ?? throw new ArgumentNullException(nameof(databaseImpl));
		}

		public string Get(string key)
		{
			lock (SyncLock(key))
			{
				string result;

				var exists = _databaseImpl.Exists(key);
				if (!exists)
				{
					result = new Nil();
				}
				else
				{
					var valueInternal = _databaseImpl.Get<object>(key);
					if (!(valueInternal is string value))
					{
						throw new InMemoryDatabaseOperationException(key, $"WRONGTYPE Operation against a key holding the wrong kind of value");
					}

					result = value;
				}

				return result;
			}
		}

		public string Set(string key, string value)
		{
			var result = SetEx(key, value, -1);

			return result;
		}

		public string SetEx(string key, string value, int seconds)
		{
			string result = "OK";

			lock (SyncLock(key))
			{
				Action SetInternal = () =>
				{
					var exists = _databaseImpl.Exists(key);
					if (exists)
					{
						_databaseImpl.Remove(key);
					}

					if (seconds <= 0)
					{
						_databaseImpl.Add(key, value, ExpirationType.Infinite);
					}
					else
					{
						_databaseImpl.Add(key, value, ExpirationType.Absolute, TimeSpan.FromSeconds(seconds));
					}
				};

				SetInternal.SafeExecution(exceptionHandler: (exception) => result = null);
			}

			return result;
		}

		public string Incr(string key)
		{
			lock (SyncLock(key))
			{
				var exists = _databaseImpl.Exists(key);
				if (!exists)
				{
					var firstValue = "0";
					Set(key, firstValue);
				}

				var valueInternal = Get(key);

				Func<long?> ToLong = () => Convert.ToInt64(valueInternal);

				var result = ToLong.SafeExecution();
				if (!result.HasValue)
				{
					throw new InMemoryDatabaseOperationException(key, $"ERR value is not an integer or out of range");
				}

				var value = result.Value + 1;
				var newValue = value.ToString();

				Set(key, newValue);

				return newValue;
			}
		}

		public string Del(params string[] keys)
		{
			lock (SyncLock($"__del-command-key__"))
			{
				var count = 0;
				foreach (var key in keys)
				{
					lock (SyncLock(key))
					{
						var exists = _databaseImpl.Exists(key);
						if (exists)
						{
							count++;
						}

						_databaseImpl.Remove(key);
					}
				}

				return count.ToString();
			}
		}

		public string Zadd(string key, params string[] values)
		{
			var result = Zadd(key, ZaddOption.None, values);

			return result;
		}

		public string ZaddXx(string key, params string[] values)
		{
			var result = Zadd(key, ZaddOption.Xx, values);

			return result;
		}

		public string ZaddNx(string key, params string[] values)
		{
			var result = Zadd(key, ZaddOption.Nx, values);

			return result;
		}

		public string ZaddCh(string key, params string[] values)
		{
			var result = Zadd(key, ZaddOption.Ch, values);

			return result;
		}

		public string ZaddIncr(string key, params string[] values)
		{
			var result = Zadd(key, ZaddOption.Incr, values);

			return result;
		}

		public string Zcard(string key)
		{
			lock (SyncLock(key))
			{
				string result;

				var exists = _databaseImpl.Exists(key);
				if (!exists)
				{
					result = "0";
				}
				else
				{
					var valueInternal = _databaseImpl.Get<object>(key);
					if (!(valueInternal is SortedSet sortedSet))
					{
						throw new InMemoryDatabaseOperationException(key, $"WRONGTYPE Operation against a key holding the wrong kind of value");
					}

					result = sortedSet.Count.ToString();
				}

				return result;
			}
		}

		public string Zrank(string key, string member)
		{
			lock (SyncLock(key))
			{
				string result;

				var exists = _databaseImpl.Exists(key);
				if (!exists)
				{
					result = new Nil();
				}
				else
				{
					var valueInternal = _databaseImpl.Get<object>(key);
					if (!(valueInternal is SortedSet sortedSet))
					{
						throw new InMemoryDatabaseOperationException(key, $"WRONGTYPE Operation against a key holding the wrong kind of value");
					}

					var rank = sortedSet.Rank(member);
					if (!rank.HasValue)
					{
						result = new Nil();
					}
					else
					{
						result = rank.Value.ToString();
					}
				}

				return result;
			}
		}

		public string Zrange(string key, int start, int stop)
		{
			var result = Zrange(key, start, stop, withScores: false);

			return result;
		}

		public string ZrangeWithScores(string key, int start, int stop)
		{
			var result = Zrange(key, start, stop, withScores: true);

			return result;
		}

		public string DbSize()
		{
			var dbSize = _databaseImpl.Count;

			return dbSize.ToString();
		}

		private string Zadd(string key, ZaddOption option, params string[] values)
		{
			lock (SyncLock(key))
			{
				var exists = _databaseImpl.Exists(key);
				if (!exists)
				{
					var newSortedSet = new SortedSet();
					_databaseImpl.Add(key, newSortedSet, ExpirationType.Infinite);
				}

				var valueInternal = _databaseImpl.Get<object>(key);
				if (!(valueInternal is SortedSet sortedSet))
				{
					throw new InMemoryDatabaseOperationException(key, $"WRONGTYPE Operation against a key holding the wrong kind of value");
				}

				if (option == ZaddOption.Incr)
				{
					var score = values[0];
					var member = values[1];

					Func<double?> ToDouble = () => Convert.ToDouble(score);

					var doubleScore = ToDouble.SafeExecution();
					if (!doubleScore.HasValue)
					{
						throw new InMemoryDatabaseOperationException(key, $"ERR value is not a valid float");
					}

					double newScore;

					var element = sortedSet[member];
					if (element == null)
					{
						sortedSet.Add(doubleScore.Value, member);
						newScore = doubleScore.Value;

						sortedSet.SortSet();
					}
					else
					{
						element.Score += doubleScore.Value;
						newScore = element.Score;

						sortedSet.SortSet();
					}

					return newScore.ToString();
				}
				else
				{
					var count = 0;
					for (int idx = 0; idx < values.Length; idx += 2)
					{
						var score = values[idx];
						var member = string.Empty;

						var memberIdx = idx + 1;
						if (memberIdx < values.Length)
						{
							member = values[memberIdx];
						}

						Func<double?> ToDouble = () => Convert.ToDouble(score);

						var doubleScore = ToDouble.SafeExecution();
						if (!doubleScore.HasValue)
						{
							throw new InMemoryDatabaseOperationException(key, $"ERR value is not a valid float");
						}

						var element = sortedSet[member];
						if (element == null)
						{
							if (option != ZaddOption.Xx)
							{
								sortedSet.Add(doubleScore.Value, member);
								sortedSet.SortSet();

								count++;
							}
						}
						else
						{
							if (option == ZaddOption.Ch && element.Score != doubleScore.Value)
							{
								count++;
							}

							if (option != ZaddOption.Nx)
							{
								element.Score = doubleScore.Value;
								sortedSet.SortSet();
							}
						}
					}

					return count.ToString();
				}
			}
		}

		private string Zrange(string key, int start, int stop, bool withScores)
		{
			lock (SyncLock(key))
			{
				string result;

				var exists = _databaseImpl.Exists(key);
				if (!exists)
				{
					result = new EmptySortedSet();
				}
				else
				{
					var valueInternal = _databaseImpl.Get<object>(key);
					if (!(valueInternal is SortedSet sortedSet))
					{
						throw new InMemoryDatabaseOperationException(key, $"WRONGTYPE Operation against a key holding the wrong kind of value");
					}

					if (sortedSet.Count == 0)
					{
						return new EmptySortedSet();
					}

					if (start == 0 && stop == 0)
					{
						var element = sortedSet[0];
						return element.ToString(withScores);
					}

					if (start < 0)
					{
						start = sortedSet.Count + start;
						if (start < 0)
						{
							start = 0;
						}
					}

					if (stop < 0)
					{
						stop = sortedSet.Count + stop;
					}

					var largestIdx = sortedSet.Count - 1;
					if (start > largestIdx || start > stop)
					{
						return new EmptySortedSet();
					}

					if (stop > largestIdx)
					{
						stop = largestIdx;
					}

					var range = sortedSet.Range(start, stop - start + 1);
					result = range.ToString(withScores);
				}

				return result;
			}
		}

		private static object SyncLock(string publicKey)
		{
			if (!Locks.ContainsKey(publicKey))
			{
				Locks.Add(publicKey, new object());
			}

			return Locks[publicKey];
		}

		#region IDisposable Members

		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_databaseImpl = null;
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
