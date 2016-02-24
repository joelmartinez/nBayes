using System.Threading;

namespace nBayes.Optimization
{
	public class Option
	{
		private int _attempts = 1;
		private int _successes = 1;

	    public float Value
		{
			get { return ((Successes / (float)Attempts)); }
		}
		
		public int Attempts {get { return _attempts; }}
		public int Successes {get { return _successes; }}
		
		public void IncrementAttempt()
		{
			Interlocked.Increment(ref _attempts);
		}
		
		public void IncrementSuccesses()
		{
			Interlocked.Increment(ref _successes);
		}
		
		public override string ToString ()
		{
			return string.Format ("{0} / {1} = {2}",
			                      _successes,
			                      _attempts,
			                      Value);
		}
		
		public static Option Named(string value)
		{
			return new NamedOption(value);
		}
		
		private class NamedOption : Option
		{
			public NamedOption(string value)
			{
				Name = value;
			}

		    private string Name {get; set;}
			
			public override string ToString ()
			{
				return string.Format ("{0}: {1}", Name, base.ToString());
			}
		}
	}
}

