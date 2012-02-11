namespace nBayes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public abstract class Entry : IEnumerable<string>
    {
        public Entry()
        {
        }

        public static Entry FromString(string content)
        {
            return new StringEntry(content);
        }

        public abstract IEnumerator<string> GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class StringEntry : Entry
        {
            private IEnumerable<string> tokens;

            public StringEntry(string stringcontent)
            {
                tokens = Parse(stringcontent);
            }

            public override IEnumerator<string> GetEnumerator()
            {
                return tokens.GetEnumerator();
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
