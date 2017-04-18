nBayes (based on Paul Graham's spam filter) makes it easy to introduce statistics based decision making into your application. Whether it's spam filtering, or something else like artificial intelligence learning ... this tool can do it. The project is written in C#

You can also check out my blog to see what I'm up to: [http://codecube.net](http://codecube.net)

## Introduction

nBayes is a simple implementation of the naive bayesian spam filter described by Paul Graham in his essay "_A Plan for Spam_" ([http://www.paulgraham.com/spam.html](http://www.paulgraham.com/spam.html)).  The API is very simple, there are just 3 classes that you need to be familiar with.

* [Index](https://github.com/joelmartinez/nBayes/wiki/Index)
* [Entry](https://github.com/joelmartinez/nBayes/wiki/Entry)
* [Analyzer](https://github.com/joelmartinez/nBayes/wiki/Analyzer)

You can train the Index by adding entries to it, and then use an Analyzer to categorize a new entry as belonging to one index or another.  In the spam filtering example, one index would be the Spam, while the other would be the "not-Spam".

## Sample Code

```CSharp
    Index spam = Index.CreateMemoryIndex();
    Index notspam = Index.CreateMemoryIndex();
    
    // train the indexes
    spam.Add(Entry.FromString("want some viagra?"));
    spam.Add(Entry.FromString("cialis can make you larger"));
    notspam.Add(Entry.FromString("Hello, how are you?"));
    notspam.Add(Entry.FromString("Did you go to the park today?"));
    
    Analyzer analyzer = new Analyzer();
    CategorizationResult result = analyzer.Categorize(
         Entry.FromString("cialis viagra"), 
         spam, 
         notspam);
    
    switch (result)
    {
        case CategorizationResult.First:
            Console.WriteLine("Spam");
            break;
        case CategorizationResult.Undetermined:
            Console.WriteLine("Undecided");
            break;
        case CategorizationResult.Second:
            Console.WriteLine("Not Spam");
            break;
    }
```

The example above uses an extremely small index of words ... however, the reported result is indeed that it categorizes it as spam.  Larger indexes are required to get better results.  The sample project provided in the source code shows how to create two indexes by doing a search of twitter for two different terms.  The top 100 results of that twitter API query will be trained into each respective index, and then it will ask you to type in a sample phrase.  This phrase will be categorized into one of each index.
