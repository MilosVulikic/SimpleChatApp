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
 * 4 Provide the method that handles chat
 * 5 Provide the method that handles client
 *  
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

namespace SimpleChatApp
{
    class Server
    {
        static void Main(string[] args)
        {            
            var port = 8888;
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);

            // Starting server
            try
            {
                ServerSocket.Start();
                Console.WriteLine("Server started and listening for clients...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            while (true)
            {
                try
                {
                    TcpClient client = ServerSocket.AcceptTcpClient();
                    Console.WriteLine("Client connected!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                              

            }


        }
    }
}
