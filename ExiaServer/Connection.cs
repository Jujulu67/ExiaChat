using System;
using System.Collections.Generic;
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

        }


    }
}
