using InMemoryDatabase.Commands;
using InMemoryDatabase.Commands.Factories;
using InMemoryDatabase.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace InMemoryDatabase.WebAPI.Controllers
{
	[Route("api/inmemorydatabase")]
	[ApiController]
	public class InMemoryDatabaseController : ControllerBase
	{
		// GET
		// curl -X GET "api/inmemorydatabase/dbsize"
		[HttpGet("{command}")]
		public ActionResult<string> Get(string command)
		{
			var allowedCommands = new[]
			{
				CommandNames.DbSize
			};

			var result = Run(allowedCommands, command, key: null, addKey: false, args: null);

			return result;
		}

		// GET
		// curl -X GET "api/inmemorydatabase/get/key"
		// curl -X GET "api/inmemorydatabase/zcard/key"
		// curl -X GET "api/inmemorydatabase/zrank/key?args=b"
		// curl -X GET "api/inmemorydatabase/zrange/key?args=0&args=-1"
		[HttpGet("{command}/{key}")]
		public ActionResult<string> Get(string command, string key, string[] args)
		{
			var allowedCommands = new[]
			{
				CommandNames.Get,
				CommandNames.Zcard,
				CommandNames.Zrank,
				CommandNames.Zrange
			};

			var result = Run(allowedCommands, command, key: key, addKey: true, args: args);

			return result;
		}

		// PUT
		// curl -X PUT "api/inmemorydatabase/set/key" -d "[ \"1\"]"
		// curl -X PUT "api/inmemorydatabase/incr/key" -d "[]"
		// curl -X PUT "api/inmemorydatabase/zadd/key" -d "[ \"1\", \"a\", \"2\", \"b\", \"3\", \"c\"]"
		[HttpPut("{command}/{key}")]
		public ActionResult<string> Put(string command, string key, [FromBody]string[] args)
		{
			var allowedCommands = new[]
			{
				CommandNames.Set,
				CommandNames.Incr,
				CommandNames.Zadd
			};

			var result = Run(allowedCommands, command, key: key, addKey: true, args: args);

			return result;
		}

		// DELETE
		// curl -X DELETE "api/inmemorydatabase/del?keys=key1&keys=key2"
		[HttpDelete("{command}")]
		public ActionResult<string> Delete(string command, string[] keys)
		{
			var allowedCommands = new[]
			{
				CommandNames.Del
			};

			var result = Run(allowedCommands, command, key: null, addKey: false, args: keys);

			return result;
		}

		private string Run(string[] allowedCommands, string command, string key = null, bool addKey = true, string[] args = null)
		{
			if (string.IsNullOrWhiteSpace(command) || !allowedCommands.Contains(command.ToUpperInvariant()))
			{
				return "Invalid command";
			}

			var count = 1;
			if (addKey)
			{
				count++;
			}

			var argsInternal = new string[count + (args?.Length ?? 0)];
			argsInternal[0] = command;

			if (addKey)
			{
				argsInternal[1] = key ?? string.Empty;
			}

			args?.CopyTo(argsInternal, count);

			var result = RunInternal(argsInternal);

			return result;
		}

		private string RunInternal(string[] args)
		{
			var command = CommandFactory.CreateCommand(args);
			if (command != null)
			{
				Exception exception = null;
				Func<object> ExecuteInternal = () =>
				{
					var resultInternal = command.Execute(args);
					return resultInternal;
				};

				var result = ExecuteInternal.SafeExecution(exceptionHandler: (ex) => exception = ex);
				if (exception == null)
				{
					return (string)result;
				}
				else
				{
					return exception.Message;
				}
			}
			else
			{
				return "Invalid command";
			}
		}
	}
}
