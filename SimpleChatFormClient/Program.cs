using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SimpleChatFormClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmClient());                       
        }        
    }





    class ClientConnector
    {
        public bool Connected { get; set; } = false;    
        TcpClient tcpClient;
        IPAddress ip;
        int port;
        string name = "Alice";


        public string GetName() { return this.name; }

        public string ConnectClientInfo(TcpClient client, IPAddress ip, int port, string name)
        {
            try
            {
                this.tcpClient = client;
                this.ip = ip;
                this.port = port;
                if (name != null || name !="")
                {
                    this.name = name;
                }                
                client.Connect(ip, port);
                Connected = true;
                return "Conected to the server: " + ip + " , port: " + port;
            }
            catch (Exception ex)
            {
                return "No response from server... " + ex.ToString();
            }
        }

        public string DisconnectClient(TcpClient client)
        {
            try
            {
                client.Client.Shutdown(SocketShutdown.Send);
                client.Close();
                return "Disconnected from the server... ";
            }
            catch (Exception ex)
            {
                return "Something went wrong... " + ex.ToString();
            }
        }
    }


}
