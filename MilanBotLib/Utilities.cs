using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MilanBotLib
{
    public static class StringExtensions
    {
        private static readonly Regex _whitespaceRegex = new Regex(@"\s{2,}|[\r\n]+");

        /// <summary>
        /// Collapses all whitespace in a string. This reduces all consecutive whitesapce characters to a single space,
        /// and removes all newline characters from the string.
        /// </summary>
        /// <param name="toCollapse">The string to collapse</param>
        /// <returns>The given string with all whitespace and newline characters collapsed to a single space</returns>
        public static string CollapseWhitespace(this string toCollapse)
        {
            return _whitespaceRegex.Replace(toCollapse, " ");
        }
    }
}
