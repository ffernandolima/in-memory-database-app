
namespace InMemoryDatabase.Types.Interfaces
{
	internal interface ISortedSetElement
	{
		double Score { get; set; }
		string Member { get; set; }

		string ToString();
		string ToString(bool withScores);
	}
}
