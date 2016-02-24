using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nBayes
{
	public class BufferedEnumerable<T> : IEnumerable<T>
	{
		private readonly IEnumerable<T> _wrapped;
		
		public BufferedEnumerable (IEnumerable<T> value)
		{
			if (value == null) throw new ArgumentNullException("value");
			
			_wrapped = value;
		}
		
		public IEnumerator GetEnumerator()
		{
			return new BufferedEnumerator(_wrapped.GetEnumerator());
		}
		
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new BufferedEnumerator(_wrapped.GetEnumerator());
		}
		
		private sealed class BufferedEnumerator : IEnumerator<T>
		{			
			private readonly IEnumerator<T> _wrappedEnumerator;
			private T _current;
			private Task<bool> _next;
			
			public BufferedEnumerator(IEnumerator<T> value)
			{
				_wrappedEnumerator = value;
			}
			
			public bool MoveNext ()
			{
				if (_next == null)
				{
					// first iteration
					_next = Task.Factory.StartNew(() => _wrappedEnumerator.MoveNext());
				}
				
				// take the pending result of the last buffered iteration
				bool returnValue = _next.Result;
				
				if (returnValue)
				{
					// grab the current value
					_current = _wrappedEnumerator.Current;
					
					// and asynchronously start the next one
					_next = Task.Factory.StartNew(() => _wrappedEnumerator.MoveNext());
				}
				else
				{
					// the enumerable is done, empty out the current result
					_current = default(T);
				}
				
				return returnValue;
			}

			public void Reset ()
			{
				if (_next != null) _next.Wait();
				
				_wrappedEnumerator.Reset();
			}

			public object Current {
				get {
					return _current;
				}
			}
			
			T IEnumerator<T>.Current {
				get {
					return _current;
				}
			}

			public void Dispose ()
			{
				if (_next != null) _next.Wait();
				
				_wrappedEnumerator.Dispose();
			}
		}
	}
}

