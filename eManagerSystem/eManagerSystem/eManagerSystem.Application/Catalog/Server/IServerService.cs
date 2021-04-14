using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eManagerSystem.Application.Catalog.Server
{
   public interface IServerService
    {
       

        void Connect();
         void Send(string filePath);

        void Receive(object obj);

        void Close();

        byte[] GetFilePath(string filePath);

        object Deserialize(byte[] data);
        byte[] Serialize(object data);
      

        int BeginExam(string inputTime, int counter, System.Timers.Timer countdown);
    }
}
