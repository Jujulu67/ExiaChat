using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ExiaServer
{
    class Connection
    {
        public Connection()
        {
            //Récupération de l'IP du serveur
            IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            Socket CurrentClient = null;
            Socket ServerSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
            Model.Logs.coMsg = "SERVEUR ON, IP=" + ipAddress.ToString();
            Logging(Model.Logs.coMsg);
            try
            {
                //On lie la socket au point de communication
                ServerSocket.Bind(new IPEndPoint(ipAddress, 8000));
                //On la positionne en mode "écoute"
                ServerSocket.Listen(10);
            }
            catch (SocketException E)
            {
                Console.WriteLine(E.Message);
            }
        }

        private void Logging(string message)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.Now + ": " + message);
            }
        }
    }
}
