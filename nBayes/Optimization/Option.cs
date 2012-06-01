using System;
using System.Threading;

namespace nBayes.Optimization
{
	public class Option
	{
		private int attempts = 1;
		private int successes = 1;
		
		public Option()
		{
		}
		
		public float Value
		{
			get { return (((float)Successes / (float)this.Attempts)); }
		}
		
		public int Attempts {get { return this.attempts; }}
		public int Successes {get { return this.successes; }}
		
		public void IncrementAttempt()
		{
			Interlocked.Increment(ref attempts);
		}
		
		public void IncrementSuccesses()
		{
			Interlocked.Increment(ref successes);
		}
		
		public override string ToString ()
		{
			return string.Format ("{0} / {1} = {2}",
			                      this.successes,
			                      this.attempts,
			                      this.Value);
		}
		
		public static Option Named(string value)
		{
			return new NamedOption(value);
		}
		
		private class NamedOption : Option
		{
			public NamedOption(string value)
			{
				this.Name = value;
			}
			
			public string Name {get; private set;}
			
			public override string ToString ()
			{
				return string.Format ("{0}: {1}", Name, base.ToString());
			}
		}
	}
}

