using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nBayes
{
	public class BufferedEnumerable<T> : IEnumerable<T>
	{
		private IEnumerable<T> wrapped;
		
		public BufferedEnumerable (IEnumerable<T> value)
		{
			if (value == null) throw new ArgumentNullException("value");
			
			this.wrapped = value;
		}
		
		public IEnumerator GetEnumerator()
		{
			return new BufferedEnumerator(this.wrapped.GetEnumerator());
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new BufferedEnumerator(this.wrapped.GetEnumerator());
		}
		
		private sealed class BufferedEnumerator : IEnumerator<T>
		{			
			private IEnumerator<T> wrappedEnumerator;
			private T current;
			private Task<bool> next;
			
			public BufferedEnumerator(IEnumerator<T> value)
			{
				this.wrappedEnumerator = value;
			}
			
			public bool MoveNext ()
			{
				if (next == null)
				{
					// first iteration
					next = Task.Factory.StartNew(() => this.wrappedEnumerator.MoveNext());
				}
				
				// take the pending result of the last buffered iteration
				bool returnValue = next.Result;
				
				if (returnValue)
				{
					// grab the current value
					this.current = this.wrappedEnumerator.Current;
					
					// and asynchronously start the next one
					next = Task.Factory.StartNew(() => this.wrappedEnumerator.MoveNext());
				}
				else
				{
					// the enumerable is done, empty out the current result
					this.current = default(T);
				}
				
				return returnValue;
			}

			public void Reset ()
			{
				if (next != null) next.Wait();
				
				this.wrappedEnumerator.Reset();
			}

			public object Current {
				get {
					return this.current;
				}
			}
			
			T IEnumerator<T>.Current {
				get {
					return this.current;
				}
			}

			public void Dispose ()
			{
				if (next != null) next.Wait();
				
				this.wrappedEnumerator.Dispose();
			}
		}
	}
}

