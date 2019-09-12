
namespace InMemoryDatabase.Types
{
	internal class Nil
	{
		private string NilValue => "nil";

		public static implicit operator string(Nil nil) => nil?.NilValue;
		public override string ToString() => this;
	}
}
