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
        public TcpClient Client { get; set; }
        public Thread ReceiveDataThread { get; set; }
        IPAddress ip;
        int port;        
        string name;

        public string GetName() { return this.name; } 

        public string ConnectDiconnect(IPAddress ip, int port, string name)
        {
            try
            {
                if (!this.Connected)
                {                    
                    this.ip = ip;
                    this.port = port;
                    if (name != null || name != "") this.name = name;                    
                    Client.Connect(ip, port);
                    this.Connected = true;
                    return "Conected to the server: " + ip + " , port: " + port;
                }
                else
                {
                    this.Connected = false;
                    this.Client.Client.Shutdown(SocketShutdown.Both);
                    this.Client.Close();
                    return "Disonnected from the server...";
                }

            }
            catch (Exception ex)
            {
                return "No response from server... " + ex.ToString();
            }
        }   
    }


}
