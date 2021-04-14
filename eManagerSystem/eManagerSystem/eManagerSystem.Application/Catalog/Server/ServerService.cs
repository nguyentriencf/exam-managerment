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

namespace eManagerSystem.Application.Catalog.Server
{
  

   public class ServerService : IServerService
    {
        
        
        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;
        ServerReponse serverReponse = new ServerReponse();


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
       

        public void  Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);             
                }   
            }
            catch
            {
                clientList.Remove(client);
                client.Close();
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
            formatter.Deserialize(stream);
            return stream;
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


    }
}
