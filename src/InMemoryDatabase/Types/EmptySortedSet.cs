
namespace InMemoryDatabase.Types
{
	internal class EmptySortedSet
	{
		private string EmptySortedSetValue => "empty list or set";

		public static implicit operator string(EmptySortedSet emptySortedSet) => emptySortedSet?.EmptySortedSetValue;
		public override string ToString() => this;
	}
}
