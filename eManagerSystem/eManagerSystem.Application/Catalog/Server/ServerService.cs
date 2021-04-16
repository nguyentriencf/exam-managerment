using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using eManagerSystem.Application.Catalog.Commom;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace eManagerSystem.Application.Catalog.Server
{
  

   public class ServerService : IServerService
    {
        
        
        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;
        ServerReponse serverReponse = new ServerReponse();
        private readonly string strCon = @"SERVER=DESKTOP-UPDAPIH\SQLEXPRESS01;Database=ExamManagernent;Integrated security =true";

   

        public void Connect()
        {
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(IP);

            Thread Listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }
              
            });
            Listen.IsBackground = true;
            Listen.Start();
                

        }
        public void Send(string filePath)
        {
            
            foreach( Socket client in clientList)
            {
                if (filePath != String.Empty)
                {
                    serverReponse.Type = ServerResponseType.SendFile;
                    serverReponse.DataResponse = GetFilePath(filePath);
                    client.Send(Serialize(serverReponse));
                    //   client.Send(GetFilePath(filePath));
                   
                }
            }
           
        }
       

        public void  Receive(object socket)
        {
             var client =  socket as Socket;
            try
            {
                while (true)
                {

                    byte[] data = new byte[1024 * 5000];
                        client.Receive(data);
                        ServerReponse serverReponse = new ServerReponse();
                        serverReponse = (ServerReponse)Deserialize(data);
                        switch (serverReponse.Type)
                        {
                            case ServerResponseType.SendFile:
                                byte[] receiveBylength = (byte[])serverReponse.DataResponse;
                               SaveFile(receiveBylength, receiveBylength.Length);

                                break;
                             case ServerResponseType.SendAcceptUser:
                            string ipUser = client.RemoteEndPoint.ToString().Split(':')[0];
                            var mssv = (string)serverReponse.DataResponse;
                            Updates(mssv);
                            
                            break;
                            

                        default:
                                break;
                        }
                    }
                  
                
            }
            catch (Exception er)
            {
                //throw er;
                 Close();
            }
        }


        public void SaveFile(byte[] data, int dataLength)
        {
            string pathSave = "D:/receive/";
            int fileNameLength = BitConverter.ToInt32(data, 0);
            string nameFile = Encoding.ASCII.GetString(data, 4, fileNameLength);
            string nameFolder = Path.GetFileName(nameFile);
            string root = pathSave + nameFolder;
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            foreach ( string Files in Directory.EnumerateFiles(nameFile))
            {
                string name = root + "/" + Path.GetFileName(Files);
            BinaryWriter writer = new BinaryWriter(File.Open(name, FileMode.Append));
                int count = dataLength - 4 - fileNameLength;
                writer.Write(data, 4 + fileNameLength, count);
            }
        
        }



        public void Close()
        {
            server.Close();
        }

        public byte[] GetFilePath(string filePath)
        {
          //  var name = Path.GetFileName(filePath);
            byte[] fNameByte = Encoding.ASCII.GetBytes(filePath);
            byte[] fileData = File.ReadAllBytes(filePath);
            byte[] serverData = new byte[4 + fNameByte.Length + fileData.Length];
            byte[] fNameLength = BitConverter.GetBytes(fNameByte.Length);
            fNameLength.CopyTo(serverData, 0);
            fNameByte.CopyTo(serverData, 4);
            fileData.CopyTo(serverData,4+fNameByte.Length);
            return serverData;
        }

        public byte[] Serialize(object data)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            return stream.ToArray();
        }
        public object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
          return  formatter.Deserialize(stream);
           
        }
        public int BeginExam(string inputTime, int counter, System.Timers.Timer countdown)
        {
          

            int minute = Convert.ToInt32(inputTime);
            counter = minute * 60;
            countdown.Enabled = true;

            serverReponse.Type =ServerResponseType.BeginExam;
            serverReponse.DataResponse = minute;
            byte[] buffer = Serialize(serverReponse);

            foreach (Socket client in clientList)
            {
                try
                {
                    client.Send(buffer);
                }
                catch (Exception ex)
                {
                    clientList.Remove(client);
                    client.Close();
                }
            }
            return counter;
        }
        private void hasParameter(SqlCommand cmd, string query, object[] para = null)
        {
            int i = 0;
            foreach (string parameter in query.Split(' ').ToArray().Where(p => p.Contains('@')))
            {
                cmd.Parameters.AddWithValue(parameter, para[i]);

                i++;
            }
        }
        public DataTable ExcuteDataReader(string query, object[] para = null)
        {
            try
            {
                DataTable data = new DataTable();
                using (SqlConnection conn = new SqlConnection(strCon))
                {

                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (para != null)
                    {

                        {
                            hasParameter(cmd, query, para);
                        }

                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(data);


                }
                return data;
            }
            catch (Exception err)
            {
                throw err;
            }

        }

        public IEnumerable<Students> readAll(int gradeId)
        {
            DataTable dataTable = ExcuteDataReader("usp_getAllStudentBySubject @gradeId", new object[] { gradeId });
            List<Students> listStudents = new List<Students>();
            foreach (DataRow row in dataTable.Rows)
            {
                Students students = new Students(row);
                listStudents.Add(students);

            }
            return listStudents;
        }

        public List<Students> ReadAll(int gradeId)
        {
            return (List<Students>)readAll(gradeId);
        }

        public IEnumerable<Grade> getAllGrade()
        {
            DataTable dataTable = ExcuteDataReader("usp_getGrade");
            List<Grade> listGrades = new List<Grade>();
            foreach (DataRow row in dataTable.Rows)
            {
                Grade grades = new Grade(row);
                listGrades.Add(grades);

            }
            return listGrades;
        }

        public void SendUser( List<Students> students)
        {
            foreach (Socket client in clientList)
            {
                            
                    serverReponse.Type = ServerResponseType.SendStudent;
                    serverReponse.DataResponse = students;
                    client.Send(Serialize(serverReponse));
                   
                
            }
        }

        public void SendSubject(string subject)
        {
            foreach (Socket client in clientList)
            {
                if (subject != String.Empty)
                {
                    serverReponse.Type = ServerResponseType.SendSubject;
                    serverReponse.DataResponse = subject;             
                    client.Send(Serialize(serverReponse));
                }
            }
        }

        public IEnumerable<Subject> getAllSubject()
        {
            DataTable dataTable = ExcuteDataReader("usp_getSubjects");
            List<Subject> listSubject = new List<Subject>();
            foreach (DataRow row in dataTable.Rows)
            {
                Subject subject = new Subject(row);
                listSubject.Add(subject);

            }
            return (IEnumerable<Subject>)listSubject;
        }
        public delegate void UpdateHandler(object sender, UpdateEventArgs args);
        public event UpdateHandler EventUpdateHandler;
        public class UpdateEventArgs : EventArgs
        {
            public string mssv { get; set; }

        }
        public void Updates(string MSSV)
        {
            UpdateEventArgs args = new UpdateEventArgs();

            args.mssv = MSSV;
            EventUpdateHandler.Invoke(this, args);


        }

        public void CheckActiveUser(List<string> list, Socket client, string mssv, string ip)
        {
            foreach (var item in list)
            {
           
                if (list.Any(p => p == item))
                {
                    string message = "Chấp nhận thành công";
                    AcceptUserSuccess(message,ip,mssv);
                }
                else
                {
                    AcceptUserNotSuccess(client, "Từ chối chấp");
                }
            }
        }

        public void AcceptUserSuccess(string message, string ip, string mssv)
        {
          
            foreach (Socket client in clientList)
            {
                string ipUser = client.RemoteEndPoint.ToString().Split(':')[0];
            
                if (ipUser == ip)
                {
                    serverReponse.Type = ServerResponseType.sendSuccess;
                    serverReponse.DataResponse = message;
                    client.Send(Serialize(serverReponse));
                    Updates(mssv);
                    break;
                }

            }
        }

        public void AcceptUserNotSuccess(Socket client, string message)
        {
            serverReponse.Type = ServerResponseType.sendFali;
            serverReponse.DataResponse = message;
            client.Send(Serialize(serverReponse));
            
        }

        public void SendMessasge(string message, List<string>IPArea, ListView listMess)
        {
            foreach (Socket client in clientList)
            {
                string ipuser = client.RemoteEndPoint.ToString().Split(':')[0];

                if (IPArea.Any(p => p == ipuser))
                {
                    serverReponse.Type = ServerResponseType.senMessage;
                    serverReponse.DataResponse = message;
                    client.Send(Serialize(serverReponse));
                    AddMessage(message, listMess);
                }
                
            }
           
        }

        public void AddMessage(string ms, ListView listMess)
        {
            listMess.Items.Add(new ListViewItem() { Text = ms });
        }

    
    }
}
