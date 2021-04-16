using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormServer
{
    public partial class InsertAreaIP : Form
    {
        public List<string> _listIpArea { get; set; }
        public InsertAreaIP(List<string> listIP)
        {
            _listIpArea = listIP;
         
            InitializeComponent();

        }
        public delegate void UpdateHandler(object sender, UpdateEventArgs args);
        public event UpdateHandler EventUpdateHandler;
        public class UpdateEventArgs : EventArgs
        {
            public List<string> listIp { get; set; }

        }
        public void Updates(string MSSV)
        {
            UpdateEventArgs args = new UpdateEventArgs();

            args.listIp = _listIpArea;
            EventUpdateHandler.Invoke(this, args);


        }

        private void btnIPAreaIP_Click(object sender, EventArgs e)
        {
            ValidateIP(); 
        }
        public void ValidateIP()
        {

            if (tbAreaIP.Text != string.Empty)
            {
                if(_listIpArea.Any(ip => ip == tbAreaIP.Text) != true)
                {
                    _listIpArea.Add(tbAreaIP.Text);
                    MessageBox.Show("Thêm vùng ip thành công");
                }   
                else
                {
                    MessageBox.Show("Đã tồn tại ip");
                }      
            }
            else
            {
                MessageBox.Show("Vui lòng nhập vùng ip");
            }
        }

    }
}
