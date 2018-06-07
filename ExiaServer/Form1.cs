using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExiaServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Model.Logs.coExist = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Model.Logs.coExist)
            {
                Model.Logs.coExist = false;
                Connection con = new Connection();
                textBox1.Text = textBox1.Text + Model.Logs.coMsg + "\r\n";
                
            }
            else
            {
                textBox1.Text = textBox1.Text + "gay\r\n";
            }
            textBox1.SelectionStart = textBox1.TextLength; //permet autoscroll textbox
            textBox1.ScrollToCaret();
        }
    }
}
