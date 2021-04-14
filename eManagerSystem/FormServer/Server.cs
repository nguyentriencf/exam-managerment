using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eManagerSystem.Application.Catalog.Server;

namespace FormServer
{
    public partial class Server : Form
    {
       // ServerService server = new ServerService();
        IServerService _server;
       

        int counter = 0;
        System.Timers.Timer countdown;

        public Server(IServerService server)
        {
        
            
            _server = server;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            countdown = new System.Timers.Timer();
            countdown.Interval = 1000;
            countdown.Elapsed += Countdown_Elapsed;

        }

        private void Countdown_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            counter -= 1;
            int minute = counter / 60;
            int second = counter % 60;
            lblTimeLeft.Text = minute + " : " + second;
            if (counter == 0)
            {
                countdown.Stop();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _server.Connect();
        }

        private void cmdBatDauLamBai_Click(object sender, EventArgs e)
        {
            counter = _server.BeginExam(txtThoiGianLamBai.Text,this.counter,countdown);
        }
        private OpenFileDialog openFileDialog1;
        // them de thi

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                { 
                    var PathName = openFileDialog1.FileName;
                    _server.Send(PathName);
                }
                catch
                {
                    MessageBox.Show("Loi mo file"); 
                }
            }
        }
        private void cmdChapNhan_Click(object sender, EventArgs e)
        {
            cmdBatDauLamBai.Visible = true;
        }
    }
}
