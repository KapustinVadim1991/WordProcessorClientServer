using System;
using System.Net.Sockets;
using System.Text;

namespace WordProcessorServer.Network
{
    class ClientObject
    {
        private TcpClient client;
        public ClientObject(TcpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Получение префикса отправленного клиентом и отправка соответствующих префиксу слов обратно клиенту
        /// </summary>
        public void DataExchangeWithClient()
        {
            NetworkStream stream = client.GetStream();
            try
            {
                if (stream.CanRead)
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    StringBuilder message = new StringBuilder();
                    int numberOfBytesRead = 0;

                    do
                    {
                        numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
                        message.Append(Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead));
                    } while (stream.DataAvailable);

                    string topFive = RequestsToDatabase.GetTopFive(message.ToString());
                    buffer = Encoding.UTF8.GetBytes(topFive);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

    }
}
