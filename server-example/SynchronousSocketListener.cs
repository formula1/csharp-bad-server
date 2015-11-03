using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Newtonsoft.Json;

namespace ServerExample {
	public delegate void ConnectionListener(Socket handler);

	public class SynchronousSocketListener {

		// Incoming data from the client.
		IPEndPoint localEndPoint;

		public SynchronousSocketListener (string configJSON) :
		this(JsonConvert.DeserializeObject<ServerConfig>(configJSON))
		{}
		public SynchronousSocketListener (ServerConfig config){
			IPAddress ipAddress = IPAddress.Parse(config.ipAddress);
			this.Initialize(new IPEndPoint(ipAddress, config.port));
		}
		public SynchronousSocketListener(int port){
			IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];

			this.Initialize(new IPEndPoint(ipAddress, port));
		}
		public void Initialize(IPEndPoint providedEndpoint){
			this.localEndPoint = providedEndpoint;
		}

		public ConnectionListener listeners;


		public void listen(int maxPending) {
			// Data buffer for incoming data.
			byte[] bytes = new Byte[1024];
			// Create a TCP/IP socket.
			Socket listener = new Socket(
				AddressFamily.InterNetwork,
				SocketType.Stream,
				ProtocolType.Tcp
			);

			try{
				listener.Bind(this.localEndPoint);
				listener.Listen(maxPending);
			}catch(Exception e){
				Console.WriteLine(e.ToString());
			}

			while (true) {
				Console.WriteLine("Waiting for a connection...");
				// Program is suspended while waiting for an incoming connection.
				Socket handler = listener.Accept();
				this.listeners(handler);
				handler.Shutdown(SocketShutdown.Both);
				handler.Close();
			}
		}
	}
}