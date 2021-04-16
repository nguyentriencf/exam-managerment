using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using eManagerSystem.Application.Catalog.Commom;
using eManagerSystem.Application;

namespace clientForm
{
    public partial class Client : Form
    {
        int counter = 0;
        System.Timers.Timer countdown;
        public Client()
        {

            InitializeComponent();
            countdown = new System.Timers.Timer();
            countdown.Interval = 1000;
            countdown.Elapsed += Countdown_Elapsed;
        }

        private void Countdown_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            counter -= 1;
            int minute = counter / 60;
            int second = counter % 60;
            SetCounter(minute, second);

            if (counter == 0)
            {


                countdown.Stop();
                FinishExam();
                Close();
            }


        }

        IPEndPoint IP;
        Socket client;
        public void Connect()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            IP = new IPEndPoint(IPAddress.Parse(myIP), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Khong the ket noi toi server");
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        public void Send(string message)
        {
            if (message != String.Empty)
            {
                client.Send(Serialize(message));
            }
        }
        
        public void Receive()
        {
            try
            {
                while (true)
                {

                    byte[] data = new byte[1024 * 5000];

                    client.Receive(data);
                    ServerReponse serverReponse = new ServerReponse();
                    serverReponse= (ServerReponse)Deserialize(data);


                    switch (serverReponse.Type)
                    {
                        case ServerResponseType.SendFile:
                            byte[] receiveBylength = (byte[])serverReponse.DataResponse;
                            string nameLink = SaveFile(receiveBylength, receiveBylength.Length);
                            SetText(nameLink);
                            break;
                        case ServerResponseType.BeginExam:
                            object timeExam = serverReponse.DataResponse;
                            int minute = int.Parse(timeExam.ToString());
                           
                            SetTime(timeExam,minute);
                           /* lblThoiGian.Text = timeExam.ToString() + " Phút";
                            int minute = Int32.Parse(timeExam.ToString());
                            counter = minute * 60;
                            countdown.Enabled = true;*/
                            break;
                        case ServerResponseType.SendStudent:
                            var userList = (List<Students>)serverReponse.DataResponse;
                            SetData(userList);
                            break;
                        case ServerResponseType.SendSubject:
                            var subject = (string)serverReponse.DataResponse;
                            SetSubject(subject);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch(Exception er)
            {
              //  throw er;
               Close();
            }
        }
        delegate void SetTextCallback(string text);

        private void SetSubject(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblMonThi.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetSubject);
                lblMonThi.Invoke(d, new object[] { text });
            }
            else
            {
                this.lblMonThi.Text = text;
            }
        }
        delegate void SetDataSourceCallBack(List<Students> students);

        private void SetData(List<Students> students)
        {

            if (this.cbDSThi.InvokeRequired)
            {
                SetDataSourceCallBack d = new SetDataSourceCallBack(SetData);
                this.Invoke(d, new object[] { students });
            }
            else
            {
                this.cbDSThi.DataSource = students;
                this.cbDSThi.DisplayMember = "FullName";
                this.cbDSThi.ValueMember = "MSSV";
            }
        }
        delegate void SetCounterCallback(int minute, int second);
        private void SetCounter(int minute, int second)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblDeThi.InvokeRequired)
            {
                SetCounterCallback d = new SetCounterCallback(SetCounter);
                this.Invoke(d, new object[] { minute,second });
            }

            else
            {
                this.lblThoiGianConLai.Text = minute + " : " + second; ;
            }
        }

        delegate void SetTimeCallback(object time,int mintute);


