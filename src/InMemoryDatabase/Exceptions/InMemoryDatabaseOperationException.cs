using System;
using System.Runtime.Serialization;

namespace InMemoryDatabase.Exceptions
{
	public class InMemoryDatabaseOperationException : ApplicationException
	{
		public string Key { get; private set; }

		public InMemoryDatabaseOperationException(string key)
		{
			Key = key;
		}

		public InMemoryDatabaseOperationException(string key, string message)
			: base(GetMessage(key, message))
		{
			Key = key;
		}

		public InMemoryDatabaseOperationException(string key, string message, Exception innerException)
			: base(GetMessage(key, message), innerException)
		{
			Key = key;
		}

		protected InMemoryDatabaseOperationException(string key, SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Key = key;
		}

		private static string GetMessage(string key, string message)
		{
			return $"(error) {message}. Key: {key}";
		}
	}
}
