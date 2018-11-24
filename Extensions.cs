using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Alessio.Base
{
	#region Configuration enums
	public enum CapitalizationOptions
	{
		FirstOnly,
		FirstOnlyTransparent, // Don't change other letters, just capitalize the first one
		EachWord,
		Reverse,
		EachWordReverse
	}
	#endregion

	public static class Extensions
    {
		#region IEnumerable
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

		public static T ArgMin<T, TProp>(this IEnumerable<T> source, Func<T, TProp> selector) where TProp: IComparable
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
			foreach(IEnumerable<T> collection in source)
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
		#endregion

		#region Strings
		public static bool IsNullOrEmpty(this string s)
		{
			return s == null || s == "";
		}

		public static bool IsNotNullOrEmpty(this string s)
		{
			return !s.IsNullOrEmpty();
		}

		public static List<string> Tokenize(this string s, bool allowDigitsInWord = true)
		{
			if (s == null) return null;

			List<string> result = new List<string>();
			if (s == "") return result;

			string last = "";
			string others = "";

			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if ((char.IsLetterOrDigit(c) && allowDigitsInWord) || (char.IsLetter(c)))
				{
					if (others != "")
					{
						result.Add(others);
						others = "";
					}
					last += c;
				}
				else
				{
					if (others == "")
					{
						result.Add(last);
						last = "";
					}
					others += c;
				}
			}

			if (others == "") result.Add(last);
			if (others != "") result.Add(others);
			return result;
		}

		public static bool IsUppercase(this string s) => s.All(c => char.IsUpper(c));

		public static bool IsLowercase(this string s) => s.All(c => char.IsLower(c));

		public static bool IsCapitalized(this string s) => s.Length > 0 && char.IsUpper(s[0]) && (s.Length == 1 || char.IsLower(s[1]));

		public static string Capitalize(this string s, CapitalizationOptions options = CapitalizationOptions.FirstOnly)
		{
			if (s.Length == 0) return "";
			else
			{
				switch (options)
				{
					case CapitalizationOptions.FirstOnly:
						return char.ToUpper(s[0]) + s.Substring(1).ToLower();
					case CapitalizationOptions.EachWord:
						return s.Split(' ').Select(word => word.Capitalize(CapitalizationOptions.FirstOnly)).JoinStrings(" ");
					case CapitalizationOptions.Reverse:
						return char.ToLower(s[0]) + s.Substring(1).ToUpper();
					case CapitalizationOptions.EachWordReverse:
						return s.Split(' ').Select(word => word.Capitalize(CapitalizationOptions.Reverse)).JoinStrings(" ");
					default:
						return s;
				}
			}
		}

		public static string InvertCase(this string s)
		{
			string result = "";
			foreach(char c in s)
			{
				if (char.IsLower(c)) result += char.ToUpper(c);
				else if (char.IsUpper(c)) result += char.ToLower(c);
				else result += c;
			}
			return result;
		}

		public static string PadWithCharacter(this string s, char pad, int toLength, bool atEnd = false)
		{
			string result = s;
			while (result.Length < toLength)
			{
				if (atEnd) result += pad;
				else result = pad + result;
			}
			return result;
		}
		#endregion

		#region Reflection
		public static object NewOrDefault(this Type type)
		{
			ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
			if (ctor != null) return ctor.Invoke(new object[] { });
			else return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
		
		public static T Clone<T>(this T obj) where T: new()
		{
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, obj);
				return (T)bf.Deserialize(ms);
			}
		}

		//public static void CopyOver<T>(this T source, T recipient)
		//{
		//	foreach (PropertyInfo info in typeof(T).GetProperties(BindingFlags.Instance))
		//		info.SetValue(recipient, info.GetValue(source, null), null);
		//}
		#endregion

		#region Deconstructors
		public static (TKey, TValue) Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp)
		{
			return (kvp.Key, kvp.Value);
		}
        #endregion

        #region DateTime
        public static DateTime GetMonthBegin(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetMonthEnd(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        public static bool IsMonthEnd(this DateTime date)
        {
            return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
        }
        #endregion
    }
}
