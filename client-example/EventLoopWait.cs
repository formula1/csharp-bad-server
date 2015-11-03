using System;

namespace clientexample
{
	public class EventLoopWait : Exception
	{
		public EventLoopWait(string message) : base(message){
		}
	}
}

