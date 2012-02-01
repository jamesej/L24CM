using System;
using System.Collections.Generic;
using System.Text;

namespace L24CM.Utility
{
    public static class StringX
    {
        public static string Left(this string s, int n)
        {
            return s.Substring(0, Math.Min(s.Length, n));
        }

        public static string Right(this string s, int n)
        {
            int len = Math.Min(s.Length, n);
            return s.Substring(s.Length - len, len);
        }

        public static string UpTo(this string s, string upTo)
        {
            return s.UpTo(upTo, StringComparison.CurrentCulture);
        }
        public static string UpTo(this string s, string upTo, StringComparison comparisonType)
        {
            int pos = s.IndexOf(upTo, comparisonType);
            if (pos == -1)
                return s;
            else
                return s.Substring(0, pos);
        }

        public static string UpToLast(this string s, string upTo)
        {
            return s.UpToLast(upTo, StringComparison.CurrentCulture);
        }
        public static string UpToLast(this string s, string upTo, StringComparison comparisonType)
        {
            int pos = s.LastIndexOf(upTo, comparisonType);
            if (pos == -1)
                return s;
            else
                return s.Substring(0, pos);
        }

        public static string UpToRequired(this string s, string upTo)
        {
            int pos = s.IndexOf(upTo);
            if (pos < 0)
                throw new FormatException("No '" + upTo + "' in string");
            else
                return s.Substring(0, pos);
        }

        public static string After(this string s, string after)
        {
            return s.After(after, StringComparison.CurrentCulture);
        }
        public static string After(this string s, string after, StringComparison comparisonType)
        {
            int pos = s.IndexOf(after, comparisonType);
            if (pos == -1)
                return "";
            else
                return s.Substring(pos + after.Length);
        }

        public static string AfterRequired(this string s, string after)
        {
            int pos = s.IndexOf(after);
            if (pos < 0)
                throw new FormatException("No '" + after + "' in string");
            else
                return s.Substring(pos + after.Length);
        }

        public static string LastAfter(this string s, string after)
        {
            int pos = s.LastIndexOf(after);
            if (pos == -1)
                return "";
            else
                return s.Substring(pos + after.Length);
        }

        public static string StandardCharsOnly(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || c == ' ')
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static string ToProper(this string s)
        {
            StringBuilder sb = new StringBuilder();
            bool isInitial = true;
            foreach (char c in s)
            {
                if (isInitial && char.IsLower(c))
                {
                    sb.Append(char.ToUpper(c));
                    isInitial = false;
                }
                else if (!isInitial && char.IsUpper(c))
                    sb.Append(char.ToLower(c));
                else
                {
                    sb.Append(c);
                    isInitial = char.IsWhiteSpace(c);
                }
            }
            return sb.ToString();
        }

        public static bool StartsWithAny(this string s, IEnumerable<string> starts, out string rem)
        {
            foreach (string start in starts)
                if (s.StartsWith(start))
                {
                    rem = s.After(start);
                    return true;
                }
            rem = null;
            return false;
        }

        public static string StripRegions(this string s, string delimStart, string delimEnd)
        {
            StringBuilder sb = new StringBuilder();
            int pos = 0, pos1 = 0;
            while (pos < s.Length)
            {
                pos1 = s.IndexOf(delimStart, pos, StringComparison.InvariantCultureIgnoreCase);
                if (pos1 == -1) pos1 = s.Length;
                sb.Append(s.Substring(pos, pos1 - pos));
                pos = pos1;
                if (pos1 < s.Length)
                {
                    pos1 = s.IndexOf("</sup>", pos, StringComparison.InvariantCultureIgnoreCase);
                    if (pos1 == -1)
                        pos = s.Length;
                    else
                        pos = pos1 + 6;
                }
            }

            return sb.ToString();
        }

        public static int? AsIntOrNull(this string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            else
                return null;
        }

        /// <summary>
        /// Gets the part of a string right of a position marker and left of the leftmost of a set of
        /// possible terminators, either including the terminator in the result or not, and
        /// as a side effect advancing the marker to just after the terminator
        /// </summary>
        /// <param name="s">string to scan</param>
        /// <param name="pos">position marker (set to -1 if no terminator match)</param>
        /// <param name="terminators">list of terminators</param>
        /// <param name="includeTerm">whether to include terminator in return</param>
        /// <returns>section before/including matched terminator, or whole string if no terminator found</returns>
        static public string GetHead(this string s, ref int pos, string[] terminators, bool includeTerm)
        {
            return s.GetHead(ref pos, terminators, includeTerm, true);
        }
        static public string GetHead(this string s, ref int pos, string[] terminators, bool includeTerm, bool skipTerm)
        {
            string tf;
            return s.GetHead(ref pos, terminators, includeTerm, skipTerm, out tf);
        }
        static public string GetHead(this string s, ref int pos, string terminator)
        {
            string tf;
            return s.GetHead(ref pos, new string[] {terminator}, false, true, out tf);
        }
        static public string GetHead(this string s, ref int pos, string[] terminators, out string tf)
        {
            return s.GetHead(ref pos, terminators, false, true, out tf);
        }
        static public string GetHead(this string s, ref int pos, string[] terminators, bool includeTerm, bool skipTerm, out string termFound)
        {
            return s.GetHead(ref pos, terminators, includeTerm, skipTerm, out termFound, StringComparison.InvariantCultureIgnoreCase);
        }
        static public string GetHead(this string s, ref int pos, string[] terminators, bool includeTerm, bool skipTerm, out string termFound, StringComparison comp)
        {
            termFound = null;
            if (pos < 0)
                return "";

            string res;

            int firstPos = int.MaxValue;
            int newPos = 0;
            int termLen = 0;
            int endPos;

            // Find leftmost match of a terminator (preferring first in list)
            foreach (string term in terminators)
            {
                newPos = s.IndexOf(term, pos, comp);
                if (newPos != -1 && newPos < firstPos)
                {
                    firstPos = newPos;
                    termLen = term.Length;
                    termFound = term;
                }
            }

            if (firstPos != int.MaxValue)
            {
                if (includeTerm)
                    endPos = firstPos + termLen;
                else
                    endPos = firstPos;
                res = s.Substring(pos, endPos - pos);
                pos = firstPos;
                if (skipTerm)
                    pos += termLen;
                if (pos >= s.Length) pos = -1;
                return res;
            }
            else
            {
                endPos = pos;
                pos = -1;
                return s.Substring(endPos);
            }
        }
    }
}
