﻿/**************************************************
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
 * 7 Added dictionary that containes clients
 * 8 Thread for client handling added
 * 9 Edited HandleClient method - use dictionary data for clients
 * 10 Edited Broadcast method - use dictionary data for clients
 * ***********************************************
 * To be added:     
 *      See if additional changes are needed for the threads and possible locks
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
using System.IO;

namespace SimpleChatApp
{
    class Server
    {
        static readonly Dictionary<int, TcpClient> clientTable = new Dictionary<int,TcpClient>();
        static readonly object _lock = new object();

        static void Main(string[] args)
        {
            int clientCount = 1;
            var port = 8888;                        
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);           
            ServerSocket.Start();
            Thread serverListeningMessageOnDisplayThread = new Thread(ServerListeningMessageOnDisplay);
            serverListeningMessageOnDisplayThread.Start();


            while (true)
            {
                try
                {
                    TcpClient client = ServerSocket.AcceptTcpClient();
                    lock (_lock) clientTable.Add(clientCount, client);
                    Log.Message("Client connected!");
                    Console.WriteLine("Client connected!");
                    Thread clientThread = new Thread(HandleClients);
                    clientThread.Start(clientCount);
                    clientCount++;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
              
            }


        }



        public static void HandleClients(object obj)
        {
            try
            {
                int clientID = (int)obj;
                TcpClient client;
                lock (_lock) client = clientTable[clientID];

                while (true)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        int bufferNoOfBytes = stream.Read(buffer, 0, buffer.Length);
                        if (bufferNoOfBytes == 0) break;
                        string data = Encoding.ASCII.GetString(buffer, 0, bufferNoOfBytes);
                        Broadcast(data);
                        Log.Message(data);
                        Console.WriteLine(data);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
                lock (_lock) clientTable.Remove(clientID);
                client.Client.Shutdown(SocketShutdown.Both);
                Log.Message("Client disconnected...");
                Console.WriteLine("Client disconnected...");
                client.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }


        public static void Broadcast(string data)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);
                lock (_lock)
                foreach (TcpClient client in clientTable.Values)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
 
        }


        static void ServerListeningMessageOnDisplay()   
        {
            int i = 0;
            string connectingInfo = "Server started and listening for clients";
            while (true)
            {
                if (i > 3) { i = 0; connectingInfo = "Server started and listening for clients"; Console.CursorVisible = false; Console.Write("\r{0}     ", connectingInfo); }
                else if (i == 0) { Console.Write("\r{0}     ", connectingInfo); }
                else if (i >= 1 && i <= 3) { Console.Write("\r{0}     ", connectingInfo += "."); }
                i++;
                Thread.Sleep(500);
                if (clientTable.Count != 0) { break; }// if client exist kill the thread
            }
        }

    }


}
