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
using LinqToExcel;

namespace FormServer
{
    public partial class Server : Form
    {

        List<studentFromExcels> studentFromExcels;
        public List<string> listIPArea = new List<string>();
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

        private OpenFileDialog openFileDialog1;
        string PathNameSubjectExam;
        // them de thi

        private void button3_Click(object sender, EventArgs e)
        {

            openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PathNameSubjectExam = openFileDialog1.FileName;
                    string fileName = Path.GetFileName(PathNameSubjectExam);

                    lstDeThi.Items.Add(fileName);
                    //_server.Send(PathName);
                }
                catch
                {
                    MessageBox.Show("Loi mo file"); 
                }
            }
        }

        private void cmdBatDauLamBai_Click(object sender, EventArgs e)
        {
            counter = _server.BeginExam(txtThoiGianLamBai.Text, this.counter, countdown);
            _server.sendDe(cbChonMonThi.Text);
            string pathSubjectExam = PathNameSubjectExam;
            _server.Send(pathSubjectExam);
            MessageBox.Show("Bắt đầu tính thời gian làm bài kiểm tra");


        }
        private void cmdChapNhan_Click(object sender, EventArgs e)
        {
           
            if (cbChonMonThi.Text != string.Empty && txtThoiGianLamBai.Text != string.Empty && lstDeThi.Items.Count >0)
            {
                cmdBatDauLamBai.Visible = true;
                MessageBox.Show("Đã đầy đủ thông tin của bài thi");
                //_server.SendSubject(cbChonMonThi.Text);     
            }
            else
            {
                MessageBox.Show("kiểm tra lai thông tin của bài kiểm tra");
            }    
          
           
        }
    
        //private void AddListUser(List<Students> students)
        //{
        //    if (students.Count > 0)
        //    {
        //        int index = 0;
        //        foreach (var items in students)
        //        {
        //            index++;
        //            PC pC = new PC();

        //            pC.MSSV = items.MSSV.ToString();
        //            pC.pcName = index.ToString();
        //            pC.ColorUser = ColorRed;
        //            listUser.Add(pC);
        //        }
        //    }

        //}
        private void AddListUser(List<string> listArea)
        {
            if (listArea.Count > 0)
            {
                int index = 0;
                foreach (var items in listArea)
                {
                    index++;
                    PC pC = new PC();
                    pC.ipclient = items;
               
                    pC.pcName = index.ToString();
                    pC.ColorUser = ColorRed;
                    listUser.Add(pC);
                }
            }

        }
        private void LoadDisPlayUser()
        {
            flowLayoutContainer.Controls.Clear();
            for (int i = 0; i < listUser.Count; i++)
            {
                flowLayoutContainer.Controls.Add(listUser[i]);

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
            OpenFileDialog oFile = new OpenFileDialog();
            oFile.ShowDialog();
            if (oFile.FileName != string.Empty)
            {
                string file = oFile.FileName;
                string exetention = Path.GetExtension(file);
                if (exetention.ToLower() == ".xls" || exetention.ToLower().Equals(".xlsx"))
                {
                    var excel = new ExcelQueryFactory(file);
                    var students = from s in excel.Worksheet<studentFromExcels>("Sheet1")
                                   select s;
                    studentFromExcels = new List<studentFromExcels>();
                    foreach (var item in students)
                    {
                        studentFromExcels.Add(item);
                    }
                    if (studentFromExcels.Count > 0)
                    {
                        SendStudentsFromExcel(studentFromExcels);
                    }
                }

            }
        }
        private void SendStudentsFromExcel(List<studentFromExcels> StudentFromExcels)
        {
            _server.SendUserFromFile("Send UserFromExcel", StudentFromExcels);
        }

        private void cmdNhapVungIP_Click(object sender, EventArgs e)
        {
            InsertAreaIP insertAreaIP = new InsertAreaIP(listIPArea);
            insertAreaIP.EventUpdateHandler += InsertAreaIP_EventUpdateHandler;
            insertAreaIP.Show();
        
        }

        private void InsertAreaIP_EventUpdateHandler(object sender, InsertAreaIP.UpdateEventArgs args)
        {
            listIPArea = args.listIp;
            AddListUser(listIPArea);
            LoadDisPlayUser();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var messageToAll = new frmMessageToAll(_server);
            messageToAll.Show();
        }

        private void cmdKichHoatAllClient_Click(object sender, EventArgs e)
        {
            _server.SendActiveControl();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _server.DisconectClient();
        }
    }
}
