/**************************************************
 *               Simple server - Server
 *               
 * This project is made in process of learning about threads and socket implementation.
 * Here are the steps I have taken in the process of creating this app.
 * 
 * Steps I took:
 * 1 Create Listener
 * 2 Start the Listener
 * 3 While the Listener is up
 *  - Declare client 
 *  - Accept client 
 *  - (Client app creating in separate Console project and implementing client connect)
 *  - Add the Chat thread and start of the the thread
 * 4 Provide the method that handles client
 *  - Network streaming - receiving data
 *  - (Client app - Network streaming - sending data)
 * 5 Provide the method that handles broadcasting
 * 6 Client Socket Shutdown 
 * ***********************************************
 * To be added: 
 *      Enable multiple client functionality
 *      Thread on server
 *      List of clients in dictionary or array
 *      Lock for the thread
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

namespace SimpleChatApp
{
    class Server
    {
        static void Main(string[] args)
        {
            int clientCount = 1;
            var port = 8888;
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);
            
            ServerSocket.Start();
            Console.WriteLine("Server started and listening for clients...");


            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                Console.WriteLine("Client connected!");
                HandleClients(client);
                clientCount++;
               
            }


        }



        public static void HandleClients(TcpClient obj)  
        {
            TcpClient client = obj; 

            while (true)
            {
  
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bufferNoOfBytes = stream.Read(buffer, 0,buffer.Length);
                if (bufferNoOfBytes == 0) break;
                string data = Encoding.ASCII.GetString(buffer, 0, bufferNoOfBytes);
                Broadcast(data, obj);
                Console.WriteLine(data);
                // Broadcast
               
            }
            client.Client.Shutdown(SocketShutdown.Both);
            Console.WriteLine("Client disconnected...");
            client.Close();
        }

        public static void Broadcast(string data, TcpClient obj)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            NetworkStream stream = obj.GetStream();
            stream.Write(buffer,0,buffer.Length);
        }
    }
}
