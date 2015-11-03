using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace clientexample
{
	public class SocketClientExample
	{
		IPEndPoint endpoint;
		LinkedList<byte[]> bufferedWrites = new LinkedList<byte[]>();
		Socket currentConnection;
		private MessageReader reader;
		private ManualResetEvent sendWaiter;


		public MessageListener listeners {
			get {
				return reader.listeners;
			}
			set {
				reader.listeners = value;
			}
		}

		public SocketClientExample (IPEndPoint targetEndPoint)
		{
			endpoint = targetEndPoint;
		}

		// Should emit a throw
		public SocketClientExample connect(){
			if (currentConnection != null) {
				throw new Exception ("This socket is already connected");
			}
			// Create a TCP/IP  socket.
			currentConnection = new Socket(AddressFamily.InterNetwork, 
				SocketType.Stream, ProtocolType.Tcp );
			currentConnection.Connect(endpoint);
			reader = new MessageReader (currentConnection);
			while (bufferedWrites.Count > 0) {
				currentConnection.Send (bufferedWrites.First.Value);
				bufferedWrites.RemoveFirst ();
			}
			return this;
		}


		public bool write(string message_str){
			byte[] message = Encoding.ASCII.GetBytes (message_str + "<EOF>");
			if(currentConnection == null){
				bufferedWrites.AddLast(message);
				return false;
			}
			currentConnection.BeginSend(message, 0, message.Length, 0,
				new AsyncCallback(SocketClientExample.writeSent), this);
			sendWaiter = new ManualResetEvent (false);
			sendWaiter.WaitOne ();
			return true;
		}

		private static void writeSent(IAsyncResult ar){
			try {
				// Retrieve the socket from the state object.
				SocketClientExample example = (SocketClientExample) ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = example.currentConnection.EndSend(ar);
				Console.WriteLine("Sent {0} bytes to server.", bytesSent);

				// Signal that all bytes have been sent.
				example.sendWaiter.Set();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}
	}
}

