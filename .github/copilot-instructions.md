# nBayes - Naive Bayesian Classifier

nBayes is a C# .NET 8.0 library implementing Paul Graham's naive Bayesian spam filter for statistical decision making and text classification. The library provides simple APIs for training indexes and categorizing text entries.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Quick Start - Build, Test, and Validate
- **Bootstrap the repository:**
  - `dotnet restore` -- takes ~2-10 seconds (first time slower). Set timeout to 30+ seconds.
  - `dotnet build` -- takes ~5-10 seconds. Set timeout to 30+ seconds.
  - `dotnet test` -- takes ~4 seconds. Set timeout to 15+ seconds.

### Core Development Commands
- **Restore dependencies:** `dotnet restore`
- **Build the solution:** `dotnet build` 
- **Run all tests:** `dotnet test`
- **Build specific project:** `dotnet build nBayes/nBayes.csproj`
- **Run sample (has limitations - see below):** `dotnet run --project nBayes.Sample`

### Key Projects Structure
- **nBayes/** - Core library with Bayesian classifier implementation
- **nBayes.Sample/** - Sample console application (has Twitter API dependency issues)
- **nBayes.test/** - NUnit test suite (2 tests, both pass reliably)

## Validation

### Always Run These Tests After Changes
1. **Build validation:** `dotnet build` - must complete without errors
2. **Unit tests:** `dotnet test` - both tests must pass
3. **Core functionality validation:** Create a simple test program to verify classification works:

```csharp
// Create simple test to validate core functionality
using System;
using nBayes;

class Program 
{
    static void Main()
    {
        var spam = nBayes.Index.CreateMemoryIndex();
        var notSpam = nBayes.Index.CreateMemoryIndex();
        
        spam.Add(Entry.FromString("want some viagra?"));
        notSpam.Add(Entry.FromString("Hello, how are you?"));
        
        var analyzer = new Analyzer();
        var result = analyzer.Categorize(Entry.FromString("viagra"), spam, notSpam);
        
        Console.WriteLine($"Classification result: {result}");
        // Should output: Classification result: First
    }
}
```

### CRITICAL Testing Scenarios
- **ALWAYS test classification accuracy** by training with known spam/not-spam examples and verifying correct categorization
- **Test with MemoryIndex and FileIndex** to ensure both storage types work
- **Verify Entry.FromString()** correctly tokenizes text input
- **Test Analyzer.Categorize()** with various input combinations

#### Complete Functionality Test
```csharp
// Test both MemoryIndex and FileIndex
var memIndex = nBayes.Index.CreateMemoryIndex();
var fileIndex = new FileIndex("/tmp/test.xml");
fileIndex.Open();

// Both should support Add/GetTokenCount operations
memIndex.Add(Entry.FromString("memory test"));
fileIndex.Add(Entry.FromString("file test"));

Console.WriteLine($"Memory index entries: {memIndex.EntryCount}");
Console.WriteLine($"File index entries: {fileIndex.EntryCount}");

fileIndex.Save(); // FileIndex requires explicit save

// Test Entry tokenization (requires System.Linq)
using System.Linq;
var entry = Entry.FromString("hello world test");
var tokens = entry.ToList(); // Entry implements IEnumerable<string>
Console.WriteLine($"Tokens: {string.Join(", ", tokens)}");
```

## Common Issues and Limitations

### Sample Application Issues
- **DO NOT rely on nBayes.Sample for validation** - it has external Twitter API dependencies that no longer work
- The sample app will hang when trying to connect to `http://search.twitter.com/search.atom` (deprecated endpoint)
- Use manual testing or unit tests instead of the sample app to validate changes

### Build Warnings
- Expect 1 warning about obsolete WebRequest in nBayes.Sample (SYSLIB0014) - this is known and acceptable
- The warning does not affect core library functionality

### Namespace Conflicts
- When using `Index` class, specify `nBayes.Index` to avoid conflict with `System.Index` in .NET
- When testing `Entry` tokenization, add `using System.Linq;` to use `.ToList()` on Entry objects

## Development Workflow

### For Library Changes
1. Make changes to files in `nBayes/` directory
2. Run `dotnet build` to verify compilation
3. Run `dotnet test` to ensure tests pass  
4. Create manual test program to validate new functionality
5. Update unit tests in `nBayes.test/` if adding new features

### For Adding Tests
- Tests use NUnit framework
- Add new test methods to `nBayes.test/TestnBayes.cs`
- Follow existing test patterns with `[Test]` attribute and `Assert.That()` assertions

## Timing Expectations
- **NEVER CANCEL** any build operation - they complete quickly
- **dotnet restore:** ~2-10 seconds (first time slower, subsequent runs ~2s)
- **dotnet build:** ~5-10 seconds (clean builds take longer)  
- **dotnet test:** ~4 seconds
- All operations are very fast, use standard timeouts (30s for builds, 15s for tests)

## Common Tasks

### Repository Root Structure
```
/home/runner/work/nBayes/nBayes/
├── .git/
├── .gitignore
├── Bayesian.sln          # Visual Studio solution file
├── LICENSE.md
├── README.md
├── nBayes/              # Core library project
│   ├── Analyzer.cs      # Main classification engine
│   ├── Entry.cs         # Text entry representation
│   ├── Index.cs         # Abstract base for indexes
│   ├── MemoryIndex.cs   # In-memory storage
│   ├── FileIndex.cs     # File-based storage
│   └── nBayes.csproj
├── nBayes.Sample/       # Sample app (has external dependencies)
│   ├── Program.cs
│   └── nBayes.Sample.csproj
└── nBayes.test/         # Unit tests
    ├── TestnBayes.cs
    └── nBayes.test.csproj
```

### Key Classes and Usage
- **Index:** Abstract base class, use `Index.CreateMemoryIndex()` for in-memory storage
- **Entry:** Text representation, create with `Entry.FromString("text to classify")`
- **Analyzer:** Classification engine, use `Categorize(entry, index1, index2)` method
- **CategorizationResult:** Result enum with values `First`, `Second`, or `Undetermined`

### Sample Working Code Pattern
```csharp
// Always use this pattern for basic classification
var categoryA = nBayes.Index.CreateMemoryIndex();
var categoryB = nBayes.Index.CreateMemoryIndex();

// Train with multiple examples
categoryA.Add(Entry.FromString("training text for category A"));
categoryB.Add(Entry.FromString("training text for category B"));

// Classify new text
var analyzer = new Analyzer();
var result = analyzer.Categorize(Entry.FromString("text to classify"), categoryA, categoryB);
```

## Dependencies and Requirements
- **.NET 8.0 SDK** (verified working with 8.0.118)
- **NUnit 4.0.1** (for testing)
- **System.ServiceModel.Syndication 8.0.0** (used by sample app only)
- **No external services required** for core library functionality