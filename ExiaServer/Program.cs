using ExiaServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExiaServer
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Connection con = new Connection();
            Controller.Controller controller = new Controller.Controller(con);
            Form1 view = new Form1(controller);
            controller.co.AddObserver(view);
            Application.Run(view);

        }

    }
}
