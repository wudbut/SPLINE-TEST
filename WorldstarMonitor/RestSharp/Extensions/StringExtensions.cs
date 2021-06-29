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

			inpu