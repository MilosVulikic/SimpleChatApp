/**************************************************
 *              Simple server - Client
 * This project is made in process of learning about threads and socket implementation.
 * Here are the steps I have taken in the process of creating this app.
 * 
 * Steps I took:
 * 1) Implementing the initial part of Server
 * 2) Creating Client
 * 3) Connecting the Client
 * 4) Network stream
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




            string messageToSend;
            while (!string.IsNullOrEmpty((messageToSend = Console.ReadLine())))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(messageToSend);
                networkStream.Write(buffer, 0, buffer.Length);
            }
            networkStream.Close();
            client.Close();
            Console.WriteLine("Disconnecting from server...");
            Console.ReadKey();
        }


















      
    }
}
