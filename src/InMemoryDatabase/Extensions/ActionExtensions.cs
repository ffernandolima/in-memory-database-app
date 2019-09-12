using System;

namespace InMemoryDatabase.Extensions
{
	public static class ActionExtensions
	{
		public static void SafeExecution(this Action action, Action<Exception> exceptionHandler = null, bool? throwException = null)
		{
			try
			{
				action?.Invoke();
			}
			catch (Exception ex)
			{
				exceptionHandler?.Invoke(ex);

				if (throwException.GetValueOrDefault())
				{
					throw;
				}
			}
		}
	}
}
