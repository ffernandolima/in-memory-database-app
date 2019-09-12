
namespace InMemoryDatabase.Commands.Interfaces
{
	public interface ICommand
	{
		object Execute(string[] args);
	}
}
