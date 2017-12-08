﻿/**************************************************
 *              Simple Form Server
 *  
 **************************************************
 * 1) Server Stop to be implemented               
 * 2) Visual presentation of connected clients
 * 3) Show client name in presentation textbox
***************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SimpleChatFormServer
{
    
    public partial class FrmServer : Form
    {
        volatile bool started = false;
        int port;
        public volatile string messageArrived;
        static readonly Dictionary<int, TcpClient> clientTable = new Dictionary<int, TcpClient>();
        int clientCount = 1;

        public FrmServer()
        {
            InitializeComponent();
            txtServerIP.Text = "127.0.0.1";
            txtServerPort.Text = "8888";
            port = Convert.ToInt32(txtServerPort.Text);
        }

        private void btnServerStart_Click(object sender, EventArgs e)
        {
            started = !started;
            if (started)
            {
                TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);                
                Thread serverListenThread = new Thread(ListenForClients);
                serverListenThread.Start(ServerSocket);
                btnServerStart.Enabled = false;
                txtServerIP.Enabled = false;
                txtServerPort.Enabled = false;
                //btnServerStart.Text = "Stop";
            }
            else
            {
                //// Server stop to be implemented               
                // btnServerStart.Text = "Start";
            }
        }

        private void txtServerIP_TextChanged(object sender, EventArgs e)
        {
            ChangeStartButton();
        }

        

        private void txtServerPort_TextChanged(object sender, EventArgs e)
        {
            ChangeStartButton();
        }


        public void ListenForClients(object serverSocket)
        {
            TcpListener tcpListener = (TcpListener)serverSocket;
            tcpListener.Start();
            PrintReceivedData("Server started Listening...");
            while (started)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    clientTable.Add(clientCount, client);
                    Log.Message("Client connected!");
                    PrintReceivedData("Client connected!");                    
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



        public void HandleClients(object obj)
        {
            try
            {
                int clientID = (int)obj;
                TcpClient client;
                client = clientTable[clientID];

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
                        PrintReceivedData(data);                        
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
                clientTable.Remove(clientID);
                client.Client.Shutdown(SocketShutdown.Both);
                Log.Message("Client disconnected...");
                PrintReceivedData("Client disconnected...");                
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

        delegate void StringArgReturningVoidDelegate(string text);

        private void PrintReceivedData(string text)
            {
                // InvokeRequired required compares the thread ID of the  
                // calling thread to the thread ID of the creating thread.  
                // If these threads are different, it returns true.  
                if (txtMessageDisplay.InvokeRequired)
                {
                    StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(PrintReceivedData);
                    Invoke(d, new object[] { text });
                }
                else
                {
                    txtMessageDisplay.AppendText(text + Environment.NewLine);
                }
            }



            private void ChangeStartButton()
        {
            if (!string.IsNullOrEmpty(txtServerIP.Text) || !string.IsNullOrEmpty(txtServerPort.Text))
            {
                btnServerStart.Enabled = true;
            }
            else
            {
                btnServerStart.Enabled = false;
            }
        }

    }
}