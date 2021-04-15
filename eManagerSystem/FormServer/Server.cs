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
using eManagerSystem.Application;
using eManagerSystem.Application.Catalog.Server;

namespace FormServer
{
    public partial class Server : Form
    {
       // ServerService server = new ServerService();
        IServerService _server;
        private List<PC> listUser = new List<PC>();
        List<Students> _students;
        private Color ColorRed = Color.FromArgb(255, 95, 79);
        private Color ColorGreen = Color.FromArgb(54, 202, 56);

        int counter = 0;
        System.Timers.Timer countdown;

        public Server(IServerService server)
        {
        
            
            _server = server;
            _server.EventUpdateHandler += _server_EventUpdateHandler;
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
                cmdBatDauLamBai.Visible = false;
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
            if (cbChonMonThi.Text != string.Empty)
            {
                _server.SendSubject(cbChonMonThi.Text);
            }
            cmdBatDauLamBai.Visible = true;
        }
        private void LoadDisPlayUser()
        {
            flowLayoutContainer.Controls.Clear();
            for (int i = 0; i < listUser.Count; i++)
            {
                flowLayoutContainer.Controls.Add(listUser[i]);

            }
        }
        private void AddListUser(List<Students> students)
        {
            if (students.Count > 0)
            {
                int index = 0;
                foreach (var items in students)
                {
                    index++;
                    PC pC = new PC();
                    pC.MSSV = items.MSSV.ToString();
                    pC.pcName = index.ToString();
                    pC.ColorUser = ColorRed;
                    listUser.Add(pC);
                }
            }

        }
        private void UpdateUserControll(string mssv)
        {
            foreach (var items in listUser)
            {
                if (items.MSSV == mssv)
                {
                    items.ColorUser = ColorGreen;

                }
            }
            LoadDisPlayUser();

        }
        private void _server_EventUpdateHandler(object sender, ServerService.UpdateEventArgs args)
        {
            UpdateUserControll(args.mssv);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            IEnumerable<Grade> grades = _server.getAllGrade();
            OpenDSSV open = new OpenDSSV(grades, _server);
            open.EventUpdateHandler += Open_EventUpdateHandler;
            open.Show();
        }

        private void Open_EventUpdateHandler(object sender, OpenDSSV.UpdateEventArgs args)
        {
            _students = args.studentsDelegate;
            AddListUser(_students);
            LoadDisPlayUser();
            _server.SendUser(_students);
        }

        private void cbChonMonThi_Click(object sender, EventArgs e)
        {

            IEnumerable<Subject> subjects = _server.getAllSubject();
            cbChonMonThi.DataSource = subjects;
            cbChonMonThi.DisplayMember = "SubjectName";
            cbChonMonThi.ValueMember = "SubjectId";
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }
    }
}
