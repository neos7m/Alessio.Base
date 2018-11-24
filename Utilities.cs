using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Alessio.Base
{
	public static class ReflectUtilities
	{
		public static T NewOrDefault<T>()
		{
			return (T)typeof(T).NewOrDefault();
		}
	}

	public static class Utilities
	{
		private static decimal EvaluateMaths(string expr, List<decimal> substituted)
		{
			// Recursion base: if the expression is only a number or a replacement, let's evaluate it directly
			if (decimal.TryParse(expr, out decimal d)) return d;
			else
			{
				const string pattern = @"^%_(?<num>\d+)_%$";
				Match m = Regex.Match(expr, pattern);
				if (m.Success)
				{
					int index = int.Parse(m.Groups["num"].Value);
					return substituted[index];
				}
			}

			// Step 1: parentheses
			if (substituted == null)
				substituted = new List<decimal>();
			int open = expr.IndexOf('(');
			if (open != -1)
			{
				int level = 1;
				int i;
				for (i = open + 1; i < expr.Length && level > 0; i++)
				{
					char cur = expr[i];
					switch (cur)
					{
						case '(':
							level++;
							break;
						case ')':
							level--;
							break;
						default:
							break;
					}
				}
				if (level != 0) throw new InvalidOperationException($"Syntax error in expression \"{expr}\": parentheses mismatch");

				//3*(2+1)-5
				string content = expr.Substring(open + 1, i - open - 2); // Should be -1, but i gets incremented the last time as well
				decimal inside = EvaluateMaths(content, substituted);
				string replacement = $"%_{substituted.Count}_%";
				substituted.Add(inside);

				return EvaluateMaths(expr.Remove(open, i - open).Insert(open, replacement), substituted); // Again, i is one bigger than expected, so i - open without +1
			}


			// Step 2: groups
			string operators = "+-*/";
			string[] parts = expr.Split(operators.ToCharArray());
			List<char> foundOps = expr.Where(c => operators.Contains(c)).ToList();
			if (foundOps.Count != parts.Length - 1)
			{
				// Check that the first part isn't a negative number
				if (foundOps[0] == '-' && foundOps.Count == parts.Length)
				{
					// Replace
					string replacement = $"%_{substituted.Count}_%";
					substituted.Add(-EvaluateMaths(parts[0]));
					string corrected = replacement;
					for (int i = 0; i < foundOps.Count; i++)
						corrected += foundOps[i] + parts[i];
				}
				else throw new InvalidOperationException($"Syntax error in expression \"{expr}\": invalid maths");
			}

			// Step 3: evaluate every single group
			List<decimal> operands = parts.Select(p => EvaluateMaths(p, substituted)).ToList();

			// Step 4: * and / first
			while (foundOps.Any(o => o == '*' || o == '/'))
			{
				int op = foundOps.FindIndex(c => c == '*' || c == '/');
				decimal left = operands[op], right = operands[op + 1];
				decimal result = (foundOps[op] == '*' ? left * right : left / right);

				foundOps.RemoveAt(op);
				operands.RemoveAt(op + 1);
				operands[op] = result;
			}

			// Step 5: then all the rest
			while (foundOps.Count > 0)
			{
				decimal left = operands[0], right = operands[1];
				decimal result = 0.0m;
				switch (foundOps[0])
				{
					case '+':
						result = left + right;
						break;
					case '-':
						result = left - right;
						break;
					// It should not get past here, but you never know
					case '*':
						result = left * right;
						break;
					case '/':
						result = left / right;
						break;
				}

				foundOps.RemoveAt(0);
				operands.RemoveAt(1);
				operands[0] = result;
			}

			// Step 6: result is in operands[0]
			return operands[0];
		}

		public static decimal EvaluateMaths(string expr)
		{
			return EvaluateMaths(expr, null);
		}
	}
}
