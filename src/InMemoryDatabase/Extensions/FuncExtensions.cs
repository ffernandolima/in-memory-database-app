using System;

namespace InMemoryDatabase.Extensions
{
	public static class FuncExtensions
	{
		public static T SafeExecution<T>(this Func<T> func, Action<Exception> exceptionHandler = null, bool? throwException = null)
		{
			try
			{
				if (func != null)
				{
					return func.Invoke();
				}
			}
			catch (Exception ex)
			{
				exceptionHandler?.Invoke(ex);

				if (throwException.GetValueOrDefault())
				{
					throw;
				}
			}

			return default;
		}

		public static TResult SafeExecution<T, TResult>(this Func<T, TResult> func, T argument, Action<Exception> exceptionHandler = null, bool? throwException = null)
		{
			try
			{
				if (func != null)
				{
					return func.Invoke(argument);
				}
			}
			catch (Exception ex)
			{
				exceptionHandler?.Invoke(ex);

				if (throwException.GetValueOrDefault())
				{
					throw;
				}
			}

			return default;
		}
	}
}
