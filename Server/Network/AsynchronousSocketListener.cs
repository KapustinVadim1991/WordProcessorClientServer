using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessor
{
    class AsynchronousSocketListener
    {
        private int portNumber;
        public AsynchronousSocketListener(int portNumber)
        {
            this.portNumber = portNumber;
        }

        public async void Listen()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAdress, portNumber);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipEndPoint);
                listener.Listen(23);

                while (true)
                {
                    Socket handler = listener.Accept();
                    string data = null;

                    var bytes = new byte[256];
                    int bytesRecieve = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRecieve);


                }
            }
        }
    }
}
