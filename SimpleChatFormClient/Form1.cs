/**************************************************
 *              Simple server - Client
 * This project is made in process of learning about threads and socket implementation.
 * Here are the steps I have taken in the process of creating this app.
 * 
 * Taking into account server was already created, 
 * steps I took in this file:
 * 1) Creation and form objects renaming
 * 2) Decision on which button gets enabled and disabled and setting initial values
 * 3) Name and server txt information collecting 
 * 4) ClientConnector created and connect to the server enabled
 * 5) Sending message implementation and testing
 * 6) ReceiveData and PrintReceivedData in thread safe way
 * 7) Removing unused code
 * ***********************************************
 * To be added:
 *      Disconnect option
 *      
 * 
 * ***********************************************/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SimpleChatFormClient
{

    public partial class frmClient : Form
    {
        int port;
        IPAddress ip;
        TcpClient client = new TcpClient();        
        ClientConnector clientConnector = new ClientConnector();
        string clientName;        
        

        public frmClient()
        {
            InitializeComponent();
            btnMessageSend.Enabled = false;
            btnServerConnect.Enabled = false;
            txtClientName.Text = "Alice";
            txtServerIP.Text = "127.0.0.1";
            txtServerPort.Text = "8888";
        }

        private void txtMessageDisplay_TextChanged(object sender, EventArgs e)
        {

        }


        private void btnClientName_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtClientName.Text) || !string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                clientName = txtClientName.Text;
                btnServerConnect.Enabled = true;                
                txtMessageDisplay.Text = txtMessageDisplay.Text +">>Chat name set to: " + txtClientName.Text + Environment.NewLine;

            }
            else btnServerConnect.Enabled = false;

        }


        private void btnServerConnect_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtServerIP.Text) && !string.IsNullOrEmpty(txtServerPort.Text))
            {                
                IPAddress.TryParse(txtServerIP.Text, out ip);
                int.TryParse(txtServerPort.Text, out port);                
            }

            var connection = clientConnector.ConnectClientInfo(client, ip, port, clientName);

            // changes on the form
            btnClientName.Enabled = false;
            txtClientName.Enabled = false;
            btnServerConnect.Enabled = false;
            txtServerIP.Enabled = false;
            txtServerPort.Enabled = false;
            btnMessageSend.Enabled = true;
            txtMessageDisplay.Text = txtMessageDisplay.Text + ">>"+ connection.ToString() + Environment.NewLine;            

            Thread threadMessageReceive = new Thread(new ParameterizedThreadStart(ReceiveData));
            threadMessageReceive.Start(client);                                   
        }



        private void btnMessageSend_Click(object sender, EventArgs e)
        {
            if (clientConnector.Connected)
            {
                if (!string.IsNullOrEmpty(txtMessageSend.Text) || !string.IsNullOrWhiteSpace(txtMessageSend.Text))                    
                {
                    NetworkStream networkStream = client.GetStream();                    
                    byte[] buffer = Encoding.ASCII.GetBytes(clientConnector.GetName() + ": " + txtMessageSend.Text);
                    networkStream.Write(buffer, 0, buffer.Length);
                    txtMessageSend.Clear();
                }
            }                        
        }
        

        delegate void StringArgReturningVoidDelegate(string text);


        //public void ReturnMessage()
        //{
        //    while (true)
        //    {
        //        //Thread.Sleep(500);                
        //        SetText("This text was set safely.");
        //        //txtMessageDisplay.Text = txtMessageDisplay.Text + "Nesto" + Environment.NewLine;
        //    }            
        //}

        public void ReceiveData(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream networkStream = tcpClient.GetStream();
            byte[] receivedByteData = new byte[1024];
            int receivedDataSize;
            while ((receivedDataSize = networkStream.Read(receivedByteData, 0, receivedByteData.Length)) > 0)
            {
                PrintReceivedData(Encoding.ASCII.GetString(receivedByteData, 0, receivedDataSize));                
            }
        }


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
                txtMessageDisplay.Text = txtMessageDisplay.Text + text;
            }
        }

    }
}
