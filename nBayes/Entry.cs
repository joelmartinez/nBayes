using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace nBayes
{
    public abstract class Entry : IEnumerable<string>
    {
        public static Entry FromString(string content)
        {
            return new StringEntry(content);
        }

        public abstract IEnumerator<string> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class StringEntry : Entry
        {
            private readonly IEnumerable<string> _tokens;

            public StringEntry(string stringcontent)
            {
                _tokens = Parse(stringcontent);
            }

            public override IEnumerator<string> GetEnumerator()
            {
                return _tokens.GetEnumerator();
            }

            /// <summary>
            /// Tokenizes a string
            /// </summary>
            private static IEnumerable<string> Parse(string source)
            {
                string clean = CleanInput(source);
                string[] tokens = clean.Split(' ');
                return tokens
                    .Where(t => !t.Equals(" ", StringComparison.InvariantCultureIgnoreCase))
                    .Select(t => t.ToLowerInvariant())
                    .Distinct();
            }

            /// <summary>
            /// Replace invalid characters with spaces.
            /// </summary>
            private static string CleanInput(string strIn)
            {
                return Regex.Replace(strIn, @"[^\w\'@-]", " ");
            }
        }

    }
}
