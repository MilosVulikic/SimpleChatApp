/**************************************************
 *              Simple server - Client
 * This project is made in process of learning about threads and socket implementation.
 * Here are the steps I have taken in the process of creating this app.
 * 
 * Steps I took:
 * 1) Implementing the initial part of Server
 * 2) Creating Client
 * 3) Connecting the Client
 * 4) Network stream Sending data
 * 5) (Server broadcast)
 * 6) Thread for receiving broadcasted data
 * 7) NetworkStream for receiving data
 * 8) Client Socket Shutdown
 * ***********************************************
 *  To be added: 
 *      Small corrections to meet the server changes
 *      
 * 
 * ***********************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SimpleChatAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 8888;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpClient client = new TcpClient();
            client.Connect(ip, port);
            Console.WriteLine("Conected to the server: {0} , port {1}", ip, port);
            NetworkStream networkStream = client.GetStream();
           
            Thread thread = new Thread(new ParameterizedThreadStart(ReceiveData));
            thread.Start(client);   

            string messageToSend;
            while (!string.IsNullOrEmpty((messageToSend = Console.ReadLine())))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(messageToSend);
                networkStream.Write(buffer, 0, buffer.Length);
            }

            client.Client.Shutdown(SocketShutdown.Send);            
            networkStream.Close();
            client.Close();
            Console.WriteLine("Disconnecting from server...");
            Console.ReadKey();
        }

        static void ReceiveData(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream networkStream = tcpClient.GetStream();
            byte[] receivedByteData = new byte[1024];
            int receivedDataSize;

            while ((receivedDataSize = networkStream.Read(receivedByteData,0,receivedByteData.Length)) >0)
            {
                Console.WriteLine(Encoding.ASCII.GetString(receivedByteData,0,receivedDataSize));
            }

        }



      
    }
}
