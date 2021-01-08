using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alessio.Base.Extensions
{
	public static class EnumerableExtensions
	{
		public static int ArgMin<T>(this IEnumerable<T> source, Comparison<T> comparer = null)
		{
			if (comparer == null)
				comparer = (a, b) => Comparer<T>.Default.Compare(a, b);

			T min = source.FirstOrDefault();
			int argMin = (source.Count() > 0 ? 0 : -1);

			for (int i = 1; i < source.Count(); i++)
			{
				T element = source.ElementAt(i);
				if (comparer(element, min) < 0)
				{
					min = element;
					argMin = i;
				}
			}

			return argMin;
		}

		public static int ArgMax<T>(this IEnumerable<T> source, Comparison<T> comparer = null)
		{
			if (comparer == null)
				comparer = (a, b) => Comparer<T>.Default.Compare(a, b);

			T max = source.FirstOrDefault();
			int argMax = (source.Count() > 0 ? 0 : -1);

			for (int i = 1; i < source.Count(); i++)
			{
				T element = source.ElementAt(i);
				if (comparer(element, max) > 0)
				{
					max = element;
					argMax = i;
				}
			}

			return argMax;
		}

		public static T ArgMin<T, TProp>(this IEnumerable<T> source, Func<T, TProp> selector) where TProp : IComparable
		{
			if (source.Count() == 0) return default(T);

			T argMin = source.FirstOrDefault();
			TProp min = selector(argMin);

			for (int i = 1; i < source.Count(); i++)
			{
				T element = source.ElementAt(i);
				TProp prop = selector(element);
				if (prop.CompareTo(min) < 0)
				{
					argMin = element;
					min = prop;
				}
			}

			return argMin;
		}

		public static T ArgMax<T, TProp>(this IEnumerable<T> source, Func<T, TProp> selector) where TProp : IComparable
		{
			if (source.Count() == 0) return default(T);

			T argMax = source.FirstOrDefault();
			TProp max = selector(argMax);

			for (int i = 1; i < source.Count(); i++)
			{
				T element = source.ElementAt(i);
				TProp prop = selector(element);
				if (prop.CompareTo(max) > 0)
				{
					argMax = element;
					max = prop;
				}
			}

			return argMax;
		}

		public static TProp MaxOrDefault<T, TProp>(this IEnumerable<T> source, Func<T, TProp> selector)
		{
			if (source.Count() == 0)
				return default(TProp);
			else return source.Max(selector);
		}

		public static TProp MinOrDefault<T, TProp>(this IEnumerable<T> source, Func<T, TProp> selector)
		{
			if (source.Count() == 0)
				return default(TProp);
			else return source.Min(selector);
		}

		public static string Join(this string s, IEnumerable<string> others, string separator)
		{
			return string.Join(separator, s, others.ToArray());

			//string result = s;
			//foreach (string o in others)
			//	result += separator + o;
			//return result;
		}

		public static string JoinStrings(this IEnumerable<string> source, string separator = "", string overrideLast = null)
		{
			if (source.Count() == 0) return "";
			else if (overrideLast == null)
				return string.Join(separator, source);
			//return source.First().Join(source.Skip(1), separator);
			else return string.Join(separator, source.Take(source.Count() - 1)) + overrideLast + source.Last();
			//else return source.First().Join(source.Skip(1).Take(source.Count() - 2), separator) + overrideLast + source.Last();
		}

		public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			for (int i = 0; i < source.Count(); i++)
				if (predicate(source.ElementAt(i))) return i;
			return -1;
		}

		public static List<int> SelectIndex<T>(this IEnumerable<T> source, Predicate<T> predicate = null)
		{
			if (predicate == null) predicate = o => true;

			List<int> result = new List<int>();
			for (int i = 0; i < source.Count(); i++)
				if (predicate(source.ElementAt(i))) result.Add(i);
			return result;
		}

		public static List<T> WhereIndex<T>(this IEnumerable<T> source, Predicate<int> predicate)
		{
			List<T> result = new List<T>();

			int count = source.Count();
			for (int i = 0; i < count; i++)
			{
				if (predicate(i))
					result.Add(source.ElementAt(i));
			}

			return result;
		}

		public static bool AllShareProperty<T, TProp>(this IEnumerable<T> source, Func<T, TProp> property)
		{
			if (source.Count() == 0) return true;
			return (source.All(s => property(s).Equals(property(source.ElementAt(0)))));
		}

		public static Dictionary<TProp, List<TSource>> GroupByProperty<TSource, TProp>(this IEnumerable<TSource> source, Func<TSource, TProp> property)
		{
			return source.GroupBy(property).ToDictionary(g => g.Key, g => g.ToList());
		}

		public static List<T> DbFree<T>(this IEnumerable<T> source)
		{
			return new List<T>(source);
		}

		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey key, out TValue value)
		{
			key = source.Key;
			value = source.Value;
		}

		public static int FirstIndexOf<T>(this IEnumerable<T> source, Predicate<T> condition)
		{
			for (int i = 0; i < source.Count(); i++)
				if (condition(source.ElementAt(i)))
					return i;

			return -1;
		}

		public static IEnumerable<T> RemoveLeading<T>(this IEnumerable<T> source, int length)
		{
			return RemoveTrailing(source.Reverse(), length).Reverse();
		}

		public static IEnumerable<T> RemoveTrailing<T>(this IEnumerable<T> source, int length)
		{
			int count = source.Count();
			return source.Take(count - length);
		}

		public static IEnumerable<T> RemoveLeading<T>(this IEnumerable<T> source, Predicate<T> pred)
		{
			int i = source.FirstIndexOf(el => !pred(el));
			if (i == -1) i = 0;
			for (; i < source.Count(); i++)
				yield return source.ElementAt(i);
		}

		public static IEnumerable<T> RemoveTrailing<T>(this IEnumerable<T> source, Predicate<T> pred)
		{
			return RemoveLeading(source.Reverse(), pred).Reverse();
		}

		public static IEnumerable<T> MergeCollections<T>(this IEnumerable<IEnumerable<T>> source)
		{
			foreach (IEnumerable<T> collection in source)
			{
				foreach (T element in collection)
					yield return element;
			}
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			HashSet<T> set = new HashSet<T>();
			foreach (T element in source) set.Add(element);
			return set;
		}
	}
}
