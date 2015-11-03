
using System;
using System.Net.Sockets;
using System.Text;
using ServerExample;


public class MainClass{

	public static string data = null;

	public static void onConnection(Socket connection)
	{
		byte[] bytes = new Byte[1024];
		data = null;

		// An incoming connection needs to be processed.
		while (true) {
			bytes = new byte[1024];
			int bytesRec = connection.Receive(bytes);
			data += Encoding.ASCII.GetString(bytes,0,bytesRec);
			if (data.IndexOf("<EOF>") > -1) {
				break;
			}
		}

		// Show the data on the console.
		Console.WriteLine( "Text received : {0}", data);

		// Echo the data back to the client.
		byte[] msg = Encoding.ASCII.GetBytes(data);

		connection.Send(msg);

	}

	public static void Main(){

		SynchronousSocketListener server = new SynchronousSocketListener(11000);

		server.listeners = new ConnectionListener(onConnection);

		server.listen(10);

		Console.WriteLine("\nPress ENTER to continue...");
		Console.Read();
	}
}