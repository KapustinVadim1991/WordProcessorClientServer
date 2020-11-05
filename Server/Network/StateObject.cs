using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessor.Network
{
    // Объект сотстояния для асинхронного чтения данных отправляемых Клиентом
    class StateObject
    {
        public StateObject(Socket client)
        {
            clientSocket = client;
        }

        public const int BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder recievedData = new StringBuilder();

        public Socket clientSocket = null;
    }
}
