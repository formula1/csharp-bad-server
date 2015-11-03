using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;

namespace clientexample
{

	public delegate void MessageListener(string message);

	public class MessageReader
	{
		public static int BUFFER_SIZE = 256;
		public static string SEPERATING_CHARS = @"<EOF>";

		// Client socket.
		public Socket client = null;
		// Size of receive buffer.
		// Receive buffer.
		public byte[] byte_buffer = new byte[MessageReader.BUFFER_SIZE];
		// Received data string.
		public StringBuilder sb = new StringBuilder();
		private string bufferedData;
		ManualResetEvent readWaiter;


		public MessageReader (Socket socket){
			client = socket;
			MainClass.listeners += new EventLoop(listenForData);
		}

		// this is ugly
		public void listenForData(){
			client.BeginReceive(
				byte_buffer, 0, MessageReader.BUFFER_SIZE, 0, 
				new AsyncCallback (MessageReader.receiveCallback), this
			);
			readWaiter = new ManualResetEvent (false);
			readWaiter.WaitOne ();
//			MainClass.eventLoop.WaitOne();
		}

		private static void receiveCallback( IAsyncResult ar ) {
			MessageReader reader = (MessageReader)ar.AsyncState;

			// Read data from the remote device.
			int bytesRead = reader.client.EndReceive(ar);

			if (bytesRead > 0) {
				reader.parseMessages (Encoding.ASCII.GetString(reader.byte_buffer,0,bytesRead));
			}
			reader.readWaiter.Set ();
		}

		public MessageListener listeners;

		private void parseMessages(string response){
			response = bufferedData + response;
			string[] parsedMessages = Regex.Split(response, MessageReader.SEPERATING_CHARS);
			int l = parsedMessages.Length - 1;
			for (int i = 0; i < l; i++) {
				listeners (parsedMessages [i]);
			}
			bufferedData = parsedMessages [l];
		}
	}
}

