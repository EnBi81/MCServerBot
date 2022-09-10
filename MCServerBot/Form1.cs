using MCWebServer.Hamachi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCWebServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = HamachiClient.GetStatus();

            textBox2.Text = $"Nickname: {data.NickName + Environment.NewLine}Ip: {data.Address + Environment.NewLine}Online: {data.Online}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var data = HamachiClient.LogOff();

            textBox2.Text = data.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var data = HamachiClient.LogOn();

            textBox2.Text = data.ToString();
        }
    }
}
