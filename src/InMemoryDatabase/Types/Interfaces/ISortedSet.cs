
namespace InMemoryDatabase.Types.Interfaces
{
	internal interface ISortedSet
	{
		int Count { get; }

		void SortSet();
		void Add(double score, string member);
		int? Rank(string member);
		ISortedSet Range(int index, int count);

		string ToString();
		string ToString(bool withScores);
	}
}
