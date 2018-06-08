using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExiaServer.Model
{
    public class WaitingThread : IModel
    {
        protected Model.Logs log = Model.Logs.GetInstance;
        ArrayList readList = new ArrayList(); //liste utilisée par socket.select 
        public bool readLock = false;//Flag aidant à la synchronisation
        public bool useLogging = false; //booleen permettant de logger le processing dans un fichier log
        byte[] msg;//Message sous forme de bytes pour socket.send et socket.receive
        string msgString = null; //contiendra le message envoyé aux autres clients
        string msgDisconnected = null; //Notification connexion/déconnexion
        public ArrayList acceptList = new ArrayList();
        public Hashtable MatchList = new Hashtable();

        public void sendMsg(byte[] msg)

        {

            for (int i = 0; i < acceptList.Count; i++)

            {

                if (((Socket)acceptList[i]).Connected)

                {

                    try

                    {

                        int bytesSent = ((Socket)acceptList[i]).Send(msg, msg.Length, SocketFlags.None);

                    }

                    catch

                    {

                        Console.Write(((Socket)acceptList[i]).GetHashCode() + " déconnecté");

                    }

                }

                else

                {

                    acceptList.Remove((Socket)acceptList[i]);

                    i--;

                }

            }

        }



        public void sendMsg(string message)

        {

            for (int i = 0; i < acceptList.Count; i++)

            {

                if (((Socket)acceptList[i]).Connected)

                {

                    try

                    {

                        byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);

                        int bytesSent = ((Socket)acceptList[i]).Send(msg, msg.Length, SocketFlags.None);

                        Console.WriteLine("Writing to:" + acceptList.Count.ToString());

                    }

                    catch

                    {

                        Console.Write(((Socket)acceptList[i]).GetHashCode() + " déconnecté");

                    }

                }

                else

                {

                    acceptList.Remove((Socket)acceptList[i]);

                    i--;

                }

            }

        }



        private void writeToAll()
        {
            sendMsg(msg);
        }

        public void Logging(string message)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.Now + ": " + message);
            }
        }

        private bool checkNick(string nick, Socket Resource)
        {
            if (MatchList.ContainsValue(nick))
            {
                //Le pseudo est déjà pris, on refuse la connexion.
                ((Socket)acceptList[acceptList.IndexOf(Resource)]).Shutdown(SocketShutdown.Both);
                ((Socket)acceptList[acceptList.IndexOf(Resource)]).Close();
                acceptList.Remove(Resource);
                Console.WriteLine("Pseudo déjà pris");
                return false;
            }
            else
            {
                MatchList.Add(Resource, nick);
            }
            return true;
        }
        private void removeNick(Socket Resource)
        {
            Console.Write("DECONNEXION DE:" + MatchList[Resource]);
            msgDisconnected = ((string)MatchList[Resource]).Trim() + " vient de se déconnecter!";
            Thread DiscInfoToAll = new Thread(new ThreadStart(infoToAll));
            DiscInfoToAll.Start();
            DiscInfoToAll.Join();
            MatchList.Remove(Resource);
        }

        private void infoToAll()
        {
            sendMsg(msgDisconnected);
        }

        public void getRead()
        {
            while (true)
            {
                readList.Clear();
                for (int i = 0; i < acceptList.Count; i++)
                {
                    readList.Add((Socket)acceptList[i]);
                }
                if (readList.Count > 0)
                {
                    Socket.Select(readList, null, null, 1000);
                    for (int i = 0; i < readList.Count; i++)
                    {
                        if (((Socket)readList[i]).Available > 0)
                        {
                            readLock = true;
                            int paquetsReceived = 0;
                            long sequence = 0;
                            string Nick = null;
                            string formattedMsg = null;
                            while (((Socket)readList[i]).Available > 0)
                            {
                                msg = new byte[((Socket)readList[i]).Available];
                                ((Socket)readList[i]).Receive(msg, msg.Length, SocketFlags.None);
                                msgString = System.Text.Encoding.UTF8.GetString(msg);
                                if (paquetsReceived == 0)
                                {
                                    string seq = msgString.Substring(0, 6);
                                    try
                                    {
                                        sequence = Convert.ToInt64(seq);
                                        Nick = msgString.Substring(6, 15);
                                        formattedMsg = Nick.Trim() + " a écrit:" +
                                        msgString.Substring(20, (msgString.Length - 20));
                                    }
                                    catch
                                    {
                                        //Ce cas de figure ne devrait normalement
                                        //jamais se produire. Il peut se produire uniquement
                                        //si un client développé par quelqu'un d'autre
                                        //tente de se connecter sur le serveur.
                                        Console.Write("Message non conforme");
                                        acceptList.Remove(((Socket)readList[i]));
                                        break;
                                    }
                                }
                                else
                                {
                                    formattedMsg = msgString;
                                }
                                msg = System.Text.Encoding.UTF8.GetBytes(formattedMsg);
                                if (sequence == 1)
                                {
                                    if (!checkNick(Nick, ((Socket)readList[i])))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        string rtfMessage = Nick.Trim() + " vient de se connecter";
                                        log.coMsg = rtfMessage;
                                    }
                                }
                                if (useLogging)
                                {
                                    Logging(formattedMsg);
                                }
                                //Démarrage du thread renvoyant le message à tous les clients
                                Thread forwardingThread = new Thread(new ThreadStart(writeToAll));
                                forwardingThread.Start();
                                forwardingThread.Join();
                                paquetsReceived++;
                            }
                            readLock = false;
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        public void CheckIfStillConnected()
        {
            /* Etant donné que la propriété .Connected d'une socket n'est pas
             * mise à jour lors de la déconnexion d'un client sans que l'on ait
             * prélablement essayé de lire ou d'écrire sur cette socket, cette méthode
             * parvient à déterminer si une socket cliente s'est déconnectée grce à la méthode
             * poll. On effectue un poll en lecture sur la socket, si le poll retourne vrai et que
             * le nombre de bytes disponible est 0, il s'agit d'une connexion terminée*/
            while (true)
            {
                for (int i = 0; i < acceptList.Count; i++)
                {
                    if (((Socket)acceptList[i]).Poll(10, SelectMode.SelectRead) && ((Socket)acceptList[i]).Available == 0)
                    {
                        if (!readLock)
                        {
                            log.coMsg ="Client " + ((Socket)acceptList[i]).GetHashCode() + " déconnecté";
                            removeNick(((Socket)acceptList[i]));
                            ((Socket)acceptList[i]).Close();
                            acceptList.Remove(((Socket)acceptList[i]));
                            i--;
                        }
                    }
                }
                Thread.Sleep(5);
            }
        }
        //Vérifie que le pseudo n'est pas déjà attribué à un autre utilisateur
        //La Hashtable matchlist ne sert qu'à ça. Pour des développements ultérieurs, elle
        //pourrait aussi servir à envoyer la liste de tous les connectés aux utilisateurs


        //public delegate void DelegChange();
        //public event DelegChange event_change;

        private List<IObserver> listObs = new List<IObserver>();

        public void AddObserver(IObserver obs)
        {
            listObs.Add(obs);
        }
        public void RemoveObserver(IObserver obs)
        {
            listObs.Remove(obs);
        }

        public void Notify(string text)
        {
            foreach(IObserver obs in listObs)
            {
                obs.SetText(text);
            }
        }

        public void SetText(string text)
        {
            log.coMsg = text;
            Notify(text);
        }

    }

}


