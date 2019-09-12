using InMemoryDatabase.Clients;
using InMemoryDatabase.Clients.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Threading;

namespace InMemoryDatabase.Tests
{
	[TestClass]
	public class InMemoryDatabaseClientTests
	{
		private IInMemoryDatabaseClient _client;

		[TestInitialize]
		public void Initialize()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			_client = new InMemoryDatabaseClient();
		}

		[TestMethod]
		public void GetTest()
		{
			var key = $"{nameof(GetTest)}Key";
			var value = $"{nameof(GetTest)}Value";

			string result;

			result = _client.Get(key);
			Assert.AreEqual("nil", result);

			_client.Set(key, value);

			result = _client.Get(key);
			Assert.AreEqual($"{nameof(GetTest)}Value", result);
		}

		[TestMethod]
		public void SetTest()
		{
			var key = $"{nameof(SetTest)}Key";
			var stringValue = $"{nameof(SetTest)}Value";

			var integerValue = "1";

			string result;

			result = _client.Set(key, stringValue);
			Assert.AreEqual("OK", result);

			result = _client.Set(key, integerValue);
			Assert.AreEqual("OK", result);

			result = _client.Get(key);
			Assert.AreEqual("1", result);
		}

		[TestMethod]
		public void SetExTest()
		{
			var key = $"{nameof(SetExTest)}Key";
			var value = $"{nameof(SetExTest)}Value";

			string result;

			result = _client.SetEx(key, value, 10);
			Assert.AreEqual("OK", result);

			Thread.Sleep(TimeSpan.FromSeconds(10));

			result = _client.Get(key);
			Assert.AreEqual("nil", result);
		}

		[TestMethod]
		public void IncrTest()
		{
			var key = $"{nameof(IncrTest)}Key";
			var value = "1";

			string result;

			result = _client.Set(key, value);
			Assert.AreEqual("OK", result);

			result = _client.Incr(key);
			Assert.AreEqual("2", result);
		}

		[TestMethod]
		public void DelTest()
		{
			var key = $"{nameof(DelTest)}Key";
			var value = $"{nameof(DelTest)}Value";

			string result;

			result = _client.Set(key, value);
			Assert.AreEqual("OK", result);

			result = _client.Get(key);
			Assert.AreEqual($"{nameof(DelTest)}Value", result);

			result = _client.Del(key);
			Assert.AreEqual("1", result);

			result = _client.Get(key);
			Assert.AreEqual("nil", result);
		}

		[TestMethod]
		public void ZaddTest()
		{
			var key = $"{nameof(ZaddTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);
		}

		[TestMethod]
		public void ZaddXxTest()
		{
			var key = $"{nameof(ZaddXxTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.ZaddXx(key, new[] { "2", "c", "4", "d" });
			Assert.AreEqual("0", result);

			result = _client.ZrangeWithScores(key, 0, -1);
			Assert.AreEqual("a 1 b 2 c 2", result);
		}

		[TestMethod]
		public void ZaddNxTest()
		{
			var key = $"{nameof(ZaddNxTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.ZaddNx(key, new[] { "2", "c", "4", "d" });
			Assert.AreEqual("1", result);

			result = _client.ZrangeWithScores(key, 0, -1);
			Assert.AreEqual("a 1 b 2 c 3 d 4", result);
		}

		[TestMethod]
		public void ZaddChTest()
		{
			var key = $"{nameof(ZaddChTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.ZaddCh(key, new[] { "1", "a", "2", "c", "4", "d" });
			Assert.AreEqual("2", result);

			result = _client.ZrangeWithScores(key, 0, -1);
			Assert.AreEqual("a 1 b 2 c 2 d 4", result);
		}

		[TestMethod]
		public void ZaddIncrTest()
		{
			var key = $"{nameof(ZaddIncrTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.ZaddIncr(key, new[] { "1", "c" });
			Assert.AreEqual("4", result);
		}

		[TestMethod]
		public void ZcardTest()
		{
			var key = $"{nameof(ZcardTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.Zcard(key);
			Assert.AreEqual("3", result);
		}

		[TestMethod]
		public void ZrankTest()
		{
			var key = $"{nameof(ZrankTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.Zrank(key, "a");
			Assert.AreEqual("0", result);

			result = _client.Zrank(key, "b");
			Assert.AreEqual("1", result);

			result = _client.Zrank(key, "c");
			Assert.AreEqual("2", result);
		}

		[TestMethod]
		public void ZrangeTest()
		{
			var key = $"{nameof(ZrangeTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.Zrange(key, 0, -1);
			Assert.AreEqual("a b c", result);
		}

		[TestMethod]
		public void ZrangeWithScoresTest()
		{
			var key = $"{nameof(ZrangeWithScoresTest)}Key";

			string result;

			result = _client.Zadd(key, new[] { "1", "a", "2", "b", "3", "c" });
			Assert.AreEqual("3", result);

			result = _client.ZrangeWithScores(key, 0, -1);
			Assert.AreEqual("a 1 b 2 c 3", result);
		}

		[TestMethod]
		public void DbSizeTest()
		{
			var key1 = $"{nameof(DbSizeTest)}Key1";
			var value1 = $"{nameof(DbSizeTest)}Value1";

			var key2 = $"{nameof(DbSizeTest)}Key2";
			var value2 = $"{nameof(DbSizeTest)}Value2";

			string result;

			result = _client.Set(key1, value1);
			Assert.AreEqual("OK", result);

			result = _client.Set(key2, value2);
			Assert.AreEqual("OK", result);

			result = _client.DbSize();
			Assert.AreEqual("2", result);
		}
	}
}
