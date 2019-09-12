using InMemoryDatabase.Types.Interfaces;

namespace InMemoryDatabase.Types
{
	internal class SortedSetElement : ISortedSetElement
	{
		public double Score { get; set; }
		public string Member { get; set; }

		public SortedSetElement(double score, string member)
		{
			Score = score;
			Member = member;
		}

		public override string ToString() => ToString(this, withScores: false);

		public string ToString(bool withScores) => ToString(this, withScores);

		private static string ToString(SortedSetElement element, bool withScores)
		{
			if (element == null)
			{
				return null;
			}

			if (withScores)
			{
				return $"{element.Member} {element.Score}";
			}

			return element.Member;
		}
	}
}
