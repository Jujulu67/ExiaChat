using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExiaServer.Model
{
    public class Logs
    {
        private static Logs instance = null;
        private static readonly object padlock = new object();
        private string _coMsg;
        public string coMsg
        {
            get{
                return _coMsg;
            }
            set
            {
                _coMsg = value;
                
            }
        }
        private bool _coExist;
        public bool coExist { get; set; }

        Logs()
        {
        }

        public static Logs GetInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Logs();
                    }
                    return instance;
                }
            }
        }

    }
}
