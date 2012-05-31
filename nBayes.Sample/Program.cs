using System;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace nBayes
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create indexes
            Console.Write("First Index: ");
            string firstSearch = Console.ReadLine();
            var firsttask = CreateIndex(firstSearch);

            Console.Write("Second Index: ");
            string secondSearch = Console.ReadLine();
            var secondtask = CreateIndex(secondSearch);

            Analyzer analyzer = new Analyzer();

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
            var task = WebHelper.Feed(string.Format("http://search.twitter.com/search.atom?rpp=100&lang=en&q={0}", scrubbed))
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
    }

	/// <summary>simple web helper to simplify the web request</summary>
	internal static class WebHelper
	{
		public static Task<string> Get(string url) {
			var tcs = new TaskCompletionSource<string>();
            var request = (HttpWebRequest)WebRequest.Create(url);
			request.UserAgent = "nBayes sample client";
            try
            {
                request.BeginGetResponse(iar =>
                {
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.EndGetResponse(iar);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var sreader = new StreamReader(response.GetResponseStream());
                            var result = sreader.ReadToEnd();

							tcs.SetResult(result);
                        }
                        else
                        {
                            tcs.SetResult(string.Empty);
                        }
                    }
                    catch (Exception exc) { tcs.SetException(exc); }
                    finally { if (response != null) response.Close(); }
                }, null);
            }
            catch (Exception exc) { tcs.SetException(exc); }
            return tcs.Task;
		}
		
		public static Task<SyndicationFeed> Feed(string url) {
			var tcs = new TaskCompletionSource<SyndicationFeed>();
			Get (url).ContinueWith(result => 
			{
				TextReader txtreader = new StringReader(result.Result);
				XmlReader reader = XmlReader.Create(txtreader);
				var feed = SyndicationFeed.Load(reader);
				tcs.SetResult(feed);
			});
			return tcs.Task;
		}
		
		public static Task<T> Json<T>(string url)
        {
            var tcs = new TaskCompletionSource<T>();
			
			Get (url).ContinueWith(result =>
            {
				try
				{
	                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
	                byte[] bytes = Encoding.UTF8.GetBytes(result.Result);
	                using (var stream = new MemoryStream(bytes))
	                {
	                    var deserialized = serializer.ReadObject(stream);
	
	                    tcs.SetResult( (T)deserialized );
	                }
				}
                catch (Exception exc) { tcs.SetException(exc); }
			});
			
			return tcs.Task;
		}
	}
}


