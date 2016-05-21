using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Extention
{
	public static class ExtentionHelper
	{

	    public static bool IsBetweenDate(this DateTime thisDateTime, DateTime beginDateTime, DateTime endDateTime)
	    {
	        return thisDateTime <= endDateTime && thisDateTime >= endDateTime;
	    }
		public static bool IsValidUri(this string s)
		{
			return Uri.IsWellFormedUriString(s, UriKind.Absolute);
		}
		public const string MatchEmailPattern =
			@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
			+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
			+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
			+ @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

		public static StringBuilder ReplaceIgnoreCase(this StringBuilder s, string oldString, string newString)
		{
			var result = Regex.Replace(s.ToString(), oldString, newString, RegexOptions.IgnoreCase);
			return new StringBuilder(result);
		}

		public static string ReplaceIgnoreCase(this string s, string newString, string oldString)
		{
			var result = Regex.Replace(s, newString, oldString, RegexOptions.IgnoreCase);
			return result;
		}

		public static bool Like(this string s, string wildcard)
		{
			// Replace the * with an .* and the ? with a dot. Put ^ at the
			// beginning and a $ at the end
			var pattern = "^" + Regex.Escape(wildcard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";

			// Now, run the Regex as you already know
			var regex = new Regex(pattern, RegexOptions.IgnoreCase);

			return regex.IsMatch(s);
		}

		public static bool IsNumber(this string searchString)
		{
			return !string.IsNullOrEmpty(searchString) && searchString.All(char.IsDigit);
		}

		public static bool BetweenDate(this DateTime value, DateTime a, DateTime b)
		{
			return ((a <= value) && (value <= b)) || ((b <= value) && (value <= a));
		}

		public static bool IsNull(this string s)
		{
			var input = s != null ? s.Trim() : s;
			if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		///     Checks whether the given Email-Parameter is a valid E-Mail address.
		/// </summary>
		/// <param name="email">Parameter-string that contains an E-Mail address.</param>
		/// <returns>
		///     True, when Parameter-string is not null and
		///     contains a valid E-Mail address;
		///     otherwise false.
		/// </returns>
		public static bool IsEmail(this string email)
		{
			if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
			return false;
		}

        private bool IsNumber(string searchString)
        {
            return !string.IsNullOrEmpty(searchString) && searchString.All(char.IsDigit);
        }
    }
}