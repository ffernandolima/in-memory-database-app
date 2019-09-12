using System;

namespace InMemoryDatabase.Clients.Interfaces
{
	public interface IInMemoryDatabaseClient : IDisposable
	{
		string Get(string key);

		string Set(string key, string value);
		string SetEx(string key, string value, int seconds);

		string Incr(string key);

		string Del(params string[] keys);

		string Zadd(string key, params string[] values);
		string ZaddXx(string key, params string[] values);
		string ZaddNx(string key, params string[] values);
		string ZaddCh(string key, params string[] values);
		string ZaddIncr(string key, params string[] values);

		string Zcard(string key);

		string Zrank(string key, string member);

		string Zrange(string key, int start, int stop);
		string ZrangeWithScores(string key, int start, int stop);

		string DbSize();
	}
}
