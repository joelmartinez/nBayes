using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using nBayes;

namespace nBayes.test
{
    [TestFixture]
    public class TestnBayes
    {
        [Test]
        public void TestBasicAnalyzerFunctionality()
        {
            // Create two memory indices
            var index1 = Index.CreateMemoryIndex();
            var index2 = Index.CreateMemoryIndex();
            
            // Add some sample entries
            index1.Add(Entry.FromString("good great fantastic wonderful"));
            index1.Add(Entry.FromString("excellent amazing perfect"));
            
            index2.Add(Entry.FromString("bad terrible awful horrible"));
            index2.Add(Entry.FromString("worst disgusting pathetic"));
            
            // Create analyzer
            var analyzer = new Analyzer();
            
            // Test categorization
            var testEntry = Entry.FromString("good wonderful");
            var result = analyzer.Categorize(testEntry, index1, index2);
            
            // Should categorize towards first index (positive words)
            Assert.That(result, Is.Not.EqualTo(CategorizationResult.Second));
        }
        
        [Test]
        public void TestEntryFromString()
        {
            var entry = Entry.FromString("hello world test");
            Assert.That(entry, Is.Not.Null);
            
            var tokens = entry.ToList();
            Assert.That(tokens, Contains.Item("hello"));
            Assert.That(tokens, Contains.Item("world"));
            Assert.That(tokens, Contains.Item("test"));
        }
        
        [Test]
        public void TestGetPredictionStateReset()
        {
            // Test that calling GetPrediction multiple times on the same analyzer
            // gives consistent results (the issue described in the bug report)
            
            // Create two memory indices
            var index1 = Index.CreateMemoryIndex();
            var index2 = Index.CreateMemoryIndex();
            
            // Add some sample entries
            index1.Add(Entry.FromString("good great fantastic wonderful"));
            index1.Add(Entry.FromString("excellent amazing perfect"));
            
            index2.Add(Entry.FromString("bad terrible awful horrible"));
            index2.Add(Entry.FromString("worst disgusting pathetic"));
            
            // Create analyzer
            var analyzer = new Analyzer();
            
            // Test the same entry multiple times - should get same result
            var testEntry = Entry.FromString("good wonderful");
            
            float firstPrediction = analyzer.GetPrediction(testEntry, index1, index2);
            float secondPrediction = analyzer.GetPrediction(testEntry, index1, index2);
            float thirdPrediction = analyzer.GetPrediction(testEntry, index1, index2);
            
            // All predictions should be identical
            Assert.That(secondPrediction, Is.EqualTo(firstPrediction), 
                "Second prediction should match first prediction");
            Assert.That(thirdPrediction, Is.EqualTo(firstPrediction), 
                "Third prediction should match first prediction");
        }
    }
}
