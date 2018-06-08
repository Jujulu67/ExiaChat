using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExiaServer.Model;

namespace ExiaServer.Controller
{
    public class Controller
    {
        public Connection co;

        public Controller(Connection co)
        {
            this.co = new Connection();
        }
    }
}
