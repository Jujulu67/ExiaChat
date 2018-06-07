using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ExiaServer
{
    class Connection:WaitingThread
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
                //Démarrage du thread avant la première connexion client
                Thread getReadClients = new Thread(new ThreadStart(getRead));
                getReadClients.Start();
                //Démarrage du thread vérifiant l'état des connexions clientes
                Thread pingPongThread = new Thread(new ThreadStart(CheckIfStillConnected));
                pingPongThread.Start();
                //Boucle infinie
                while (true)
                {

                    // Console.WriteLine("Attente d'une nouvelle connexion...");
                    Model.Logs.coMsg = "Attente d'une nouvelle connexion...";
                     //L'exécution du thread courant est bloquée jusqu'à ce qu'un
                     //nouveau client se connecte
                    CurrentClient = ServerSocket.Accept();
                    Console.WriteLine("Nouveau client:" + CurrentClient.GetHashCode());
                    //Stockage de la ressource dans l'arraylist acceptlist
                    acceptList.Add(CurrentClient);
                }
            }
            catch (SocketException E)
            {
                Console.WriteLine(E.Message);
            }

        }

       
    }
}
