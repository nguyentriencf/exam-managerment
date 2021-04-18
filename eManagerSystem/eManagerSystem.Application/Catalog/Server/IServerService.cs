using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static eManagerSystem.Application.Catalog.Server.ServerService;
using System.Windows.Forms;

namespace eManagerSystem.Application.Catalog.Server
{
   public interface IServerService
    {
        event UpdateHandler EventUpdateHandler;

        void Connect();
         void Send(string filePath);
        void sendDe(string namede);
        void Receive(object sọcket);

        void Close();

        byte[] GetFilePath(string filePath);

        object Deserialize(byte[] data);
        byte[] Serialize(object data);

        void SendUser(List<Students> students);

        void SendSubject(string subject);
        List<Students> ReadAll(int gradeId);

        IEnumerable<Grade> getAllGrade();

        IEnumerable<Subject> getAllSubject();
        void SaveFile(byte[] data, int dataLength);
        int BeginExam(string inputTime, int counter, System.Timers.Timer countdown);
        void CheckActiveUser(List<string> list, Socket client, string mssv, string ip);
        void AcceptUserSuccess(string message, string ip, string mssv);
        void AcceptUserNotSuccess(Socket client, string message);
        void SendMessasge(string message, ListView listMess);
        void AddMessage(string ms, ListView list);
        void SendUserFromFile(string option, IEnumerable<object> students);
        void SendActiveControl();
        void DisconectClient();
    }
}
