using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPServer
{
    public class ServerClient : IDisposable
    {
        private readonly TcpClient client;
        private NetworkStream stream;
        private byte[] bytes;

        public bool Connected { get => client.Connected; }

        public ServerClient()
        {
            this.client = new TcpClient();
            bytes = new byte[16];
        }

        public void Connect(IPAddress iPAddress, int port)
        {
            client.Connect(iPAddress, port);
            stream = client.GetStream();
        }

        public async Task ConnectAsync(IPAddress iPAddress, int port)
        {
            await client.ConnectAsync(iPAddress, port);
            stream = client.GetStream();
        }

        public void Close()
        {
            Dispose();
        }

        public async ValueTask WriteAsync(string message)
        {            
            await stream.WriteAsync(Encoding.ASCII.GetBytes(message + "\n"));
        }

        public async IAsyncEnumerable<string> ReadAsync()
        {
            do
            {
                var bytesReceived = await stream.ReadAsync(bytes);
                var stringReceived = Encoding.ASCII.GetString(bytes, 0, bytesReceived);

                yield return stringReceived;
            }
            while (client.Available > 0);
        }

        public void Dispose()
        {
            stream?.Dispose();
            if (client.Connected)
                client.Close();
        }
    }
}