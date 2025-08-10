using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

using nBayes.Optimization;

namespace nBayes
{
    class Program
    {
        static void Main(string[] args)
        {
			TestBinaryClassifier();
			
			//TestOptimizer();
			
			//TestBufferedEnumerable();
        }
		
	private static void TestBinaryClassifier()
	{
	    // Create indexes from twitter using two user supplied terms
            Console.Write("First Index: ");
            string firstSearch = Console.ReadLine();
            var firsttask = CreateIndex(firstSearch);

            Console.Write("Second Index: ");
            string secondSearch = Console.ReadLine();
            var secondtask = CreateIndex(secondSearch);

            Analyzer analyzer = new Analyzer();

	    // let the user create a sample text entry that we will classify against the two indices
            Console.Write("Text to Categorize: ");
	    var entryToCategorize = Entry.FromString(Console.ReadLine());
			
	    var first = firsttask.Result;
	    var second = secondtask.Result;
			
            CategorizationResult result = analyzer.Categorize(entryToCategorize, first, second);

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

        private static Task<FileIndex> CreateIndex(string input)
        {
            var scrubbed = HttpUtility.UrlEncode(input);
            var task = WebHelper.Feed(string.Format("https://www.reddit.com/search.rss?q={0}&sort=new", scrubbed))
			.ContinueWith(r =>
			{
				var feed = r.Result;
		
		        	var index = new FileIndex(input + ".xml");
		        	index.Open();
		
		        	foreach (SyndicationItem item in feed.Items)
		        	{
		            		index.Add(Entry.FromString(item.Title.Text));
		        	}
		
		        	return index;
			});
			
			return task;
        	}
		
/// <summary>This sample program has 3 options, and will use the the optimizer
/// to run a test scenario. Two options are picked, and we will simulate user interest in one,
/// then user sentiment changing to favor the second option.</summary>
private static void TestOptimizer()
{
	var optimizer = new Optimizer();
	
	// define the available options
	Option[] options = new Option[] {
		Option.Named("Orange"),
		Option.Named("Green"),
		Option.Named("White")
	};
	optimizer.Add(options[0]);
	optimizer.Add(options[1]);
	optimizer.Add(options[2]);
	
	// pick two options, and define when user interest will change
	var firstWinner = options[2]; // white
	var secondWinner = options[1]; // green
	var switchRatio = .35f; // 35% of the way through the test set
	int tries = 100; // the test set
	
	for (int i = 0; i < tries; i++) {
		Option selected = optimizer.Choose().Result; // don't care about asyncrony in this particular example
		Console.WriteLine("Choosing: {0}", selected);
		
		// decide which chosen option 'the users' will prefer
		bool isFirstBatch = (float)tries * switchRatio > i;
		if (isFirstBatch && selected == firstWinner)
			firstWinner.IncrementSuccesses();
		else if (!isFirstBatch && selected == secondWinner)
			secondWinner.IncrementSuccesses();
	}
	
	Console.WriteLine("\nResults! We expect that ({0}) will have the highest success rate, and ({1}) will be in second place", secondWinner, firstWinner);
	Console.WriteLine("\nThis is the final result after {0} tries\n{1}", tries, optimizer);
}
		
	
		private static void TestBufferedEnumerable()
		{
			var numbers = new int[] {1,2,3,4,5,6};
			
			var buffered = new BufferedEnumerable<int>(numbers);
			
			Console.WriteLine("enumerating now");
			
			foreach(var item in buffered) Console.WriteLine(item);
		}

    }
	/// <summary>simple web helper to simplify the web request</summary>
	internal static class WebHelper
	{
		private static readonly HttpClient httpClient = new HttpClient();
		
		static WebHelper()
		{
			httpClient.DefaultRequestHeaders.Add("User-Agent", "nBayes sample client");
		}
		
		public static async Task<string> Get(string url) {
			try
			{
				var response = await httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Warning: Failed to retrieve content from {url}: {ex.Message}");
				return string.Empty;
			}
		}
		
		public static async Task<SyndicationFeed> Feed(string url) {
			var content = await Get(url);
			if (string.IsNullOrEmpty(content))
			{
				Console.WriteLine($"Warning: No content retrieved from {url}, creating empty feed for testing");
				// Create a minimal feed for testing when no internet access
				var emptyFeed = new SyndicationFeed("Test Feed", "Test feed for nBayes when no internet access", new Uri("http://localhost"));
				emptyFeed.Items = new List<SyndicationItem>
				{
					new SyndicationItem("Sample Entry 1", "This is sample content for testing", new Uri("http://localhost/1")),
					new SyndicationItem("Sample Entry 2", "This is more sample content for testing", new Uri("http://localhost/2"))
				};
				return emptyFeed;
			}
				
			TextReader txtreader = new StringReader(content);
			XmlReader reader = XmlReader.Create(txtreader);
			var feed = SyndicationFeed.Load(reader);
			return feed;
		}
		
		public static async Task<T> Json<T>(string url)
        {
			var content = await Get(url);
			if (string.IsNullOrEmpty(content))
				throw new InvalidOperationException($"Failed to retrieve content from {url}");
				
			try
			{
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                using (var stream = new MemoryStream(bytes))
                {
                    var deserialized = serializer.ReadObject(stream);
                    return (T)deserialized;
                }
			}
            catch (Exception exc) 
			{ 
				throw new InvalidOperationException($"Failed to deserialize JSON from {url}", exc);
			}
		}
	}
}


