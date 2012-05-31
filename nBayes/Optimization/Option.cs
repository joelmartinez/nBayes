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
			get { return ((float)this.Attempts) / ((float)Successes); }
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
	}
}

