using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExiaServer.Model
{
    public class Connection:WaitingThread
    {
        IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress;
        Socket CurrentClient = null;
        Socket ServerSocket = new Socket(AddressFamily.InterNetwork,
        SocketType.Stream,
        ProtocolType.Tcp);
        
        public Connection()
        {           

        }

        public void Waiting()
        {
            //L'exécution du thread courant est bloquée jusqu'à ce qu'un
            //nouveau client se connecte
            CurrentClient = ServerSocket.Accept();
            
            //Stockage de la ressource dans l'arraylist acceptlist
            acceptList.Add(CurrentClient);

            Thread w = new Thread(new ThreadStart(Waiting));
            w.Start();
        }
        
        public void Initialize()
        {
            //Récupération de l'IP du serveur
            ipAddress = ipHostEntry.AddressList[0];
            SetText("SERVEUR ON, IP=" + ipAddress.ToString(),false);
          
            try
            {
                //On lie la socket au point de communication
                ServerSocket.Bind(new IPEndPoint(ipAddress, 8000));
                //On la positionne en mode "écoute"
                ServerSocket.Listen(10);
                //Démarrage du thread avant la première connexion client
                Thread getReadClients = new Thread(new ThreadStart(getRead));
                getReadClients.Start();
                //Démarrage du thread vérifiant l'état des connexions clientes
                Thread pingPongThread = new Thread(new ThreadStart(CheckIfStillConnected));
                pingPongThread.Start();
                //thread pour l'attente de co
                Thread w = new Thread(new ThreadStart(Waiting));
                w.Start();

            }
            catch (SocketException E)
            {
                Console.WriteLine(E.Message);
            }
        }
       
    }
}
