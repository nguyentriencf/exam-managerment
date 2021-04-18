using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace clientForm
{
    public partial class Message : Form
    {
        string _mess;
        public Message(string mess)
        {
            _mess = mess;
            InitializeComponent();
            ReceiveMess(mess);
        }

        public void ReceiveMess(string mess)
        {
            listView1.Items.Add(mess);
        }
       
    }
}
