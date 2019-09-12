using InMemoryDatabase.Extensions;
using InMemoryDatabase.Types.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace InMemoryDatabase.Types
{
	internal class SortedSet : ISortedSet
	{
		public int Count => Elements.Count;
		protected IList<ISortedSetElement> Elements { get; private set; } = new List<ISortedSetElement>();

		public ISortedSetElement this[int index] => Elements[index];
		public ISortedSetElement this[string member] => Elements.FirstOrDefault(e => e.Member == member);

		public SortedSet()
		{ }

		public SortedSet(IList<ISortedSetElement> elements)
		{
			Elements.AddRange(elements);
		}

		public void SortSet()
		{
			Elements = Elements.OrderBy(e => e.Score).ThenBy(e => e.Member).ToList();
		}

		public void Add(double score, string member)
		{
			var element = new SortedSetElement(score, member);
			Elements.Add(element);
		}

		public int? Rank(string member)
		{
			var element = this[member];
			if (element == null)
			{
				return null;
			}

			var indexOf = Elements.IndexOf(element);

			return indexOf;
		}

		public ISortedSet Range(int index, int count)
		{
			var internalElements = (List<ISortedSetElement>)Elements;

			var range = internalElements.GetRange(index, count);
			var result = new SortedSet(range);

			return result;
		}

		public override string ToString() => ToString(this, withScores: false);

		public string ToString(bool withScores) => ToString(this, withScores);

		private static string ToString(SortedSet sortedSet, bool withScores)
		{
			if (sortedSet == null)
			{
				return null;
			}

			return string.Join(" ", sortedSet.Elements.Select(e => e.ToString(withScores)));
		}
	}
}
