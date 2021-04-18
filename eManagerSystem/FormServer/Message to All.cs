using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eManagerSystem.Application.Catalog.Server;

namespace FormServer
{
    public partial class frmMessageToAll : Form
    {
        IServerService _server;
      
        public frmMessageToAll(IServerService server)
        {
            _server = server;
         
            InitializeComponent();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            _server.SendMessasge(txtContentMessage.Text,lvMessage);
        }
    }
}
