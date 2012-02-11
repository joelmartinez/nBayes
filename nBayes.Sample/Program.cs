using System;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;

namespace nBayes
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create indexes
            Console.Write("First Index: ");
            string firstSearch = Console.ReadLine();
            var first = CreateIndex(firstSearch);

            Console.Write("Second Index: ");
            string secondSearch = Console.ReadLine();
            var second = CreateIndex(secondSearch);

            Analyzer analyzer = new Analyzer();

            Console.Write("Text to Categorize: ");

            CategorizationResult result = analyzer.Categorize(Entry.FromString(Console.ReadLine()), first, second);

            Console.Write("And the verdict is ... ");

            switch (result)
            {
                case CategorizationResult.First:
                    Console.WriteLine(firstSearch);
                    break;
                case CategorizationResult.Undetermined:
                    Console.WriteLine("no clue");
                    break;
                case CategorizationResult.Second:
                    Console.WriteLine(secondSearch);
                    break;
            }

            first.Save();
            second.Save();
        }

        private static FileIndex CreateIndex(string input)
        {
            var scrubbed = HttpUtility.UrlEncode(input);
            var reader = XmlReader.Create(string.Format("http://search.twitter.com/search.atom?rpp=100&lang=en&q={0}", scrubbed));
            var feed = SyndicationFeed.Load(reader);

            var index = new FileIndex(input + ".xml");
            index.Open();

            foreach (SyndicationItem item in feed.Items)
            {
                index.Add(Entry.FromString(item.Title.Text));
            }

            return index;
        }
    }
}
