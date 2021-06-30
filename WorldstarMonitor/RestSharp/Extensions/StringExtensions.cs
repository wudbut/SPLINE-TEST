#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

#if SILVERLIGHT
using System.Windows.Browser;
#endif

#if WINDOWS_PHONE
#endif

#if FRAMEWORK || MONOTOUCH || MONODROID
using RestSharp.Contrib;
#endif


namespace RestSharp.Extensions
{
	public static class StringExtensions
	{
#if !PocketPC
		public static string UrlDecode(this string input)
		{
			return HttpUtility.UrlDecode(input);
		}
#endif

		/// <summary>
		/// Uses Uri.EscapeDataString() based on recommendations on MSDN
		/// http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
		/// </summary>
		public static string UrlEncode(this string input)
		{
			const int maxLength = 32766;
			if (input == null)
				throw new ArgumentNullException("input");

			if (input.Length <= maxLength)
				return Uri.EscapeDataString(input);

			StringBuilder sb = new StringBuilder(input.Length * 2);
			int index = 0;
			while (index < input.Length)
			{
				int length = Math.Min(input.Length - index, maxLength);
				string subString = input.Substring(index, length);
				sb.Append(Uri.EscapeDataString(subString));
				index += subString.Length;
			}

			return sb.ToString();
		}

#if !PocketPC
		public static string HtmlDecode(this string input)
		{
			return HttpUtility.HtmlDecode(input);
		}

		public static string HtmlEncode(this string input)
		{
			return HttpUtility.HtmlEncode(input);
		}
#endif

#if FRAMEWORK
		public static string HtmlAttributeEncode(this string input)
		{
			return HttpUtility.HtmlAttributeEncode(input);
		}
#endif

		/// <summary>
		/// Check that a string is not null or empty
		/// </summary>
		/// <param name="input">String to check</param>
		/// <returns>bool</returns>
		public static bool HasValue(this string input)
		{
			return !string.IsNullOrEmpty(input);
		}

		/// <summary>
		/// Remove underscores from a string
		/// </summary>
		/// <param name="input">String to process</param>
		/// <returns>string</returns>
		public static string RemoveUnderscoresAndDashes(this string input)
		{
			return input.Replace("_", "").Replace("-", ""); // avoiding regex
		}

		/// <summary>
		/// Parses most common JSON date formats
		/// </summary>
		/// <param name="input">JSON value to parse</param>
		/// <returns>DateTime</returns>
		public static DateTime ParseJsonDate(this string input, CultureInfo culture)
		{
			input = input.Replace("\n", "");
			input = input.Replace("\r", "");

			input = input.RemoveSurroundingQuotes();

			long? unix = null;
            try {
                unix = Int64.Parse(input);
            } catch (Exception) { };
			if (unix.HasValue)
			{
				var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				return epoch.AddSeconds(unix.Value);
			}

			if (input.Contains("/Date("))
			{
				return ExtractDate(input, @"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/", culture);
			}

			if (input.Contains("new Date("))
			{
				input = input.Replace(" ", "");
				// because all whitespace is removed, match against newDate( instead of new Date(
				return ExtractDate(input, @"newDate\((-?\d+)*\)", culture);
			}

			return ParseFormattedDate(input, culture);
		}

		/// <summary>
		/// Remove leading and trailing " from a string
		/// </summary>
		/// <param name="input">String to parse</param>
		/// <returns>String</returns>
		public static string RemoveSurroundingQuotes(this string input)
		{
			if (input.StartsWith("\"") && input.EndsWith("\""))
			{
				// remove leading/trailing quotes
				input = input.Substring(1, input.Length - 2);
			}
			return input;
		}

		private static DateTime ParseFormattedDate(string input, CultureInfo culture)
		{
			var formats = new[] {
				"u", 
				"s", 
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", 
				"yyyy-MM-ddTHH:mm:ssZ", 
				"yyyy-MM-dd HH:mm:ssZ", 
				"yyyy-MM-ddTHH:mm:ss", 
				"yyyy-MM-ddTHH:mm:sszzzzzz",
				"M/d/yyyy h:mm:ss tt" // default format for invariant culture
			};

#if PocketPC
			foreach (string format in formats) {
				try {
					return DateTime.ParseExact(input, format, culture);
				} catch (Exception) {
				}
			}
			try {
				return DateTime.Parse(input, culture);
			} catch (Exception) {
			}
#else
			DateTime date;
			if (DateTime.TryParseExact(input, formats, culture, DateTimeStyles.None, out date))
			{
				return date;
			}
			if (DateTime.TryParse(input, culture, DateTimeStyles.None, out date))
			{
				return date;
			}
#endif

			return default(DateTime);
		}

		private static DateTime ExtractDate(string input, string pattern, CultureInfo culture)
		{
			DateTime dt = DateTime.MinValue;
			var regex = new Regex(pattern);
			if (regex.IsMatch(input))
			{
				var matches = regex.Matches(input);
				var match = matches[0];
				var ms = Convert.ToInt64(match.Groups[1].Value);
				var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				dt = epoch.AddMilliseconds(ms);

				// adjust if time zone modifier present
				if (match.Groups.Count > 2 && !String.IsNullOrEmpty(match.Groups[3].Value))
				{
					var mod = DateTime.ParseExact(match.Groups[3].Value, "HHmm", culture);
					if (match.Groups[2].Value == "+")
					{
						dt = dt.Add(mod.TimeOfDay);
					}
					else
					{
						dt = dt.Subtract(mod.TimeOfDay);
					}
				}

			}
			return dt;
		}

		/// <summary>
		/// Checks a string to see if it matches a regex
		/// </summary>
		/// <param name="input">String to check</param>
		/// <param name="pattern">Pattern to match</param>
		/// <returns>bool</returns>
		public static bool Matches(this string input, string pattern)