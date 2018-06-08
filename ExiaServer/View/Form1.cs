using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExiaServer.Model;
using System.Threading;

namespace ExiaServer
{
    public partial class Form1 : Form, IObserver
    {
        private Model.Logs log = Model.Logs.GetInstance;
        private Controller.Controller ctrl;
        delegate void SetTextDelegate(string text);

        public Form1(Controller.Controller ctrl)
        {
            InitializeComponent();
            this.ctrl = ctrl;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log.coExist = true;
            //WaitingThread w = new WaitingThread();
           // WaitingThread.DelegChange deleg = new WaitingThread.DelegChange(Print_Val);
            //w.event_change += deleg;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (log.coExist)
            {
                log.coExist = false;
                ctrl.co.Initialize();
               
                //textBox1.Text = textBox1.Text + log.coMsg + "\r\n";
                
            }
            else
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + "SERVER ALREADY ON FAG";
            }
            textBox1.SelectionStart = textBox1.TextLength; //permet autoscroll textbox
            textBox1.ScrollToCaret();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the  
            // calling thread to the thread ID of the creating thread.  
            // If these threads are different, it returns true.  
            if (this.textBox1.InvokeRequired)
            {
                SetTextDelegate d = new SetTextDelegate(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                if (text.Contains("SER"))  textBox1.Text= text;
                else textBox1.Text = textBox1.Text + Environment.NewLine + text;
            }
        }
     
        
    }
}
