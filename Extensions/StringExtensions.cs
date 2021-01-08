using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alessio.Base.Extensions
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

	public static class StringExtensions
	{
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
			foreach (char c in s)
			{
				if (char.IsLower(c)) result += char.ToUpper(c);
				else if (char.IsUpper(c)) result += char.ToLower(c);
				else result += c;
			}
			return result;
		}

		public static string ToSlugCase(this string s)
		{
			List<string> parts = new List<string>();
			string lastPart = "";

			foreach (char c in s)
			{
				if (char.IsUpper(c))
				{
					if (lastPart == "" && parts.Count == 0) // If the first letter is uppercase, it should be treated as if it was lowercase
						lastPart += c;
					else
					{
						parts.Add(lastPart.ToLower());
						lastPart = c.ToString();
					}
				}
				else
					lastPart += c;
			}

			return string.Join("_", parts);
		}

		public static string ToCommonCaseWithSpaces(this string s)
		{
			List<string> words = new List<string>();
			string lastWord = "";

			foreach (char c in s)
			{
				if (char.IsUpper(c) && lastWord != "")
				{
					words.Add(lastWord);
					lastWord = c.ToString();
				}
				else lastWord += c;
			}

			if (lastWord != "")
				words.Add(lastWord);

			return string.Join(" ", words).Capitalize();
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
	}
}