        private void SetTime(object time,int minute)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblThoiGian.InvokeRequired)
            {
                SetTimeCallback d = new SetTimeCallback(SetTime);
                this.Invoke(d, new object[] { time ,minute });
            }

            else
            {
               
                this.lblThoiGian.Text = time.ToString() + " Phút";
                counter = minute * 60;
                
                countdown.Enabled = true;
            }
        }

  


        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lblDeThi.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            
            else
            {
                this.lblDeThi.Text = text;
            }
        }

        
        public void Close()
        {
            client.Close();
        }

        private byte[] Serialize(object data)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, data);
            return memoryStream.ToArray();
        }

       
        private string SaveFile(byte[] data, int dataLength)
        {
            string pathSave = "D:/";
            int fileNameLength = BitConverter.ToInt32(data, 0);
            string nameFile = Encoding.ASCII.GetString(data,4, fileNameLength);
            string name = pathSave +Path.GetFileName(nameFile);
            
            BinaryWriter writer = new BinaryWriter(File.Open(name, FileMode.Append));
            int count = dataLength - 4 - fileNameLength;
            writer.Write(data, 4 + fileNameLength,count);
            return name;
        }

        private void cmdKetNoi_Click(object sender, EventArgs e)
        {
            try
            {
                Connect();
                MessageBox.Show("Kết nối Server thành công");
            }
            catch 
            {

                MessageBox.Show("Kết nối Server thất bại");
            }
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lblDeThi_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.lblDeThi.LinkVisited = true; 
            System.Diagnostics.Process.Start(this.lblDeThi.Text);
        }
    
        public byte[] GetFilePath(string filePath)
        {
            //  var name = Path.GetFileName(filePath);
           
           
            byte[] fNameByte = Encoding.ASCII.GetBytes(filePath);
            string nameFile = Directory.EnumerateFiles(filePath).FirstOrDefault();
            byte[] fileData = File.ReadAllBytes(nameFile);
            byte[] serverData = new byte[4 + fNameByte.Length + fileData.Length];
            byte[] fNameLength = BitConverter.GetBytes(fNameByte.Length);
            fNameLength.CopyTo(serverData, 0);
            fNameByte.CopyTo(serverData, 4);
            fileData.CopyTo(serverData, 4 + fNameByte.Length);
            return serverData;
        }
        public void SendToServer(string filePath)
        {
            try
            {
                if (filePath != String.Empty)
                {
                    ServerReponse serverReponse = new ServerReponse();
                    serverReponse.Type = ServerResponseType.SendFile;
                    serverReponse.DataResponse = GetFilePath(filePath);
                    client.Send(Serialize(serverReponse));
                    MessageBox.Show("Nộp bài thành công");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
       
              
        }
        private OpenFileDialog openFileDialog1;
        private void FinishExam()
        {
            try
            {

                var PathName = @"D:\odiaZ";
                string nameDic = Directory.GetDirectories(PathName).FirstOrDefault();


                SendToServer(nameDic);
            }
            catch
            {
                MessageBox.Show("Loi mo file");
            }
        }
        private void cmdNopBaiThi_Click(object sender, EventArgs e)
        {
            FinishExam();

    
        }
        public object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
          return  formatter.Deserialize(stream);
           
        }
        public void SendAcceptUser(string ipClient)
        {
            if (ipClient != String.Empty)
            {
                ServerReponse serverReponse = new ServerReponse();
                serverReponse.Type = ServerResponseType.SendAcceptUser;
                serverReponse.DataResponse = ipClient;
                client.Send(Serialize(serverReponse));
            }

        }

        private void cmdChapNhan_Click(object sender, EventArgs e)
        {
            if (lblMaSo.Text != string.Empty)
            {
                SendAcceptUser(lblMaSo.Text);
                MessageBox.Show("Ket noi success!");

            }
            else
            {
                MessageBox.Show("Ban chua chon Ten thi sinh");
            }
        }

        private void cbDSThi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDSThi.SelectedItem != null)
            {

                string MSSV = cbDSThi.SelectedValue.ToString();
                if (MSSV != "eManagerSystem.Application.Students")
                {
                    lblMaSo.Text = MSSV;
                    lblHoTen.Text = cbDSThi.Text;
                }

            }
        }
    }


}
