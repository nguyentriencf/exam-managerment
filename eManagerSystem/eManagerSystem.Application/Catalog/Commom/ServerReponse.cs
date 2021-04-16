using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eManagerSystem.Application.Catalog.Commom
{
    [Serializable]
    public enum ServerResponseType
    {
        sendSuccess,
        sendFali,
        SendFile,
        SendList,
        SendStudent,
        SendString,
        BeginExam,
        FinishExam,
        LockClient,
        SendSubject,
       SendAcceptUser
    }
    [Serializable]
    public class ServerReponse
    {
      
            public ServerResponseType Type { get; set; }
            public object DataResponse { get; set; }
     
    }
}
