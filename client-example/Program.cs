using System;
using System.Threading;

using System.Net;
using clientexample;

namespace clientexample
{

	public delegate void EventLoop();

	class MainClass
	{

		static string[] messages = {
			"I am the first"
//			"I am second",
//			"heres a third",
//			"and a forth"
		};

		static int counter = 0;

		static SocketClientExample client;
		public static ManualResetEvent eventLoop = new ManualResetEvent(false);


		public static void finishedAsync(){
			eventLoop.Set ();
		}

		static void recievedMessage(string message){
			Console.WriteLine (message);
		}

		static void nextMessage(){
			if(counter >= messages.Length) throw new Exception("your done");
			try{
				Console.WriteLine("sending {0}", messages[counter] );
				client.write (messages [counter]);
			}catch(Exception e){
				Console.WriteLine (e);
			}
			counter++;
		}

		public static EventLoop listeners;

		public static void Main (string[] args)
		{
			Console.WriteLine("\nPress ENTER to continue...");
			Console.Read();

			IPHostEntry ipHostInfo = Dns.Resolve (Dns.GetHostName ());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress,11000);
			try{
				listeners += new EventLoop(nextMessage);
				client = new SocketClientExample (remoteEP);
				client.connect();
				client.listeners += new MessageListener(recievedMessage);
				Console.WriteLine("Listener Length: {0}", listeners.GetInvocationList().Length.ToString());
				while (listeners.GetInvocationList().Length > 0){
					listeners();
				}
				Console.WriteLine("Finished Exec");
			}catch(Exception e){
				Console.WriteLine (e.ToString());
			}
		}
	}
}
